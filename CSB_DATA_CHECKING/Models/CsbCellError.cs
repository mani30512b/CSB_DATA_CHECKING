namespace CSB_DATA_CHECKING.Models
{
    public class CsbCellError
    {
        public int RowNumber { get; set; }
        public string ColumnName { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}
