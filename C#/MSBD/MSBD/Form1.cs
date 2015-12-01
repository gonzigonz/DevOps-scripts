using System;
using System.Globalization;
using System.Windows.Forms;
using MSBD.Entities;
using MSBD.Services;

namespace MSBD
{
    public partial class Form1 : Form
    {
        private DownloadConfig _currentConfig;
        private readonly MssqlBlobDownloaderService _downloaderService;
        private readonly ConfigService _configService;

        public Form1()
        {
            InitializeComponent();
            _downloaderService = new MssqlBlobDownloaderService();
            _configService = new ConfigService();
            ReadOrCreateDefaultConfig();
        }

        public void WriteToTextBoxOutput(String value)
        {
            textBoxOutput.AppendText(value.ToString(CultureInfo.InvariantCulture));
        }

        public void WriteLineToTextBoxOutput(String value)
        {
            WriteToTextBoxOutput(value + Environment.NewLine);
        }

        private void buttonDownload_Click(object sender, EventArgs e)
        {
            _currentConfig = GetConfigFromGui();

            try
            {
                _downloaderService.Download(_currentConfig);
                _configService.SaveToDefaultConfig(_currentConfig);
            }
            catch (Exception ex)
            {
                Program.WriteLine(ex.Source + Environment.NewLine + ex.Message);
            }

        }

        private void buttonRunQuery_Click(object sender, EventArgs e)
        {
            _currentConfig = GetConfigFromGui();

            try
            {
                _downloaderService.RunSqlQuery(_currentConfig);
                _configService.SaveToDefaultConfig(_currentConfig);
            }
            catch (Exception ex)
            {
                Program.WriteLine(ex.Source + Environment.NewLine + ex.Message);
            }
            
        }

        private void ReadOrCreateDefaultConfig()
        {
            try
            {
                _currentConfig = _configService.ReadDefaultConfig();
                RefreshGui(_currentConfig);
            }
            catch (Exception)
            {
                _currentConfig = new DownloadConfig
                {
                    ConnectionString = "Server = <servername>; Database = <databasename>; Trusted_Connection = True",
                    QueryString = "SELECT filename, blob FROM schema.tablename",
                    DownloadPath = @"C:\Temp",
                    BlobColumnName = "blob",
                    FilenameColumnName = "filename"
                };
            }
            finally
            {
                RefreshGui(_currentConfig);
            }
        }

        private void RefreshGui(DownloadConfig config)
        {
            textBoxConnectionString.Text = config.ConnectionString;
            textBoxSqlQueryString.Text = config.QueryString;
            textBoxDownloadPath.Text = config.DownloadPath;
            textBoxBlobColumnName.Text = config.BlobColumnName;
            textBoxFileNameColumnName.Text = config.FilenameColumnName;
        }

        private DownloadConfig GetConfigFromGui()
        {
            var config = new DownloadConfig
            {
                ConnectionString = textBoxConnectionString.Text,
                QueryString = textBoxSqlQueryString.Text,
                DownloadPath = textBoxDownloadPath.Text,
                BlobColumnName = textBoxBlobColumnName.Text,
                FilenameColumnName = textBoxFileNameColumnName.Text
            };
            return config;
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            textBoxOutput.Clear();
        }
    }
}
