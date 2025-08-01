using CSB_DATA_CHECKING.Models;
using CSB_DATA_CHECKING.Rules;
using OfficeOpenXml;

namespace CSB_DATA_CHECKING.Services
{
    public class CsbValidatorService : ICsbValidatorService
    {
        private readonly List<ICsbRule> _rules;
        private readonly Rule1_FileNameRule _fileNameRule;

        public CsbValidatorService()
        {
            _fileNameRule = new Rule1_FileNameRule();

            _rules = new List<ICsbRule>
            {
                _fileNameRule,
                new Rule2_ColumnDataType()
            };
        }

        public Task<CsbValidationResults> ValidateCsbFileAsync(IFormFile file)
        {
            var result = new CsbValidationResults
            {
                FileName = Path.GetFileName(file.FileName),
                Errors = new List<string>(),
                PassedRules = new List<string>(),
                FailedRules = new List<string>(),
                ErrorRows = new List<CsbRow>()
            };

            var headers = Rule1_FileNameRule.GetExcelHeaders(file);

            if (headers == null || headers.Count == 0)
            {
                result.Errors.Add("The uploaded file is empty or invalid.");
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

            // Apply all rules to each row
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

                    int prevErrorCount = result.Errors.Count;

                    rule.Validate(headers, row, row.RowNumber, result);

                    if (result.Errors.Count > prevErrorCount)
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

            result.Success = result.Errors.Count == 0;
            result.Message = result.Success ? "Validation passed." : "Validation failed.";

            return Task.FromResult(result);
        }
    }
}
