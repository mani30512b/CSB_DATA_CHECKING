using CSB_DATA_CHECKING.Helper.ExceptionLogHelper; // [Logger Added]
using CSB_DATA_CHECKING.Models;
using CSB_DATA_CHECKING.Rules;
using OfficeOpenXml;

namespace CSB_DATA_CHECKING.Services
{
    public class CsbValidatorService : ICsbValidatorService
    {
        private readonly List<ICsbRule> _rules;
        private readonly Rule1_FileNameRule _fileNameRule;
        private readonly ExceptionLogger _logger = new ExceptionLogger(); // [Logger Added]

        public CsbValidatorService()
        {
            _fileNameRule = new Rule1_FileNameRule();
            _rules = new List<ICsbRule>
            {
                _fileNameRule,
                new Rule2_ColumnDataTypes()
            };
        }

        public Task<CsbValidationResults> ValidateCsbFileAsync(IFormFile file)
        {
            var result = new CsbValidationResults
            {
                FileName = Path.GetFileName(file.FileName),
                CellErrors = new List<CsbCellError>(),
                PassedRules = new List<string>(),
                FailedRules = new List<string>(),
                ErrorRows = new List<CsbRow>()
            };

            try // [Logger Added]
            {
                var headers = Rule1_FileNameRule.GetExcelHeaders(file);


                if (headers == null || headers.Count == 0)
                {
                    result.CellErrors.Add(new CsbCellError
                    {
                        RowNumber = 0,
                        ColumnName = "Header",
                        Message = "The uploaded file is empty or invalid."
                    });
                    result.Success = false;
                    result.Message = "Validation failed.";
                    return Task.FromResult(result);
                }

                if (!_fileNameRule.ValidateFileLevel(result.FileName, headers.Count, result))
                {
                    result.Success = false;
                    result.Message = "File-level validation failed.";
                    return Task.FromResult(result);
                }

                var dataLines = new List<List<string>>();

                using (var stream = file.OpenReadStream())
                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets[0];
                    int rowCount = worksheet.Dimension.End.Row;
                    int colCount = worksheet.Dimension.End.Column;

                    for (int row = 2; row <= rowCount; row++)
                    {
                        var cols = new List<string>();
                        for (int col = 1; col <= colCount; col++)
                        {
                            var cellValue = worksheet.Cells[row, col].Text?.Trim() ?? string.Empty;
                            cols.Add(cellValue);
                        }
                        dataLines.Add(cols);
                    }
                }

                foreach (var rule in _rules)
                {
                    bool ruleFailed = false;

                    for (int i = 0; i < dataLines.Count; i++)
                    {
                        var row = new CsbRow
                        {
                            RowNumber = i + 2,
                            Columns = dataLines[i]
                        };

                        int prevErrorCount = result.CellErrors.Count;

                        rule.Validate(headers, row, row.RowNumber, result);

                        int newErrorCount = result.CellErrors.Count;

                        if (newErrorCount > prevErrorCount)
                        {
                            ruleFailed = true;

                            if (!result.ErrorRows.Any(r => r.RowNumber == row.RowNumber))
                            {
                                result.ErrorRows.Add(row);
                            }
                        }
                    }

                    if (ruleFailed)
                        result.FailedRules.Add(rule.RuleName);
                    else
                        result.PassedRules.Add(rule.RuleName);
                }

                result.Success = result.CellErrors.Count == 0;
                result.Message = result.Success ? "Validation passed." : "Validation failed.";
            }
            catch (Exception ex)
            {
                _logger.ExceptionLog("CsbValidatorService", ex.ToString()); // [Logger Added]
                result.Success = false;
                result.Message = "An error occurred during validation.";
            }

            return Task.FromResult(result);
        }
    }
}
