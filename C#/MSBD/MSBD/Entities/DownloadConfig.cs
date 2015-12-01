namespace MSBD.Entities
{
    public class DownloadConfig
    {
        public string ConnectionString { get; set; }
        public string QueryString { get; set; }
        public string DownloadPath { get; set; }
        public string BlobColumnName { get; set; }
        public string FilenameColumnName { get; set; }
        public override string ToString()
        {
            var value = "";
            foreach (var prop in GetType().GetProperties())
            {
                value += prop.Name + ":" + prop.GetValue(this, null);
            }
            return value;
        }
    }
}
