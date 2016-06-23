using System;
using System.Globalization;
using System.Windows.Forms;
using MSBD.Entities;
using MSBD.Helpers;
using MSBD.Services;

namespace MSBD
{
    public partial class Form1 : Form
    {
        private DownloadConfig _currentConfig;
        private readonly MssqlBlobDownloaderService _downloaderService;
        private readonly ConfigService _configService;
        private string _currentOpenFilename;

        public Form1()
        {
            InitializeComponent();
            _downloaderService = new MssqlBlobDownloaderService(ApplicationType.Form);
            _configService = new ConfigService();
            _currentOpenFilename = _configService.DefaultFilename;
            ReadOrCreateDefaultConfig();
            labelConfigChangedFlag.Visible = false;
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
            }
            catch (Exception)
            {
                _currentConfig = CreateNewConfig();
                _configService.SaveToDefaultConfig(_currentConfig);
            }
            finally
            {
                RefreshGui(_currentConfig);
            }
        }

        private DownloadConfig CreateNewConfig()
        {
            return new DownloadConfig
            {
                ConnectionString = "Server = <servername>; Database = <databasename>; Trusted_Connection = True; Connection Timeout = 60",
                QueryString = "SELECT filename, blob FROM schema.tablename",
                DownloadPath = @"C:\Temp",
                BlobColumnName = "blob",
                FilenameColumnName = "filename",
                QueryTimeout = 30
            };
        }

        private void RefreshGui(DownloadConfig config)
        {
            groupBoxConfig.Text = @"Config - " + _currentOpenFilename;
            textBoxConnectionString.Text = config.ConnectionString;
            textBoxSqlQueryString.Text = config.QueryString;
            textBoxDownloadPath.Text = config.DownloadPath;
            textBoxBlobColumnName.Text = config.BlobColumnName;
            textBoxFileNameColumnName.Text = config.FilenameColumnName;
            txtQueryTimeout.Text = config.QueryTimeout.ToString();
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

            int queryTimeout;
            if (int.TryParse(txtQueryTimeout.Text, out queryTimeout))
                config.QueryTimeout = queryTimeout;

            return config;
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            textBoxOutput.Clear();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _currentConfig = CreateNewConfig();
            _currentOpenFilename = _configService.DefaultFilename;
            RefreshGui(_currentConfig);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _configService.SaveToDefaultConfig(_currentConfig);
            Close();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Save file
            _currentConfig = GetConfigFromGui();
            if (_currentOpenFilename == _configService.DefaultFilename)
            {
                _configService.SaveToDefaultConfig(_currentConfig);
            }
            else
            {
                _configService.SaveConfig(_currentConfig, _currentOpenFilename);
            }
            labelConfigChangedFlag.Visible = false;
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var saveDialog = new SaveFileDialog();
            saveDialog.Filter = @"MS SQL Blob Downloader Config|*.msbd";
            saveDialog.AddExtension = true;
            var result = saveDialog.ShowDialog();
            if (result != DialogResult.OK) return;

            // Save file as
            _currentConfig = GetConfigFromGui();
            _currentOpenFilename = saveDialog.FileName;
            _configService.SaveConfig(_currentConfig, _currentOpenFilename);
            
            // Update GUI - Specificailly the group config name
            RefreshGui(_currentConfig);
            labelConfigChangedFlag.Visible = false;
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var openDialog = new OpenFileDialog();
            openDialog.Filter = @"MS SQL Blob Downloader Config|*.msbd";
            var result = openDialog.ShowDialog();
            if (result != DialogResult.OK) return;

            // Open selected file
            _currentOpenFilename = openDialog.FileName;
            _currentConfig = _configService.ReadConfig(_currentOpenFilename);
            RefreshGui(_currentConfig);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            _configService.SaveToDefaultConfig(_currentConfig);
        }

        protected void OnConfigChanged(object sender, EventArgs e)
        {
            labelConfigChangedFlag.Visible = true;
        }
    }
}
