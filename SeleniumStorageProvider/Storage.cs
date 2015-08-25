using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using SeleniumStorageProvider.Enum;
using SeleniumStorageProvider.Interfaces;
using SeleniumStorageProvider.Provider;

namespace SeleniumStorageProvider
{
    public class Storage
    {
        private readonly IStorageProvider _storageProvider;
        public Storage() : this(new AzureBlobProvider())
        {
            
        }

        public Storage(IStorageProvider storage)
        {
            _storageProvider = storage;
        }

        public void Save(byte[] screenshot, string pageSource, string url, string message, string methodName, EventType type)
        {
            _storageProvider.Save(screenshot, pageSource, url, message, methodName, type);
        }
    }
}
