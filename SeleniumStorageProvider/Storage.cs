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
        /// <summary>
        /// The _storage provider
        /// </summary>
        private readonly IStorageProvider _storageProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="Storage"/> class.
        /// </summary>
        public Storage() : this(new AzureBlob())
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
        /// Saves the specified base64 file.
        /// </summary>
        /// <param name="base64File">The base64 file.</param>
        /// <param name="pageSource">The page source.</param>
        /// <param name="url">The URL.</param>
        /// <param name="message">The message.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="type">The type.</param>
        /// <exception cref="System.Exception">Error creating the html template</exception>
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

            string fileName = string.Format("{0}.html", DateTime.Now.ToString("HH-mm-ss"));


            // Save the html file
            _storageProvider.Save(byteArray, methodName.ToLowerInvariant(), fileName, type);
        }

        /// <summary>
        /// Creates the error template.
        /// </summary>
        /// <param name="base64File">The base64 file.</param>
        /// <param name="pageSource">The page source.</param>
        /// <param name="url">The URL.</param>
        /// <param name="message">The message.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <returns></returns>
        private string CreateErrorTemplate(string base64File, string pageSource, string url, string message, string methodName)
        {
            const string htmlTemplateFileName = "ErrorTemplate.html";
            string html = LoadTemplate(Assembly.GetExecutingAssembly(), htmlTemplateFileName);
            var encodedPageSource = HttpUtility.HtmlEncode(pageSource);
            return html.Replace("{url}", url).Replace("{message}", message).Replace("{base64string}", base64File).Replace("{pagesource}", encodedPageSource).Replace("{methodName}", methodName);
        }

        /// <summary>
        /// Loads the template.
        /// </summary>
        /// <param name="currentAssembly">The current assembly.</param>
        /// <param name="resourceName">Name of the resource.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">currentAssembly</exception>
        /// <exception cref="System.NullReferenceException">
        /// </exception>
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
