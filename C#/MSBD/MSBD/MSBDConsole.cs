using System;
using MSBD.Entities;
using MSBD.Helpers;
using MSBD.Services;

namespace MSBD
{
    public class MSBDConsole
    {
        private string[] _args;
        public string StatusMessage { get; private set; }

        private bool DownloadSuccessful { get; set; }

        public MSBDConsole(string[] args)
        {
            _args = args;
            StatusMessage = "Ready.";
        }
        
        internal void RunWithArguments()
        {
            if (HelpArgumentPassed())
            {
                ConsoleHelper.ShowHelpMessage();
                return;
            }

            var config = CreateConfigFromArgs();
            OutputConfigValues(config);

            try
            {
                var blobService = new MssqlBlobDownloaderService(ApplicationType.Console);
                blobService.Download(config);
                StatusMessage = "Download complete!";
            }
            catch (Exception e)
            {
                StatusMessage = e is IndexOutOfRangeException ? "Syntax error! Invalid number of arguments." : e.Message;
                DownloadSuccessful = false;
            }
            finally
            {
                Console.WriteLine(StatusMessage);
                if (!DownloadSuccessful)
                    ConsoleHelper.ShowHelpMessage();
            }
        }

        private void OutputConfigValues(DownloadConfig config)
        {
            Console.WriteLine();
            Console.WriteLine("Will attemp to download blob using the following parameters:");
            Console.WriteLine("connectionString: {0}", config.ConnectionString);
            Console.WriteLine("sqlQuery: \"{0}\"", config.QueryString);
            Console.WriteLine("downloadPath: {0}", config.DownloadPath);
            Console.WriteLine("filenameColumnName: {0}", config.FilenameColumnName);
            Console.WriteLine("blobColumnName: {0}", config.FilenameColumnName);
            Console.WriteLine("Query Timeout (sec): {0}", config.QueryTimeout);
            Console.WriteLine();
            Console.WriteLine("Downloading...");
        }

        private DownloadConfig CreateConfigFromArgs()
        {
            try
            {
                var config = new DownloadConfig()
                {
                    ConnectionString = _args[0],
                    QueryString = _args[1],
                    DownloadPath = _args[2],
                    FilenameColumnName = _args[3],
                    BlobColumnName = _args[4]
                };

                if (_args[5] == null)
                {
                    config.QueryTimeout = 30;
                }
                else
                {
                    int queryTimeout;
                    if (int.TryParse(_args[5], out queryTimeout))
                        config.QueryTimeout = queryTimeout;

                    config.QueryTimeout = queryTimeout;
                }

                return config;
            }
            catch (Exception e)
            {
                Console.WriteLine("Invalid argument(s)");
                Console.WriteLine(e.Message);
                Console.WriteLine();
                ConsoleHelper.ShowHelpMessage();
                throw;
            }
        }
        
        private bool HelpArgumentPassed()
        {
            return (_args[0] == "?" ||
                    _args[0] == "/?" ||
                    _args[0] == "help" ||
                    _args[0] == "/help");
        }
    }
}
