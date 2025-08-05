namespace CSB_DATA_CHECKING.Models
{
    public class CsbRow
    {
        public int RowNumber { get; set; }
        //public List<string> Columns { get; set; }
        public required List<string> Columns { get; set; }   // ✅ Use 'required' keyword (C# 11+)


        public List<string> Values => Columns; // ✅ This resolves the "no definition for 'Values'" error
    }
}
