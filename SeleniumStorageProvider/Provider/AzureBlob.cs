using System;
using System.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using SeleniumStorageProvider.Interfaces;

namespace SeleniumStorageProvider.Provider
{
    public class AzureBlob : IStorageProvider
    {
        private CloudStorageAccount CloudStorageAccount { get; set; }

        private string StorageContainer
        {
            get
            {
                string containerName = ConfigurationManager.AppSettings["StorageContainer"];
                return string.IsNullOrEmpty(containerName) ? "default" : containerName;
            }
        }

        public AzureBlob()
        {
            string connectionString = ConfigurationManager.AppSettings["StorageConnectionString"];
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new Exception("There is in the appsettings no key found with name: StorageConnectionString");
            }

            CloudStorageAccount = CloudStorageAccount.Parse(connectionString);
        }

        public void Save(byte[] file, string fileName)
        {
            var dateTime = DateTime.Now;
            CloudBlobClient blobClient = CloudStorageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("seleniumscreenshots");
            container.CreateIfNotExists();

            CloudBlockBlob blockBlob = container.GetBlockBlobReference(string.Format("{0}/{1}/{2}/{3}/{4}", StorageContainer, dateTime.Year, dateTime.Month, dateTime.Day, fileName));
            blockBlob.Properties.ContentType = "text/html";
            blockBlob.UploadFromByteArrayAsync(file, 0, file.Length);
        }
    }
}
