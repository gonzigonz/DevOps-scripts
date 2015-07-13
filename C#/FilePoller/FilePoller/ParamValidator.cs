using System;
using System.IO;

namespace FilePoller
{
    internal class ParamValidator
    {
        private readonly string[] _args;
        private string _searchString;
        private int _maxPollingAttempts;
        private int _pollingInterval;

        public ParamValidator(string[] args)
        {
            _args = args;
        }

        internal string SearchString
        {
            get { return _args[0]; }
        }

        internal int PollingInterval
        {
            get { return _pollingInterval; }
        }

        internal int MaxPollingAttempts
        {
            get { return _maxPollingAttempts; }
        }

        internal void Validate()
        {
            CheckCorrectNumberOfArgumentsGiven();
            CheckFristArgumentHasValidDirectoryPath();
            CheckSecondArgumentIsValidPollingInterval();
            CheckThridArgumentIsValidMaximumPolesInt();
        }

        private void CheckCorrectNumberOfArgumentsGiven()
        {
            if (_args.Length < 3)
            {
                throw new ArgumentException("Invalid syntax. Missing a parrameter");
            }
        }

        private void CheckFristArgumentHasValidDirectoryPath()
        {
            _searchString = _args[0];
            var directory = Path.GetDirectoryName(_searchString);
            if (String.IsNullOrEmpty(directory) || !Directory.Exists(directory))
            {
                throw  new ArgumentException(String.Format(
                    "Invalid search parameter. Directory '{0}' not found!",
                    directory));
            }
        }

        private void CheckSecondArgumentIsValidPollingInterval()
        {
            try
            {
                _pollingInterval = Convert.ToInt32(_args[1]);
            }
            catch (Exception)
            {
                throw new ArgumentException(String.Format(
                    "Invalid polling interval parameter - '{0}'!",
                    _args[1]));
            }

            if (_pollingInterval == 0)
            {
                throw new ArgumentException("Invalid polling interval. Can not be zero!");
            }
        }

        private void CheckThridArgumentIsValidMaximumPolesInt()
        {
            try
            {
                _maxPollingAttempts = Convert.ToInt32(_args[2]);
            }
            catch (Exception)
            {
                throw new ArgumentException(String.Format(
                    "Invalid max polling attempts parameter - '{0}'!",
                    _args[1]));
            }

            if (_maxPollingAttempts == 0)
            {
                throw new ArgumentException("Invalid max polling attempts. Can not be zero!");
            }
        }
    }
}
