using System;

namespace MSBD.Test.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            
            var myStringArray  = new string[]{"select LinkId, *, FileName from [dbo].[BLOBS2]"};
                foreach (var argument in myStringArray)
            {
                Console.WriteLine(argument);
            }

            Console.WriteLine();
            Console.WriteLine("Starting MS SQL Blob Downloader (MSBD) Service...");
            var blobDownloaderService = new MssqlBlobDownloaderService(
                "Server=luc-gonzalo;Database=test;Trusted_Connection=True;",
                myStringArray[0],
                @"C:\Temp\Test's",
                "Object",
                "FileName"
                );

            Console.WriteLine();
            Console.WriteLine(DateTime.Now);
            Console.WriteLine("Running MS SQL Blob Downloader (MSBD) Service...");
            blobDownloaderService.Download();
            Console.WriteLine(DateTime.Now);

            Console.WriteLine();
            Console.ReadLine();
        }
    }
}
