using System;

namespace MSBD.Entities
{
    public class DownloadConfig
    {
        public string ConnectionString { get; set; }
        public int QueryTimeout { get; set; }
        public string QueryString { get; set; }
        public string DownloadPath { get; set; }
        public string BlobColumnName { get; set; }
        public string FilenameColumnName { get; set; }

        public void ClearAll()
        {
            foreach (var propInfo in GetType().GetProperties())
            {
                propInfo.SetValue(this,"",null);
            }
        }
    }
}
