namespace CSB_DATA_CHECKING.Models
{
    public class CsbRow
    {
        public int RowNumber { get; set; }
        public List<string> Columns { get; set; } = new();
    }
}
