// Gonzalo Lucero
// 9th Feb 2015
// The MIT License (MIT)

using System;

namespace FilePoller
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("======================== FILE POLLER ========================");
            Console.WriteLine("Run Time: {0}", DateTime.Now);

            try
            {
                var validator = new ParamValidator(args);
                validator.Validate();

                Console.WriteLine("Search String: {0}", validator.SearchString);
                Console.WriteLine("Polling Interval: {0}", validator.PollingInterval);
                Console.WriteLine("Maximum Poles: {0}", validator.MaxPollingAttempts);
                Console.WriteLine();

                var pollingEngine = new PollingEngine(
                validator.SearchString,
                validator.PollingInterval,
                validator.MaxPollingAttempts);

                var searchResult = pollingEngine.Run();

                Console.WriteLine(searchResult ? "Search successful, file found!" : "Search failed, no file found!");
            }
            catch (Exception e)
            {
                Console.WriteLine();
                Console.WriteLine("Error: {0}",e.Message);
                ShowHelpMessage();
            }

            Console.WriteLine();
            Console.WriteLine("End Time: {0}", DateTime.Now);
        }

        private static void ShowHelpMessage()
        {
            Console.WriteLine();
            Console.WriteLine("HELP");
            Console.WriteLine();
            Console.WriteLine(" FilePoller.exe <searchString> <pollingInterval> <maxPollingAttempts>");
            Console.WriteLine();
            Console.WriteLine(" <searchString>          A search string to a filepath to search for. It can");
            Console.WriteLine("                         be a wild card however any file matching will");
            Console.WriteLine("                         evaluate to true.");
            Console.WriteLine("                         *Filepaths with spaces require quotes.");
            Console.WriteLine();
            Console.WriteLine(" <pollingInterval>       The time in seconds to wait between polls. Can not");
            Console.WriteLine("                         be zero.");
            Console.WriteLine();
            Console.WriteLine(" <maxPollingAttempts>    The number of attempts to poll for file. Can not");
            Console.WriteLine("                         be zero.");
            Console.WriteLine();
            Console.WriteLine("EXAMPLE");
            Console.WriteLine();
            Console.WriteLine(@"    FilePoller.exe ""C:\temp\test file_*.log"" 5 12");
        }
    }
}
