using CSB_DATA_CHECKING.Helper.ExceptionLogHelper; //  Added for logger
using CSB_DATA_CHECKING.Helpers;
using CSB_DATA_CHECKING.Models;
using System.Globalization;
using System.Text.RegularExpressions;

namespace CSB_DATA_CHECKING.Rules
{
    public class Rule2_ColumnDataTypes : ICsbRule
    {
        public string RuleName => "Rule2: Column Data Types";

        //public void Validate(List<string> headers, CsbRow row, int rowIndex, CsbValidationResults result)
        //{
        //    //  Logger instance
        //    var logger = new ExceptionLogger();

        //    try //Outer try-catch for the entire row validation
        //    {
        //        for (int i = 0; i < headers.Count; i++)
        //        {
        //            string header = headers[i];

        //            if (!CsbColumnTypeMapping.ColumnTypes.TryGetValue(header, out var expectedType))
        //                continue;

        //            if (i >= row.Values.Count)
        //                continue;

        //            string? rawValue = row.Values[i];
        //            string cellValue = rawValue?.Trim() ?? string.Empty;

        //            // Skip if value is null/empty/NA
        //            if (string.IsNullOrWhiteSpace(cellValue))
        //            {
        //                continue;
        //            }

        //            if (cellValue.Equals("NA", StringComparison.OrdinalIgnoreCase) ||
        //                cellValue.Equals("--") ||
        //                cellValue.Equals("null", StringComparison.OrdinalIgnoreCase) ||
        //                cellValue.Equals("00/00/0000"))
        //            {
        //                result.CellErrors.Add(new CsbCellError
        //                {
        //                    RowNumber = row.RowNumber,
        //                    ColumnName = header,
        //                    Message = $"Invalid placeholder value '{cellValue}' in column '{header}'"
        //                });
        //                continue;
        //            }

        //            bool isValid = true;

        //            try //Try-catch inside type checking block
        //            {
        //                if (expectedType == typeof(DateTime))
        //                {
        //                    //isValid = DateTime.TryParse(cellValue, out _);
        //                    isValid = DateTime.TryParseExact(
        //                            cellValue,
        //                            new[] { "dd-MMM-yyyy", "dd/MM/yyyy", "yyyy-MM-dd" },
        //                            CultureInfo.InvariantCulture,
        //                            DateTimeStyles.None,
        //                            out _
        //                        );

        //                }
        //                else if (expectedType == typeof(int))
        //                {
        //                    isValid = int.TryParse(cellValue, out _);
        //                }
        //                else if (expectedType == typeof(decimal))
        //                {
        //                    //isValid = decimal.TryParse(cellValue, out _);
        //                    isValid = decimal.TryParse(cellValue, NumberStyles.Any, CultureInfo.InvariantCulture, out _);

        //                }

        //                if (!isValid)
        //                {
        //                    result.CellErrors.Add(new CsbCellError
        //                    {
        //                        RowNumber = row.RowNumber,
        //                        ColumnName = header,
        //                        Message = $"Invalid {expectedType.Name} format in column '{header}'"
        //                    });
        //                    continue;
        //                }

        //                if (expectedType == typeof(string) && IsSensitiveColumn(header) && IsOnlyNumeric(cellValue))
        //                {
        //                    result.CellErrors.Add(new CsbCellError
        //                    {
        //                        RowNumber = row.RowNumber,
        //                        ColumnName = header,
        //                        Message = $"Value '{cellValue}' in column '{header}' appears numeric-only but should contain a proper name"
        //                    });
        //                }
        //            }
        //            catch (Exception ex) // Catch individual cell validation error
        //            {
        //                logger.ExceptionLog("Rule2_ColumnDataTypes -> Validate (cell validation)", ex.ToString());
        //            }
        //        }
        //    }
        //    catch (Exception ex) //Catch general row validation error
        //    {
        //        logger.ExceptionLog("Rule2_ColumnDataTypes -> Validate (row)", ex.ToString());
        //    }
        //}


        public void Validate(List<string> headers, CsbRow row, int rowIndex, CsbValidationResults result)
        {
            for (int i = 0; i < headers.Count; i++)
            {
                string header = headers[i];

                if (!CsbColumnTypeMapping.ColumnTypes.TryGetValue(header, out var expectedType))
                    continue;

                if (i >= row.Values.Count || string.IsNullOrWhiteSpace(row.Values[i]?.Trim()))
                    continue;

                string cellValue = row.Values[i]?.Trim() ?? string.Empty;


                // Skip placeholder values
                if (cellValue.Equals("NA", StringComparison.OrdinalIgnoreCase) ||
                    cellValue.Equals("--") ||
                    cellValue.Equals("null", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                bool isValid = true;
                string errorMessage = string.Empty;

                string[] acceptedFormats = new[]
                {
                    "dd-MM-yyyy",
                    "dd/MM/yyyy",
                    "dd-MMM-yyyy",
                    "dd/MMM/yyyy",
                    "yyyy-MM-dd",
                    "yyyy/MM/dd",
                    "MM/dd/yyyy",
                    "MMM dd, yyyy",
                    "dd.MM.yyyy",
                    "M/d/yyyy",
                    "dd MMM yyyy",
                    "dd MMM, yyyy"
                };

                if (expectedType == typeof(DateTime))
                {
                    bool isDateValid = false;

                    // First, try normal parse (handles true DateTime types from Excel)
                    if (DateTime.TryParse(cellValue, CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
                    {
                        isDateValid = true;
                    }
                    else
                    {
                        // Try against all accepted string formats
                        isDateValid = DateTime.TryParseExact(
                            cellValue.Trim(),
                            acceptedFormats,
                            CultureInfo.InvariantCulture,
                            DateTimeStyles.None,
                            out _
                        );
                    }

                    if (!isDateValid)
                    {
                        errorMessage = $"Invalid date format. Supported formats: {string.Join(", ", acceptedFormats)}";
                        isValid = false;
                    }


                }

                else if (expectedType == typeof(decimal))
                {
                    if (!decimal.TryParse(
                        cellValue,
                        NumberStyles.AllowDecimalPoint,
                        CultureInfo.InvariantCulture,
                        out _))
                    {
                        errorMessage = $"Invalid decimal number format";
                        isValid = false;
                    }
                }
                else if (expectedType == typeof(string) && IsSensitiveColumn(header) && IsOnlyNumeric(cellValue))
                {
                    result.CellErrors.Add(new CsbCellError
                    {
                        RowNumber = row.RowNumber,
                        ColumnName = header,
                        Message = $"Value '{cellValue}' in column '{header}' appears numeric-only but should contain a proper name"
                    });
                }

                if (!isValid)
                {
                    result.CellErrors.Add(new CsbCellError
                    {
                        RowNumber = row.RowNumber,
                        ColumnName = header,
                        Message = errorMessage
                    });
                }
            }
        }

        public bool ValidateFileLevel(string fileName, int actualColumnCount, CsbValidationResults result)
        {
            // Logger instance
            var logger = new ExceptionLogger();

            try
            {
                return true;
            }
            catch (Exception ex) // Catch if any file-level logic is added later
            {
                logger.ExceptionLog("Rule2_ColumnDataTypes -> ValidateFileLevel", ex.ToString());
                return true;
            }
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

