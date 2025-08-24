namespace CSB_DATA_CHECKING.Models
{
    public class NewCsbValidationResult
    {
        public string Summary { get; set; } = string.Empty;
        public bool IsSuccess { get; set; }
        public Dictionary<string, string> SuccessfulRules { get; set; } = new();
        public Dictionary<string, string> UnsuccessfulRules { get; set; } = new();
    }
}
