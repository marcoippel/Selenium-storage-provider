using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using SeleniumStorageProvider.Enum;
using SeleniumStorageProvider.Interfaces;

namespace SeleniumStorageProvider.Provider
{
    public class AzureBlobProvider : IStorageProvider
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

        public AzureBlobProvider()
        {
            string connectionString = ConfigurationManager.AppSettings["StorageConnectionString"];
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new Exception("There is in the appsettings no key found with name: StorageConnectionString");
            }

            CloudStorageAccount = CloudStorageAccount.Parse(connectionString);
        }

        public void Save(byte[] screenshot, string pageSource, string url, string message, string methodName, EventType eventType)
        {
            var htmlFile = CreateErrorTemplate(Convert.ToBase64String(screenshot), pageSource, url, message, methodName);
            if (string.IsNullOrEmpty(htmlFile))
            {
                throw new Exception("Error creating the html template");
            }

            byte[] file = Encoding.ASCII.GetBytes(htmlFile);

            string fileName = string.Format("{0}.html", DateTime.Now.ToString("HH-mm-ss"));

            var dateTime = DateTime.Now;
            CloudBlobClient blobClient = CloudStorageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("seleniumscreenshots");
            container.CreateIfNotExists();

            string blobFileName = eventType == EventType.Info ? string.Format("{0}/{1}/{2}/{3}/info/{4}", StorageContainer, dateTime.Year, dateTime.Month, dateTime.Day, fileName) : string.Format("{0}/{1}/{2}/{3}/error/{4}", StorageContainer, dateTime.Year, dateTime.Month, dateTime.Day, fileName);

            CloudBlockBlob blockBlob = container.GetBlockBlobReference(blobFileName);
            blockBlob.Properties.ContentType = "text/html";
            blockBlob.UploadFromByteArrayAsync(file, 0, file.Length);
        }

        private string CreateErrorTemplate(string base64File, string pageSource, string url, string message, string methodName)
        {
            const string htmlTemplateFileName = "ErrorTemplate.html";
            string html = LoadTemplate(Assembly.GetExecutingAssembly(), htmlTemplateFileName);
            var encodedPageSource = HttpUtility.HtmlEncode(pageSource);
            return html.Replace("{url}", url).Replace("{message}", message).Replace("{base64string}", base64File).Replace("{pagesource}", encodedPageSource).Replace("{methodName}", methodName);
        }

        private string LoadTemplate(Assembly currentAssembly, string resourceName)
        {
            if (currentAssembly == null)
            {
                throw new ArgumentNullException("currentAssembly");
            }

            var resource = currentAssembly.GetManifestResourceNames().FirstOrDefault(r => r.EndsWith(resourceName));
            if (resource == null)
            {
                throw new NullReferenceException(string.Format("Resource ending with name {0} not found", resourceName));
            }

            Stream htmlTemplateFile = currentAssembly.GetManifestResourceStream(resource);
            if (htmlTemplateFile == null)
            {
                throw new NullReferenceException(
                    string.Format("Profile xml ending with name {0} not loaded from assembly", resourceName));
            }

            StreamReader reader = new StreamReader(htmlTemplateFile);
            string html = reader.ReadToEnd();

            return html;
        }

        
    }
}
