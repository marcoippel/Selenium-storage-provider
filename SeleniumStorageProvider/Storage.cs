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
        private IStorageProvider _storageProvider;
        public Storage() : this(new AzureBlob())
        {
            
        }

        public Storage(IStorageProvider storage)
        {
            _storageProvider = storage;
        }

        public void Save(string base64File, string pageSource, string url, string message, string methodName, EventType type)
        {
            //
            // Create html file
            var htmlFile = CreateErrorTemplate(base64File, pageSource, url, message, methodName);
            if (string.IsNullOrEmpty(htmlFile))
            {
                throw new Exception("Error creating the html template");

            }
            byte[] byteArray = Encoding.ASCII.GetBytes(htmlFile);

            string fileName = string.Format("{0}.html", DateTime.Now.ToString("hh-mm-ss"));


            // Save the html file
            _storageProvider.Save(byteArray, methodName.ToLowerInvariant(), fileName, type);
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
