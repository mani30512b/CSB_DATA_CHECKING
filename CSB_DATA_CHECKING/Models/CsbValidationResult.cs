namespace CSB_DATA_CHECKING.Models
{
    public class CsbValidationResults
    {
        public bool Success { get; set; } = false;
        public string Message { get; set; } = string.Empty;
        public List<string> Errors { get; set; } = new();
        public List<string> PassedRules { get; set; } = new();
        public List<string> FailedRules { get; set; } = new();
        public List<CsbRow> ErrorRows { get; set; } = new();

        // ✅ Add this line:
        public string FileName { get; set; } = string.Empty;
    }
}
