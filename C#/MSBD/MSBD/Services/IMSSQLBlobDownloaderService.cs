using MSBD.Entities;

namespace MSBD.Services
{
    public interface IMssqlBlobDownloaderService
    {
        void Download(DownloadConfig config);
        void RunSqlQuery(DownloadConfig config);
    }
}
