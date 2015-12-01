using System;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;

namespace MSBD.Test.ConsoleApp
{
    public class MssqlBlobDownloaderService : IMssqlBlobDownloaderService
    {
        private string _connectionString;
        private string _sqlQueryString;
        private string _downloadPath;
        private string _blobColumnName;
        private string _fileNameField;

        public MssqlBlobDownloaderService(string connectionString, string sqlQueryString, string downloadPath
            , string blobColumnName, string fileNameField)
        {
            _connectionString = connectionString;
            _sqlQueryString = sqlQueryString;
            _downloadPath = downloadPath;
            _blobColumnName = blobColumnName;
            _fileNameField = fileNameField;
        }

        public void Download()
        {
            // Validate our data
            if (String.IsNullOrEmpty(_blobColumnName))
            {
                throw new Exception("MssqlBlobDownloaderService: The Blob Column Name can not be null or blank!");
            }
            if (String.IsNullOrEmpty(_fileNameField))
            {
                throw new Exception("MssqlBlobDownloaderService: The FileName Column Name can not be null or blank!");
            }

            var blobColumnOrdinal = GetColumnOrdinal(_blobColumnName);
            var fileNameColumnOrdinal = GetColumnOrdinal(_fileNameField);

            ViewSqlQueryResults();
            Console.WriteLine();

            Console.WriteLine("DOWNLOADING...");
            var startTime = DateTime.Now;
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    using (var sqlCommand = new SqlCommand(_sqlQueryString, connection))
                    {
                        connection.Open();
                        using (var sqlDataReader = sqlCommand.ExecuteReader())
                        {
                            var count = 0;

                            while (sqlDataReader.Read())
                            {
                                var fileName = sqlDataReader.GetString(fileNameColumnOrdinal);
                                var fileStreamPath = Path.Combine(_downloadPath, fileName);
                                Console.WriteLine("Downloading " + fileName + " to " + fileStreamPath + "...");

                                var buffer = new byte[sqlDataReader.GetBytes(blobColumnOrdinal, 0L, null, 0, int.MaxValue)];
                                sqlDataReader.GetBytes(blobColumnOrdinal, 0L, buffer, 0, buffer.Length);
                                using (var fileStream = new FileStream(fileStreamPath, FileMode.Create, FileAccess.Write))
                                    fileStream.Write(buffer, 0, buffer.Length);

                                count += 1;
                            }
                            Console.WriteLine();
                            Console.WriteLine("Done (" + count + " files downloaded)");

                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine();
                Console.WriteLine("SQL Error: " + e);
            }
            finally
            {
                var duration = DateTime.Now - startTime;
                Console.WriteLine("Duration: " + duration);
                Console.WriteLine();
            }
        }

        private void ViewSqlQueryResults()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var sqlCommand = new SqlCommand(_sqlQueryString, connection))
                {
                    connection.Open();
                    using (var sqlDataReader = sqlCommand.ExecuteReader())
                    {
                        var fieldCount = sqlDataReader.FieldCount;

                        Console.WriteLine("RESULTS");

                        // Get headers
                        for (var i = 0; i < fieldCount; i++)
                        {
                            Console.Write(sqlDataReader.GetName(i) + "|");
                        }
                        Console.WriteLine();

                        while (sqlDataReader.Read())
                        {
                            for (var i = 0; i < fieldCount; i++)
                            {
                                //Console.Write(sqlDataReader.GetValue(i) + "|");
                            }
                            Console.WriteLine();
                        }
                    }
                }
            }
        }

        private int GetColumnOrdinal(string columnName)
        {
            if (String.IsNullOrEmpty(columnName))
            {
                throw new Exception("GetColumnOrdinal(string columnName): The Column Name can not be null or blank!");
            }

            //Query SQL to find the blob column ordinal (coloumn index in table)
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var sqlCommand = new SqlCommand(_sqlQueryString, connection))
                {
                    connection.Open();
                    using (var sqlDataReader = sqlCommand.ExecuteReader())
                    {
                        for (var i = 0; i < sqlDataReader.FieldCount; i++)
                        {
                            if (String.IsNullOrEmpty(sqlDataReader.GetName(i))) continue;
                            if (sqlDataReader.GetName(i) == columnName)
                            {
                                return i;
                            }
                        }

                        //If we haven't returned a value by now then it doesn't exits!
                        throw new Exception("GetColumnOrdinal(string columnName): The given Column Name '" + columnName + "' was not found!");
                    }
                }
            }
        }
    }
}
