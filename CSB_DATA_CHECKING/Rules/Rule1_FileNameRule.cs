using CSB_DATA_CHECKING.Models;
using OfficeOpenXml;

namespace CSB_DATA_CHECKING.Rules
{
    public class Rule1_FileNameRule : ICsbRule
    {
        public string RuleName => "Rule 1 - Valid Filename and Column Count";

        private static readonly Dictionary<string, int> ExpectedFiles = new()
        {
            { "CCOD_20250131.xlsx", 43 },
            { "Covenant_20250131.xlsx", 23 },
            { "CRILC_20250131.xlsx", 5 },
            { "Testing_File.xlsx", 6 },
            { "CURRENTACCOUNT_20250131.xlsx", 4 },
            { "IEDPMS_20250131.xlsm", 19 },
            { "IEDPMS_20250131.xlsx", 18 },
            { "STOCKSTATEMENT_20250131.xlsx", 16 },
            { "TL_20250131.xlsx", 23 }
        };

        public static List<string> GetExcelHeaders(IFormFile file)
        {
            using var stream = file.OpenReadStream();
            using var package = new ExcelPackage(stream);
            var worksheet = package.Workbook.Worksheets[0];

            var headers = new List<string>();

            int colCount = worksheet.Dimension.End.Column;

            for (int col = 1; col <= colCount; col++)
            {
                var cellValue = worksheet.Cells[1, col].Text?.Trim();

                if (!string.IsNullOrWhiteSpace(cellValue))
                {
                    headers.Add(cellValue);
                }
            }

            return headers;
        }

        public void Validate(List<string> headers, CsbRow row, int rowIndex, CsbValidationResults result)
        {
            // Rule 1 does not require row-level validation
        }

        public bool ValidateFileLevel(string fileName, int actualColumnCount, CsbValidationResults result)
        {
            if (!ExpectedFiles.TryGetValue(fileName, out int expectedCount))
            {
                result.CellErrors.Add(new CsbCellError
                {
                    RowNumber = 0,
                    ColumnName = "Filename",
                    Message = $"[Rule 1] Invalid file name: {fileName}"
                });
                return false;
            }

            if (actualColumnCount != expectedCount)
            {
                result.CellErrors.Add(new CsbCellError
                {
                    RowNumber = 0,
                    ColumnName = "Header Count",
                    Message = $"[Rule 1] File '{fileName}' should have {expectedCount} columns but found {actualColumnCount}."
                });
                return false;
            }

            return true;
        }
    }
}
