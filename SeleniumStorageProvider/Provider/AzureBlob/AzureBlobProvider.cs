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

namespace SeleniumStorageProvider.Provider.AzureBlob
{
    public class AzureBlobProvider : IStorageProvider
    {
        private CloudStorageAccount CloudStorageAccount { get; set; }

        private string StorageContainer
        {
            get
            {
                string containerName = ConfigurationManager.AppSettings["AzureBlob:StorageContainer"];
                return string.IsNullOrEmpty(containerName) ? "default" : containerName;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureBlobProvider"/> class.
        /// </summary>
        /// <param name="connectionstring">The connectionstring.</param>
        public AzureBlobProvider(string connectionstring) : this(CloudStorageAccount.Parse(connectionstring))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureBlobProvider"/> class.
        /// </summary>
        /// <exception cref="System.Exception">There is in the appsettings no key found with name: StorageConnectionString</exception>
        public AzureBlobProvider(CloudStorageAccount cloudStorageAccount)
        {
            CloudStorageAccount = cloudStorageAccount;
        }

        /// <summary>
        /// Saves the specified screenshot.
        /// </summary>
        /// <param name="screenshot">The screenshot.</param>
        /// <param name="pageSource">The page source.</param>
        /// <param name="url">The URL.</param>
        /// <param name="message">The message.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="eventType">Type of the event.</param>
        /// <exception cref="System.Exception">Error creating the html template</exception>
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

        private static string CreateErrorTemplate(string base64File, string pageSource, string url, string message, string methodName)
        {
            const string htmlTemplateFileName = "ErrorTemplate.html";
            string html = LoadTemplate(Assembly.GetExecutingAssembly(), htmlTemplateFileName);
            var encodedPageSource = HttpUtility.HtmlEncode(pageSource);
            return html.Replace("{url}", url).Replace("{message}", message).Replace("{base64string}", base64File).Replace("{pagesource}", encodedPageSource).Replace("{methodName}", methodName);
        }

        private static string LoadTemplate(Assembly currentAssembly, string resourceName)
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
