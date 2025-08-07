namespace CSB_DATA_CHECKING.Helper.ExceptionLogHelper
{
    public class ExceptionLogger
    {
        private static readonly string fileDirectory = Path.Combine(AppContext.BaseDirectory, "ErrorLog");
        private static readonly string logFilePath = Path.Combine(fileDirectory, "ErrorLog.txt");
        private static readonly object lockCheck = new object();

        public ExceptionLogger()
        {
            // Create directory if it doesn't exist
            if (!Directory.Exists(fileDirectory))
            {
                Directory.CreateDirectory(fileDirectory);
            }
        }

        public void ExceptionLog(string source, string exception)
        {
            string message = string.Format("Time: {0}", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt"));
            message += Environment.NewLine;
            message += "-----------------------------------------------------------";
            message += Environment.NewLine;
            message += $"Source: {source}";
            message += Environment.NewLine;
            message += $"Exception: {exception}";
            message += Environment.NewLine;
            message += "-----------------------------------------------------------";
            message += Environment.NewLine;

            lock (lockCheck)
            {
                using (StreamWriter writer = new StreamWriter(logFilePath, true))
                {
                    writer.WriteLine(message);
                }
            }
        }
    }
}
