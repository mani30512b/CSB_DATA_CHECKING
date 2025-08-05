namespace CSB_DATA_CHECKING.Models
{
    public class CsbValidationResults
    {
        public bool Success { get; set; } = false;
        public string Message { get; set; } = string.Empty;
        public List<string> PassedRules { get; set; } = new();
        public List<string> FailedRules { get; set; } = new();
        public string FileName { get; set; } = string.Empty;

        // ✅ Use the CsbCellError class from CsbCellError.cs
        public List<CsbCellError> CellErrors { get; set; } = new();

        // Optional: full row data if needed
        public List<CsbRow> ErrorRows { get; set; } = new();
    }
}
