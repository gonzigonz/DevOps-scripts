***************************************************** FilePoller.exe *****************************************************
Gonzalo Lucero
9th Feb 2015
The MIT License (MIT)
Source Code avalible at https://github.com/gonzigonz/DevOps-scripts

File Poller is a general purpose .Net console application that can be used to poll for the existence of files on the Windows platform.

HELP

 FilePoller.exe <searchString> <pollingInterval> <maxPollingAttempts>

 <searchString>          A search string to a filepath to search for. It can
                         be a wild card however any file matching will
                         evaluate to true.
                         *Filepaths with spaces require quotes.

 <pollingInterval>       The time in seconds to wait between polls. Can not
                         be zero.

 <maxPollingAttempts>    The number of attempts to poll for file. Can not
                         be zero.

EXAMPLE

    FilePoller.exe "C:\temp\test file_*.log" 5 12
