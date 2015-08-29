using System;
using System.Configuration;
using SeleniumStorageProvider.Enum;
using SeleniumStorageProvider.Interfaces;
using SeleniumStorageProvider.Provider.AzureBlob;

namespace SeleniumStorageProvider
{
    public class Storage
    {
        private readonly IStorageProvider _storageProvider;

        private static string Connectionstring
        {
            get
            {
                string connectionString = ConfigurationManager.AppSettings["AzureBlob:StorageConnectionString"];
                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new Exception("There is in the appsettings no key found with name: StorageConnectionString");
                }

                return connectionString;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Storage"/> class.
        /// </summary>
        public Storage() : this(new AzureBlobProvider(Connectionstring))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Storage"/> class.
        /// </summary>
        /// <param name="storage">The storage.</param>
        public Storage(IStorageProvider storage)
        {
            _storageProvider = storage;
        }

        /// <summary>
        /// Saves the specified screenshot.
        /// </summary>
        /// <param name="screenshot">The screenshot.</param>
        /// <param name="pageSource">The page source.</param>
        /// <param name="url">The URL.</param>
        /// <param name="message">The message.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="type">The type.</param>
        public void Save(byte[] screenshot, string pageSource, string url, string message, string methodName, EventType type)
        {
            _storageProvider.Save(screenshot, pageSource, url, message, methodName, type);
        }
    }
}
