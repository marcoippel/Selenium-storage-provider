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
        /// <exception cref="System.Exception">There is in the appsettings no key found with name: StorageConnectionString</exception>
        public AzureBlobProvider(string connectionString)
        {
            //string connectionString = ConfigurationManager.AppSettings["AzureBlob:StorageConnectionString"];
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new Exception("There is in the appsettings no key found with name: AzureBlob:StorageConnectionString");
            }

            CloudStorageAccount = CloudStorageAccount.Parse(connectionString);
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
            var htmlFile = CreateScreenShotErrorTemplate(Convert.ToBase64String(screenshot), pageSource, url, message, methodName);
            if (string.IsNullOrEmpty(htmlFile))
            {
                throw new Exception("Error creating the html template");
            }

            byte[] file = Encoding.ASCII.GetBytes(htmlFile);

            string fileName = string.Format("{0}.html", DateTime.Now.ToString("HH-mm-ss"));

            DateTime dateTime = DateTime.Now;
            string eventTypeName = eventType == EventType.Info ? "info" : "error";
            string blobFileName = string.Format("{0}/{1}/{2}/{3}{4}/{5}/{6}/{7}", StorageContainer, dateTime.Year, dateTime.Month, dateTime.Day, GetEnvironmentName(url), methodName, eventTypeName, fileName);

            CloudBlobContainer container = CreateCloudBlobContainer("seleniumscreenshots");
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(blobFileName);
            blockBlob.Properties.ContentType = "text/html";
            blockBlob.UploadFromByteArrayAsync(file, 0, file.Length);
        }

        /// <summary>
        /// Saves the specified screen recording file.
        /// </summary>
        /// <param name="screenRecordingFile">The screen recording file.</param>
        /// <param name="pageSource">The page source.</param>
        /// <param name="url">The URL.</param>
        /// <param name="message">The message.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="eventType">Type of the event.</param>
        /// <exception cref="Exception">Error creating the html template</exception>
        public void Save(string screenRecordingFile, string pageSource, string url, string message, string methodName, EventType eventType)
        {
            if (File.Exists(screenRecordingFile))
            {
                DateTime dateTime = DateTime.Now;
                string eventTypeName = eventType == EventType.Info ? "info" : "error";
                string fileName = Path.GetFileName(screenRecordingFile);
                string blobFileName = string.Format("{0}/{1}/{2}/{3}{4}/{5}/{6}/{7}", StorageContainer, dateTime.Year, dateTime.Month, dateTime.Day, GetEnvironmentName(url), methodName, eventTypeName, fileName);

                CloudBlobContainer container = CreateCloudBlobContainer("seleniumscreencaptures");
                CloudBlockBlob blockBlob = container.GetBlockBlobReference(blobFileName);
                using (var fileStream = File.OpenRead(screenRecordingFile))
                {
                    blockBlob.UploadFromStream(fileStream);
                }

                File.Delete(screenRecordingFile);

                var htmlFile = CreateScreenCaptureErrorTemplate(GenerateEmbedVideoCode(blobFileName), pageSource, url, message, methodName);
                if (string.IsNullOrEmpty(htmlFile))
                {
                    throw new Exception("Error creating the html template");
                }
            }
        }

        private string GenerateEmbedVideoCode(string blobFileName)
        {
            const string htmlTemplateFileName = "EmbedVideo.html";
            string html = LoadTemplate(Assembly.GetExecutingAssembly(), htmlTemplateFileName);

            return html.Replace("{embeddedvideo}", blobFileName);
        }

        private CloudBlobContainer CreateCloudBlobContainer(string containerName)
        {
            CloudBlobClient blobClient = CloudStorageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(containerName);
            container.CreateIfNotExists();
            return container;
        }

        private string GetEnvironmentName(string url)
        {
            Uri uri = new Uri(url);

            string host = uri.Host;
            return !string.IsNullOrEmpty(ConfigurationManager.AppSettings[host]) ? string.Format("/{0}", ConfigurationManager.AppSettings[host]) : string.Empty;
        }

        private static string CreateScreenShotErrorTemplate(string base64File, string pageSource, string url, string message, string methodName)
        {
            const string htmlTemplateFileName = "ScreenShotErrorTemplate.html";
            string html = LoadTemplate(Assembly.GetExecutingAssembly(), htmlTemplateFileName);
            var encodedPageSource = HttpUtility.HtmlEncode(pageSource);
            return html.Replace("{url}", url).Replace("{message}", message).Replace("{base64string}", base64File).Replace("{pagesource}", encodedPageSource).Replace("{methodName}", methodName);
        }

        private string CreateScreenCaptureErrorTemplate(string embeddedVideo, string pageSource, string url, string message, string methodName)
        {
            const string htmlTemplateFileName = "ScreenCaptureErrorTemplate.html";
            string html = LoadTemplate(Assembly.GetExecutingAssembly(), htmlTemplateFileName);
            var encodedPageSource = HttpUtility.HtmlEncode(pageSource);
            return html.Replace("{url}", url).Replace("{message}", message).Replace("{embeddedvideo}", embeddedVideo).Replace("{pagesource}", encodedPageSource).Replace("{methodName}", methodName);
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
