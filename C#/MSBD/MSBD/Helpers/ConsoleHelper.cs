using System;
using System.IO;
using System.Reflection;

namespace MSBD.Helpers
{
    public static class ConsoleHelper
    {
        private static string _exeName;

        public static void ShowHelpMessage()
        {
            if (string.IsNullOrEmpty(_exeName))
            {
                var codeBase = Assembly.GetExecutingAssembly().CodeBase;
                _exeName = Path.GetFileName(codeBase);
            }

            Console.WriteLine();
            Console.WriteLine("HELP");
            Console.WriteLine();
            Console.WriteLine("{0} <connectionString> <sqlQuery> <downloadPath> <filenameColumnName> <blobColumnName> <query_timeout - optional>", _exeName);
            Console.WriteLine();
            Console.WriteLine(" <conectionString>  - SQL server connection string to connect to.");
            Console.WriteLine();
            Console.WriteLine(" <sqlQuery>  - SQL query to return blob data.");
            Console.WriteLine("             - Qutoes are required!");
            Console.WriteLine();
            Console.WriteLine(" <downloadPath>  - The path where the file(s) will be downloaded.");
            Console.WriteLine();
            Console.WriteLine(" <filenameColumnName>  - Column name that represents the file name.");
            Console.WriteLine();
            Console.WriteLine(" <blobColumnName>  - Column name that represents the blob.");
            Console.WriteLine();
            Console.WriteLine(" <query timeout>  - The number of seconds for the query timeout. Default is 30 seconds.");
            Console.WriteLine();
            Console.WriteLine("EXAMPLE");
            Console.WriteLine();
            Console.WriteLine("{0} \"Server=LOCALHOST;Database=TEST;Trusted_Connection=TRUE;\" \"SELECT FileName, Blob from [dbo].[blob] WHERE Size > 5000\" C:\\Temp\\Downloads 60", _exeName);
        }
    }
}
