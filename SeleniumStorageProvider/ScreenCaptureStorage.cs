using System;
using System.Configuration;
using System.IO;
using Microsoft.Expression.Encoder.ScreenCapture;
using SeleniumStorageProvider.Enum;
using SeleniumStorageProvider.Interfaces;
using SeleniumStorageProvider.Provider.AzureBlob;

namespace SeleniumStorageProvider
{
    public class ScreenCaptureStorage
    {
        private readonly IStorageProvider _storageProvider;
        private ScreenCaptureJob ScreenCaptureJob { get; set; }
        private string OutputScreenCaptureFile { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScreenCaptureStorage"/> class.
        /// </summary>
        public ScreenCaptureStorage(string screenCaptureFolder) : this(new AzureBlobProvider(ConfigurationManager.AppSettings["AzureBlob:StorageConnectionString"]), screenCaptureFolder)
        {
            ScreenCaptureJob = new ScreenCaptureJob();
            if (screenCaptureFolder == null || !Directory.Exists(screenCaptureFolder))
            {
                throw new DirectoryNotFoundException(string.Format("Directory {0} not found", screenCaptureFolder));
            }

            OutputScreenCaptureFile = String.Format("{0}\\ScreenCapture.wmv", screenCaptureFolder);
            ScreenCaptureJob.OutputScreenCaptureFileName = OutputScreenCaptureFile;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScreenCaptureStorage"/> class.
        /// </summary>
        /// <param name="storage">The storage.</param>
        /// <param name="screenCaptureFolder"></param>
        public ScreenCaptureStorage(IStorageProvider storage, string screenCaptureFolder)
        {
            ScreenCaptureJob = new ScreenCaptureJob();
            _storageProvider = storage;
            if (screenCaptureFolder == null || !Directory.Exists(screenCaptureFolder))
            {
                throw new DirectoryNotFoundException(string.Format("Directory {0} not found", screenCaptureFolder));
            }
            
            OutputScreenCaptureFile = String.Format("{0}\\ScreenCapture.wmv", screenCaptureFolder);
            ScreenCaptureJob.OutputScreenCaptureFileName = OutputScreenCaptureFile;
        }

        public void Start()
        {
            ScreenCaptureJob.Start();
        }

        public void Stop()
        {
            ScreenCaptureJob.Stop();
        }

        /// <summary>
        /// Saves the specified page source.
        /// </summary>
        /// <param name="pageSource">The page source.</param>
        /// <param name="url">The URL.</param>
        /// <param name="message">The message.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="type">The type.</param>
        public void Save(string pageSource, string url, string message, string methodName, EventType type)
        {
            this.Stop();
            _storageProvider.Save(OutputScreenCaptureFile, pageSource, url, message, methodName, type);
        }
    }
}
