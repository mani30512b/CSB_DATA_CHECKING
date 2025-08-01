namespace CSB_DATA_CHECKING.Models
{
    public class ErrorRow
    {
        public int RowNumber { get; set; }
        public List<string> Columns { get; set; } = new();
    }
}
