using System;
using System.Data.SqlClient;
using System.IO;
using MSBD.Entities;
using MSBD.Helpers;

namespace MSBD.Services
{
    public class MssqlBlobDownloaderService : IMssqlBlobDownloaderService
    {
        private DownloadConfig _currentConfig;
        private readonly ApplicationType _appType;

        public MssqlBlobDownloaderService(ApplicationType appType = ApplicationType.Form)
        {
            _appType = appType;
        }

        public void Download(DownloadConfig config)
        {
            WriteLine();
            WriteLine(@"Downloading Blobs...");

            var startTime = DateTime.Now;
            var count = 0;
            _currentConfig = config;

            // Validate our data
            if (string.IsNullOrEmpty(_currentConfig.BlobColumnName))
                throw new Exception("MssqlBlobDownloaderService: The Blob Column Name can not be null or blank!");

            if (string.IsNullOrEmpty(_currentConfig.FilenameColumnName))
                throw new Exception("MssqlBlobDownloaderService: The FileName Column Name can not be null or blank!");

            var blobColumnOrdinal = GetColumnOrdinal(_currentConfig.BlobColumnName);
            var fileNameColumnOrdinal = GetColumnOrdinal(_currentConfig.FilenameColumnName);
            
            try
            {
                ExecuteCommandWithReader(sqlDataReader =>
                {
                    while (sqlDataReader.Read())
                    {
                        var fileName = sqlDataReader.GetString(fileNameColumnOrdinal);
                        var fileNamePath = Path.Combine(_currentConfig.DownloadPath, fileName);
                        WriteLine("Downloading \"{0}\" to {1}", fileName, fileNamePath);
                        try
                        {
                            var buffer = new byte[sqlDataReader.GetBytes(blobColumnOrdinal, 0L, null, 0, int.MaxValue)];
                            sqlDataReader.GetBytes(blobColumnOrdinal, 0L, buffer, 0, buffer.Length);
                            using (var fileStream = new FileStream(fileNamePath, FileMode.Create, FileAccess.Write))
                                fileStream.Write(buffer, 0, buffer.Length);

                            WriteLine(" - Complete");
                        }
                        catch (Exception e)
                        {
                            WriteLine(" - Error ({0})", e);
                        }

                        count += 1;
                    }
                    WriteLine();
                });
            }
            catch (Exception e)
            {
                WriteLine();
                WriteLine("SQL Error: {0}", e);
            }
            finally
            {
                // Display when done
                var ts = DateTime.Now - startTime;
                var duration = string.Format("{0:00}:{1:00}:{2:00}", ts.TotalHours, ts.Minutes, ts.Seconds);

                WriteLine("Download Completed\t{0}\t{1} files", duration, count);
                WriteLine();
            }
        }

        public void RunSqlQuery(DownloadConfig config)
        {
            WriteLine(@"Running Query...");
            var startTime = DateTime.Now; 
            
            _currentConfig = config;
            var count = 0;

            try
            {
                ExecuteCommandWithReader(sqlDataReader =>
                {
                    var fieldCount = sqlDataReader.FieldCount;

                    // Get headers
                    for (var i = 0; i < fieldCount; i++)
                    {
                        Write(sqlDataReader.GetName(i) + "\t");
                    }
                    WriteLine();

                    // Get records
                    var cache = "";
                    while (sqlDataReader.Read())
                    {
                        for (var i = 0; i < fieldCount; i++)
                        {
                            cache += sqlDataReader.GetValue(i) + "\t";
                        }
                        count += 1;
                        cache += Environment.NewLine;
                    }
                    WriteLine(cache);
                });
            }
            catch (Exception e)
            {
                WriteLine();
                WriteLine("SQL Error: {0}", e);
            }
            finally
            {
                // Display when done
                var ts = DateTime.Now - startTime;
                var duration = string.Format("{0:00}:{1:00}:{2:00}", ts.TotalHours, ts.Minutes, ts.Seconds);

                WriteLine("Query Executed Successfully\t{0}\t{1} rows", duration, count);
                WriteLine();
            }
        }

        private void WriteLine(string msg = "")
        {
            switch (_appType)
            {
                case ApplicationType.Console:
                    Console.WriteLine(msg);
                    break;
                case ApplicationType.Form:
                    Program.WriteLine(msg);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void WriteLine(string msg, params object[] arg)
        {
            switch (_appType)
            {
                case ApplicationType.Console:
                    Console.WriteLine(msg, arg);
                    break;
                case ApplicationType.Form:
                    Program.WriteLine(string.Format(msg, arg));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void Write(string msg, params object[] arg)
        {
            switch (_appType)
            {
                case ApplicationType.Console:
                    Console.Write(msg, arg);
                    break;
                case ApplicationType.Form:
                    Program.Write(string.Format(msg, arg));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private int GetColumnOrdinal(string columnName)
        {
            if (string.IsNullOrEmpty(columnName))
                throw new Exception("GetColumnOrdinal(string columnName): The Column Name can not be null or blank!");

            return ExecuteCommandWithReader<int>(sqlDataReader =>
            {
                for (var i = 0; i < sqlDataReader.FieldCount; i++)
                {
                    if (string.IsNullOrEmpty(sqlDataReader.GetName(i)))
                        continue;

                    if (string.Equals(sqlDataReader.GetName(i), columnName, StringComparison.CurrentCultureIgnoreCase))
                        return i;
                }

                //If we haven't returned a value by now then it doesn't exits!
                throw new Exception("GetColumnOrdinal(string columnName): The given Column Name '" + columnName + "' was not found!");
            });
        }

        private void ExecuteCommandWithReader(Action<SqlDataReader> readerExecution)
        {
            using (var connection = new SqlConnection(_currentConfig.ConnectionString))
            {
                using (var sqlCommand = new SqlCommand(_currentConfig.QueryString, connection))
                {
                    sqlCommand.CommandTimeout = _currentConfig.QueryTimeout;
                    connection.Open();
                    using (var sqlDataReader = sqlCommand.ExecuteReader())
                    {
                        readerExecution(sqlDataReader);
                    }
                }
            }
        }

        private TResult ExecuteCommandWithReader<TResult>(Func<SqlDataReader, TResult> readerExecution)
        {
            TResult result;
            using (var connection = new SqlConnection(_currentConfig.ConnectionString))
            {
                using (var sqlCommand = new SqlCommand(_currentConfig.QueryString, connection))
                {
                    sqlCommand.CommandTimeout = _currentConfig.QueryTimeout;
                    connection.Open();
                    using (var sqlDataReader = sqlCommand.ExecuteReader())
                    {
                        result = readerExecution(sqlDataReader);
                    }
                }
            }
            return result;
        }

        protected virtual bool IsFileLocked(FileInfo file)
        {
            FileStream stream = null;

            try
            {
                stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            //file is not locked
            return false;
        }
    }
}
