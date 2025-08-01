using CSB_DATA_CHECKING.Helpers;
using CSB_DATA_CHECKING.Models;

namespace CSB_DATA_CHECKING.Rules
{
    public class Rule2_ColumnDataType : ICsbRule
    {
        public string RuleName => "Rule 2 - Column Data Type Check";

        public void Validate(List<string> headers, CsbRow row, int rowNumber, CsbValidationResults result)
        {
            for (int colIndex = 0; colIndex < headers.Count && colIndex < row.Columns.Count; colIndex++)
            {
                string header = headers[colIndex];
                string value = row.Columns[colIndex];

                if (CsbColumnTypeMapping.ColumnTypes.TryGetValue(header, out Type expectedType))
                {
                    bool isValid = expectedType switch
                    {
                        var t when t == typeof(DateTime) => DateTime.TryParse(value, out _),
                        var t when t == typeof(decimal) => decimal.TryParse(value, out _),
                        var t when t == typeof(int) => int.TryParse(value, out _),
                        var t when t == typeof(string) => !string.IsNullOrWhiteSpace(value),
                        _ => true
                    };

                    if (!isValid)
                    {
                        result.Errors.Add($"[Rule 2] Row {rowNumber}, Column {colIndex + 1} ({header}): Expected {expectedType.Name}, got '{value}'");
                    }
                }
            }
        }
    }
}
