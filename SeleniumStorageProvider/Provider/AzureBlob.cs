using System;
using System.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using SeleniumStorageProvider.Enum;
using SeleniumStorageProvider.Interfaces;

namespace SeleniumStorageProvider.Provider
{
    public class AzureBlob : IStorageProvider
    {
        private CloudStorageAccount CloudStorageAccount { get; set; }

        /// <summary>
        /// Gets the storage container.
        /// </summary>
        /// <value>
        /// The storage container.
        /// </value>
        private string StorageContainer
        {
            get
            {
                string containerName = ConfigurationManager.AppSettings["StorageContainer"];
                return string.IsNullOrEmpty(containerName) ? "default" : containerName;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureBlob"/> class.
        /// </summary>
        /// <exception cref="System.Exception">There is in the appsettings no key found with name: StorageConnectionString</exception>
        public AzureBlob()
        {
            string connectionString = ConfigurationManager.AppSettings["StorageConnectionString"];
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new Exception("There is in the appsettings no key found with name: StorageConnectionString");
            }

            CloudStorageAccount = CloudStorageAccount.Parse(connectionString);
        }

        /// <summary>
        /// Saves the specified file.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="type">The type.</param>
        public void Save(byte[] file,string methodName, string fileName, EventType type)
        {
            var dateTime = DateTime.Now;
            CloudBlobClient blobClient = CloudStorageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("seleniumscreenshots");
            container.CreateIfNotExists();

            string blobFileName = type == EventType.Info ? string.Format("{0}/{1}/{2}/{3}/info/{4}/{5}", StorageContainer, dateTime.Year, dateTime.Month, dateTime.Day, methodName, fileName) : string.Format("{0}/{1}/{2}/{3}/error/{4}/{5}", StorageContainer, dateTime.Year, dateTime.Month, dateTime.Day, methodName, fileName);

            CloudBlockBlob blockBlob = container.GetBlockBlobReference(blobFileName);
            blockBlob.Properties.ContentType = "text/html";
            blockBlob.UploadFromByteArray(file, 0, file.Length);
        }
    }
}
