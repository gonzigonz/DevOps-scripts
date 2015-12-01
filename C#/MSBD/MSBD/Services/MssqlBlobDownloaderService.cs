using System;
using System.Data.SqlClient;
using System.IO;
using MSBD.Entities;

namespace MSBD.Services
{
    public class MssqlBlobDownloaderService : IMssqlBlobDownloaderService
    {
        private DownloadConfig _currentConfig;

        public void Download(DownloadConfig config)
        {
            Program.WriteLine();
            Program.WriteLine(@"Downloading Blobs...");

            var startTime = DateTime.Now;
            var count = 0;
            _currentConfig = config;

            // Validate our data
            if (String.IsNullOrEmpty(_currentConfig.BlobColumnName))
            {
                throw new Exception("MssqlBlobDownloaderService: The Blob Column Name can not be null or blank!");
            }
            if (String.IsNullOrEmpty(_currentConfig.FilenameColumnName))
            {
                throw new Exception("MssqlBlobDownloaderService: The FileName Column Name can not be null or blank!");
            }

            var blobColumnOrdinal = GetColumnOrdinal(_currentConfig.BlobColumnName);
            var fileNameColumnOrdinal = GetColumnOrdinal(_currentConfig.FilenameColumnName);
            
            try
            {
                using (var connection = new SqlConnection(_currentConfig.ConnectionString))
                {
                    using (var sqlCommand = new SqlCommand(_currentConfig.QueryString, connection))
                    {
                        connection.Open();
                        using (var sqlDataReader = sqlCommand.ExecuteReader())

                        {
                            
                            while (sqlDataReader.Read())
                            {
                                var fileName = sqlDataReader.GetString(fileNameColumnOrdinal);
                                var fileNamePath = Path.Combine(_currentConfig.DownloadPath, fileName);
                                Program.Write("Downloading \"" + fileName + "\" to " + fileNamePath);

                                try
                                {
                                    var buffer = new byte[sqlDataReader.GetBytes(blobColumnOrdinal, 0L, null, 0, int.MaxValue)];
                                    sqlDataReader.GetBytes(blobColumnOrdinal, 0L, buffer, 0, buffer.Length);
                                    using (var fileStream = new FileStream(fileNamePath, FileMode.Create, FileAccess.Write))
                                        fileStream.Write(buffer, 0, buffer.Length);

                                    Program.WriteLine(" - Complete");
                                }
                                catch (Exception e)
                                {
                                    Program.WriteLine(" - Error (" + e + ")");
                                }

                                count += 1;
                            }
                            Program.WriteLine();
                            //Program.WriteLine("Done (" + count + " files downloaded)");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Program.WriteLine();
                Program.WriteLine("SQL Error: " + e);
            }
            finally
            {
                //// Display when done
                //var ts = DateTime.Now - startTime;
                //var duration = string.Format(
                //    "{0:00}:{1:00}:{2:00}", ts.TotalHours, ts.Minutes, ts.Seconds);

                //Program.WriteLine(@"Duration: " + duration);
                //Program.WriteLine();

                // Display when done
                var ts = DateTime.Now - startTime;
                var duration = string.Format(
                    "{0:00}:{1:00}:{2:00}", ts.TotalHours, ts.Minutes, ts.Seconds);

                Program.WriteLine(string.Format(
                    "Download Completed\t{0}\t{1} files", duration, count));
                Program.WriteLine();
            }
        }

        public void RunSqlQuery(DownloadConfig config)
        {
            Program.WriteLine(@"Running Query...");
            var startTime = DateTime.Now; 
            
            _currentConfig = config;
            var count = 0;

            try
            {
                using (var connection = new SqlConnection(_currentConfig.ConnectionString))
                {
                    using (var sqlCommand = new SqlCommand(_currentConfig.QueryString, connection))
                    {
                        connection.Open();
                        using (var sqlDataReader = sqlCommand.ExecuteReader())
                        {
                            var fieldCount = sqlDataReader.FieldCount;

                            // Get headers
                            for (var i = 0; i < fieldCount; i++)
                            {
                                Program.Write(sqlDataReader.GetName(i) + "\t");
                            }
                            Program.WriteLine();

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
                            Program.WriteLine(cache);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Program.WriteLine();
                Program.WriteLine("SQL Error: " + e);
            }
            finally
            {
                // Display when done
                var ts = DateTime.Now - startTime;
                var duration = string.Format(
                    "{0:00}:{1:00}:{2:00}", ts.TotalHours, ts.Minutes, ts.Seconds);

                Program.WriteLine(string.Format(
                    "Query Executed Successfully\t{0}\t{1} rows", duration, count));
                Program.WriteLine();
            }
        }

        private int GetColumnOrdinal(string columnName)
        {
            if (String.IsNullOrEmpty(columnName))
            {
                throw new Exception("GetColumnOrdinal(string columnName): The Column Name can not be null or blank!");
            }

            //Query SQL to find the blob column ordinal (coloumn index in table)
            using (var connection = new SqlConnection(_currentConfig.ConnectionString))
            {
                using (var sqlCommand = new SqlCommand(_currentConfig.QueryString, connection))
                {
                    connection.Open();
                    using (var sqlDataReader = sqlCommand.ExecuteReader())
                    {
                        for (var i = 0; i < sqlDataReader.FieldCount; i++)
                        {
                            if (String.IsNullOrEmpty(sqlDataReader.GetName(i))) continue;
                            if (sqlDataReader.GetName(i).ToLower() == columnName.ToLower())
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
