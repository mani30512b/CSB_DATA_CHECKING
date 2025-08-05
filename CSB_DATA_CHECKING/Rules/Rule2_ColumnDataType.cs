
using CSB_DATA_CHECKING.Helpers;
using CSB_DATA_CHECKING.Models;
using System.Text.RegularExpressions;

namespace CSB_DATA_CHECKING.Rules
{
    public class Rule2_ColumnDataTypes : ICsbRule
    {
        public string RuleName => "Rule2: Column Data Types";

        //public void Validate(List<string> headers, CsbRow row, int rowIndex, CsbValidationResults result)
        //{
        //    for (int i = 0; i < headers.Count; i++)
        //    {
        //        string header = headers[i];

        //        if (!CsbColumnTypeMapping.ColumnTypes.TryGetValue(header, out var expectedType))
        //            continue;

        //        if (i >= row.Values.Count)
        //            continue;

        //        string cellValue = row.Values[i];

        //        if (string.IsNullOrWhiteSpace(cellValue))
        //            continue;

        //        // Check for valid type
        //        bool isValid = expectedType == typeof(DateTime)
        //            ? DateTime.TryParse(cellValue, out _)
        //            : expectedType == typeof(int)
        //                ? int.TryParse(cellValue, out _)
        //                : expectedType == typeof(decimal)
        //                    ? decimal.TryParse(cellValue, out _)
        //                    : true;

        //        // Add type error if invalid
        //        if (!isValid)
        //        {
        //            result.CellErrors.Add(new CsbCellError
        //            {
        //                RowNumber = row.RowNumber,
        //                ColumnName = header,
        //                Message = $"Invalid {expectedType.Name} format in column '{header}'"
        //            });
        //            continue;
        //        }

        //        // Extra check: if it's a sensitive column expecting names but contains only digits
        //        if (expectedType == typeof(string) && IsSensitiveColumn(header) && IsOnlyNumeric(cellValue))
        //        {
        //            result.CellErrors.Add(new CsbCellError
        //            {
        //                RowNumber = row.RowNumber,
        //                ColumnName = header,
        //                Message = $"Value '{cellValue}' in column '{header}' appears numeric-only but should contain a proper name"
        //            });
        //        }
        //    }
        //}

        //public void Validate(List<string> headers, CsbRow row, int rowIndex, CsbValidationResults result)
        //{
        //    for (int i = 0; i < headers.Count; i++)
        //    {
        //        string header = headers[i];

        //        if (!CsbColumnTypeMapping.ColumnTypes.TryGetValue(header, out var expectedType))
        //            continue;

        //        if (i >= row.Values.Count)
        //            continue;

        //        //string cellValue = row.Values[i]?.Trim();

        //        string cellValue = row.Values[i]?.ToString().Trim();

        //        // Stronger empty check
        //        if (string.IsNullOrWhiteSpace(cellValue) ||
        //            cellValue.Equals("NA", StringComparison.OrdinalIgnoreCase) ||
        //            cellValue.Equals("--") ||
        //            cellValue.Equals("null", StringComparison.OrdinalIgnoreCase) ||
        //            cellValue == "00/00/0000")
        //        {
        //            continue;
        //        }


        //        bool isValid = expectedType == typeof(DateTime)
        //            ? DateTime.TryParse(cellValue, out _)
        //            : expectedType == typeof(int)
        //                ? int.TryParse(cellValue, out _)
        //                : expectedType == typeof(decimal)
        //                    ? decimal.TryParse(cellValue, out _)
        //                    : true;

        //        if (!isValid)
        //        {
        //            result.CellErrors.Add(new CsbCellError
        //            {
        //                RowNumber = row.RowNumber,
        //                ColumnName = header,
        //                Message = $"Invalid {expectedType.Name} format in column '{header}'"
        //            });
        //            continue;
        //        }

        //        // Extra check for string-only columns with numeric-only values
        //        if (expectedType == typeof(string) && IsSensitiveColumn(header) && IsOnlyNumeric(cellValue))
        //        {
        //            result.CellErrors.Add(new CsbCellError
        //            {
        //                RowNumber = row.RowNumber,
        //                ColumnName = header,
        //                Message = $"Value '{cellValue}' in column '{header}' appears numeric-only but should contain a proper name"
        //            });
        //        }
        //    }
        //}


        public void Validate(List<string> headers, CsbRow row, int rowIndex, CsbValidationResults result)
        {
            for (int i = 0; i < headers.Count; i++)
            {
                string header = headers[i];

                if (!CsbColumnTypeMapping.ColumnTypes.TryGetValue(header, out var expectedType))
                    continue;

                if (i >= row.Values.Count)
                    continue;

                string? rawValue = row.Values[i];
                string cellValue = rawValue?.Trim() ?? string.Empty;

                // Skip if value is null/empty/NA
                if (string.IsNullOrWhiteSpace(cellValue) ||
                    cellValue.Equals("NA", StringComparison.OrdinalIgnoreCase) ||
                    cellValue.Equals("--") ||
                    cellValue.Equals("null", StringComparison.OrdinalIgnoreCase) ||
                    cellValue.Equals("00/00/0000"))
                {
                    continue;
                }

                bool isValid = true;

                if (expectedType == typeof(DateTime))
                {
                    // Try parsing to DateTime
                    isValid = DateTime.TryParse(cellValue, out _);
                }
                else if (expectedType == typeof(int))
                {
                    isValid = int.TryParse(cellValue, out _);
                }
                else if (expectedType == typeof(decimal))
                {
                    isValid = decimal.TryParse(cellValue, out _);
                }

                if (!isValid)
                {
                    result.CellErrors.Add(new CsbCellError
                    {
                        RowNumber = row.RowNumber,
                        ColumnName = header,
                        Message = $"Invalid {expectedType.Name} format in column '{header}'"
                    });
                    continue;
                }

                // Extra string-only name check
                if (expectedType == typeof(string) && IsSensitiveColumn(header) && IsOnlyNumeric(cellValue))
                {
                    result.CellErrors.Add(new CsbCellError
                    {
                        RowNumber = row.RowNumber,
                        ColumnName = header,
                        Message = $"Value '{cellValue}' in column '{header}' appears numeric-only but should contain a proper name"
                    });
                }
            }
        }



        public bool ValidateFileLevel(string fileName, int actualColumnCount, CsbValidationResults result)
        {
            return true;
        }

        private bool IsOnlyNumeric(string value)
        {
            return Regex.IsMatch(value.Trim(), @"^\d+$");
        }

        private bool IsSensitiveColumn(string columnName)
        {
            return new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "Name of the Borrower",
                "Name of the bank which reported borrower as default"
            }.Contains(columnName);
        }
    }
}




