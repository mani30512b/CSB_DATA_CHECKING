using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;

namespace CSB_DATA_CHECKING.Helpers
{
    public static class ExcelHighlightHelper
    {
        public static byte[] HighlightErrorsInExcel(Stream originalExcelStream, List<CSB_DATA_CHECKING.Models.CsbCellError> cellErrors)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using var package = new ExcelPackage(originalExcelStream);
            var worksheet = package.Workbook.Worksheets[0];

            foreach (var error in cellErrors)
            {
                int row = error.RowNumber; // +1 because Excel rows start at 1
                int col = GetColumnIndexByHeader(worksheet, error.ColumnName);

                if (col > 0)
                {
                    var cell = worksheet.Cells[row, col];
                    cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    cell.Style.Fill.BackgroundColor.SetColor(Color.LightPink);

                    // Optional: add a comment
                    cell.AddComment(error.Message, "Validator");
                }
            }

            return package.GetAsByteArray();
        }

        private static int GetColumnIndexByHeader(ExcelWorksheet sheet, string headerName)
        {
            for (int col = 1; col <= sheet.Dimension.Columns; col++)
            {
                var cellValue = sheet.Cells[1, col].Text;
                if (string.Equals(cellValue, headerName, StringComparison.OrdinalIgnoreCase))
                    return col;
            }
            return -1;
        }
    }
}



