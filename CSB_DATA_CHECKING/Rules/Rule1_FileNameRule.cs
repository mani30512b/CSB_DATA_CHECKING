// Added using for the logger
using CSB_DATA_CHECKING.Helper.ExceptionLogHelper;
using CSB_DATA_CHECKING.Models;
using OfficeOpenXml;

namespace CSB_DATA_CHECKING.Rules
{
    public class Rule1_FileNameRule : ICsbRule
    {
        public string RuleName => "Rule 1 - Valid Filename and Column Count";

        //private static readonly Dictionary<string, int> ExpectedFiles = new()
        //{
        //    { "CCOD_20250131.xlsx", 43 },
        //    { "Covenant_20250131.xlsx", 23 },
        //    { "CRILC_20250131.xlsx", 5 },
        //    { "Testing_File.xlsx", 6 },
        //    { "CURRENTACCOUNT_20250131.xlsx", 4 },
        //    { "IEDPMS_20250131.xlsm", 19 },
        //    { "IEDPMS_20250131.xlsx", 18 },
        //    { "STOCKSTATEMENT_20250131.xlsx", 16 },
        //    { "TL_20250131.xlsx", 23 }
        //};

        private static readonly Dictionary<string, int> ExpectedFilePrefixes = new()
        {
            { "CCOD_", 43 },
            { "Covenant_", 24 },
            { "CRILC_", 5 },
            { "Testing_File_", 7 },
            { "CURRENT ACCOUNT_", 4 },
            { "IEDPMS_", 18 }, // .xlsm and .xlsx may need to be handled differently if needed
            { "STOCKSTATEMENT_", 16 },
            { "TL_", 23 },
            { "CURRENTACCOUNT_", 4 },
            {"New_Testing_File", 24 }
        };


        //  Injected logger instance
        private readonly ExceptionLogger _logger = new();

        public static List<string> GetExcelHeaders(IFormFile file)
        {
            var headers = new List<string>();
            try
            {
                using var stream = file.OpenReadStream();
                using var package = new ExcelPackage(stream);
                var worksheet = package.Workbook.Worksheets[0];

                int colCount = worksheet.Dimension.End.Column;

                for (int col = 1; col <= colCount; col++)
                {
                    var cellValue = worksheet.Cells[1, col].Text?.Trim();

                    if (!string.IsNullOrWhiteSpace(cellValue))
                    {
                        headers.Add(cellValue);
                    }
                }
            }
            catch (Exception ex)
            {
                // Added logging for exception in GetExcelHeaders
                new ExceptionLogger().ExceptionLog("Rule1_FileNameRule.cs - GetExcelHeaders", ex.ToString());
            }

            return headers;
        }

        public void Validate(List<string> headers, CsbRow row, int rowIndex, CsbValidationResults result)
        {
            // Rule 1 does not require row-level validation
        }

        //public bool ValidateFileLevel(string fileName, int actualColumnCount, CsbValidationResults result)
        //{
        //    try
        //    {
        //        if (!ExpectedFiles.TryGetValue(fileName, out int expectedCount))
        //        {
        //            result.CellErrors.Add(new CsbCellError
        //            {
        //                RowNumber = 0,
        //                ColumnName = "Filename",
        //                Message = $"[Rule 1] Invalid file name: {fileName}"
        //            });
        //            return false;
        //        }

        //        if (actualColumnCount != expectedCount)
        //        {
        //            result.CellErrors.Add(new CsbCellError
        //            {
        //                RowNumber = 0,
        //                ColumnName = "Header Count",
        //                Message = $"[Rule 1] File '{fileName}' should have {expectedCount} columns but found {actualColumnCount}."
        //            });
        //            return false;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // Added logging for exception in ValidateFileLevel
        //        _logger.ExceptionLog("Rule1_FileNameRule.cs - ValidateFileLevel", ex.ToString());
        //        return false;
        //    }

        //    return true;
        //}


        public bool ValidateFileLevel(string fileName, int actualColumnCount, CsbValidationResults result)
        {
            try
            {
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);

                foreach (var prefix in ExpectedFilePrefixes.Keys)
                {
                    if (fileNameWithoutExtension.StartsWith(prefix))
                    {
                        string datePart = fileNameWithoutExtension.Substring(prefix.Length);

                        if (!DateTime.TryParseExact(datePart, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out _))
                        {
                            result.CellErrors.Add(new CsbCellError
                            {
                                RowNumber = 0,
                                ColumnName = "Filename",
                                Message = $"[Rule 1] Part-1 Invalid date format in filename: {fileName}"
                            });
                            return false;
                        }

                        int expectedCount = ExpectedFilePrefixes[prefix];
                        if (actualColumnCount != expectedCount)
                        {
                            result.CellErrors.Add(new CsbCellError
                            {
                                RowNumber = 0,
                                ColumnName = "Header Count",
                                Message = $"[Rule 1] Part-2 File '{fileName}' should have {expectedCount} columns but found {actualColumnCount}."
                            });
                            return false;
                        }

                        return true;
                    }
                }

                // If no valid prefix matched
                result.CellErrors.Add(new CsbCellError
                {
                    RowNumber = 0,
                    ColumnName = "Filename",
                    Message = $"[Rule 1] Unknown or invalid filename prefix in: {fileName}"
                });
            }
            catch (Exception ex)
            {
                _logger.ExceptionLog("Rule1_FileNameRule.cs - ValidateFileLevel", ex.ToString());
            }

            return false;
        }

    }
}





