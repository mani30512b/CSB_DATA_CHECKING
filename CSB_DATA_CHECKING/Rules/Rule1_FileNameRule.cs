using CSB_DATA_CHECKING.Models;
using OfficeOpenXml;

namespace CSB_DATA_CHECKING.Rules
{
    public class Rule1_FileNameRule : ICsbRule
    {
        public string RuleName => "Rule 1 - Valid Filename and Column Count";

        // Updated expected filenames and their respective column counts
        private static readonly Dictionary<string, int> ExpectedFiles = new()
        {
            { "CCOD_20250131.xlsx", 40 },
            { "Covenant_20250131.xlsx", 23 },
            { "CRILC_20250131.xlsx", 5 },
            { "Testing_File.xlsx", 5 },
            { "CURRENTACCOUNT_20250131.xlsx", 4 },
            { "IEDPMS_20250131.xlsm", 19 },
            { "IEDPMS_20250131.xlsx", 18 },
            { "STOCKSTATEMENT_20250131.xlsx", 16 },
            { "TL_20250131.xlsx", 23 }
        };

        // ✅ This method extracts headers from the uploaded Excel file
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

                // ✅ Only include non-empty headers
                if (!string.IsNullOrWhiteSpace(cellValue))
                {
                    headers.Add(cellValue);
                }
            }

            return headers;
        }


        public void Validate(List<string> headers, CsbRow row, int rowIndex, CsbValidationResults result)
        {
            // Row-level validation not needed for Rule 1
        }

        // ✅ Called during file-level validation
        public bool ValidateFileLevel(string fileName, int actualColumnCount, CsbValidationResults result)
        {
            if (!ExpectedFiles.TryGetValue(fileName, out int expectedCount))
            {
                result.Errors.Add($"[Rule 1] Invalid file name: {fileName}");
                return false;
            }

            if (actualColumnCount != expectedCount)
            {
                result.Errors.Add($"[Rule 1] File '{fileName}' should have {expectedCount} columns but found {actualColumnCount}.");
                return false;
            }

            return true;
        }
    }
}
