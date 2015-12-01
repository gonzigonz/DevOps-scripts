using MSBD.Entities;

namespace MSBD.Services
{
    interface IMssqlBlobDownloaderService
    {
        void Download(DownloadConfig config);
        void RunSqlQuery(DownloadConfig config);
    }
}
