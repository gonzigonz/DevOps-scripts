using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Xml.Serialization;
using MSBD.Entities;


namespace MSBD.Services
{
    class ConfigService
    {
        private const string DEFAULT_FILENAME = "default.msbd";
        private readonly IsolatedStorageFile _userStorage;
        private readonly XmlSerializer _xmlSerializer;

        public ConfigService()
        {
            _xmlSerializer = new XmlSerializer(typeof(DownloadConfig));
            _userStorage = IsolatedStorageFile.GetUserStoreForAssembly();
        }

        public String DefaultFilename
        {
            get { return DEFAULT_FILENAME; }
        }

        internal void SaveConfig(DownloadConfig config, String path)
        {
            using (var streamWriter = new StreamWriter(path,false))
                Save(streamWriter,config);
        }

        internal void SaveToDefaultConfig(DownloadConfig defaulgConfig)
        {
            using (var userStorageFileStream = new IsolatedStorageFileStream(DEFAULT_FILENAME, FileMode.Create, _userStorage))
                using (var userStorageStreamWriter = new StreamWriter(userStorageFileStream))
                    Save(userStorageStreamWriter, defaulgConfig);
        }

        internal DownloadConfig ReadConfig(String path)
        {
            using (var streamReader = new StreamReader(path))
                return Read(streamReader);
        }

        internal DownloadConfig ReadDefaultConfig()
        {
            using (var userStorageFileStream = new IsolatedStorageFileStream(DEFAULT_FILENAME, FileMode.OpenOrCreate, _userStorage))
                using (var userStorageStreamReader = new StreamReader(userStorageFileStream))
                    return Read(userStorageStreamReader);
        }

        private void Save(StreamWriter streamWriter, DownloadConfig config)
        {
            using (streamWriter)
                _xmlSerializer.Serialize(streamWriter,config);
        }

        private DownloadConfig Read(StreamReader streamReader)
        {
            using (streamReader)
                return (DownloadConfig) _xmlSerializer.Deserialize(streamReader);
        }
     
    }
}
