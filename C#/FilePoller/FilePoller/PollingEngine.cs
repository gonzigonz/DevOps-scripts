using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace FilePoller
{
    internal class PollingEngine
    {
        private readonly string _searchDirectory;
        private readonly string _searchFilename;
        private readonly int _pollingInterval;
        private readonly int _maxPoles;

        public PollingEngine(string searchString, int pollingInterval, int maxPoles)
        {
            _searchDirectory = Path.GetDirectoryName(searchString);
            _searchFilename = Path.GetFileName(searchString);
            _pollingInterval = pollingInterval;
            _maxPoles = maxPoles;
        }

        internal bool Run()
        {
            bool isFound;
            var pollingCount = 0;
            do
            {
                Console.Write("Search attempt {0}...", ++pollingCount);

                var filesFound = Directory.GetFiles(_searchDirectory, _searchFilename);
                isFound = filesFound.Any();
                if (isFound)
                {
                    var theFileFound = filesFound.FirstOrDefault();
                    Console.WriteLine(" {0} found! - {1}", theFileFound, DateTime.Now);
                }
                else
                {
                    Console.WriteLine(" no file - {0}", DateTime.Now);
                    Thread.Sleep(_pollingInterval * 1000);
                }

            } while (!isFound && pollingCount != _maxPoles);

            return isFound;
        }
    }
}