namespace CSB_DATA_CHECKING.Helpers
{
    public static class CsbColumnTypeMapping
    {
        public static readonly Dictionary<string, Type> ColumnTypes = new(StringComparer.OrdinalIgnoreCase)
        {
            { "Sr.No", typeof(string) },
            { "Name of the Borrower", typeof(string) },
            { "Borrower Name", typeof(string) },
            { "Name of the bank which reported borrower as default", typeof(string) },
            { "Date", typeof(DateTime) },
            { "ACCOUNT_NUMBER", typeof(string) },
            { "BRANCH", typeof(string) },
            { "CLIENTID", typeof(string) },
            { "OPENDATE", typeof(DateTime) },
            { "ACC_NAME", typeof(string) },
            { "PRODUCT", typeof(string) },
            { "PRODUCT_NAME", typeof(string) },
            //{ "OUTSTANDING_LC_DEVOLVED", typeof(decimal) },
            { "OUTSTANDING_LC_DEVOLVED", typeof(string) },
            { "DATE_LC_DEVOLVED", typeof(DateTime) },
            { "OUTSTANDING_BG_INVOKED", typeof(string) },
            { "DATE_BG_INVOKED", typeof(DateTime) },
            { "HIGHEST_DB_TRAN_AMT_MONTH", typeof(decimal) },
            { "HIGHEST_CR_TRAN_AMT_MONTH", typeof(decimal) },
            { "TOTAL_CREDIT_PREV_MONTH", typeof(decimal) },
            { "LAST_CREDIT_DATE", typeof(DateTime) },
            { "PREV_MONTH_END_BALANCE", typeof(decimal) },
            { "PREV_MONTH_CASH_WITHDRWAL", typeof(decimal) },
            { "LIMIT_EXP_DATE", typeof(DateTime) },
            { "DCCO_DATE_PREV_MONTHEND", typeof(DateTime) },
            { "DCCO_DATE_2MONTH_PREV", typeof(DateTime) },
            { "BALANCE", typeof(decimal) },
            { "ACNTS_OPENING_DATE", typeof(DateTime) },
            { "EXPOSURE", typeof(decimal) },
            { "due_date", typeof(DateTime) },
            { "start_date", typeof(DateTime) },
            { "net_due_date", typeof(string) },
            { "sanction_date", typeof(string) }
        };
    }
}
