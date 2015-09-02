using System;
using System.Fakes;
using System.Security.Policy;
using System.Threading.Tasks;
using System.Web;
using System.Web.Fakes;
using System.Web.Security.Fakes;
using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Blob.Fakes;
using Microsoft.WindowsAzure.Storage.Fakes;
using SeleniumStorageProvider.Enum;
using SeleniumStorageProvider.Provider.AzureBlob;
using SeleniumStorageProviderTests.Business;

namespace SeleniumStorageProviderTests.Provider.AzureBlob
{
    [TestClass()]
    public class AzureBlobProviderTests
    {
        private byte[] _screenshot;
        private string _url;
        private EmbeddedResource _embeddedResource;
        private string _message;
        private string _methodName;

        [TestInitialize]
        public void Setup()
        {
            _embeddedResource = new EmbeddedResource();
            _methodName = "Slackprovider_can_post";
            _url = "http://www.unittest.nl";
            _screenshot = _embeddedResource.Get();
            _message = "unittest message";
        }

        

        [TestMethod()]
        public void SaveTest()
        {
            using (ShimsContext.Create())
            {
                ShimCloudStorageAccount shimCloudStorageAccount = new ShimCloudStorageAccount();
                ShimCloudStorageAccount.ParseString = s => { return shimCloudStorageAccount; };

                ShimCloudBlobClient shimCloudBlobClient = new ShimCloudBlobClient();
                ShimCloudBlobContainer shimCloudBlobContainer = new ShimCloudBlobContainer();
                ShimCloudBlockBlob shimCloudBlockBlob = new ShimCloudBlockBlob();

                shimCloudBlobClient.GetContainerReferenceString = s => { return shimCloudBlobContainer; };
                shimCloudStorageAccount.CreateCloudBlobClient = () => { return shimCloudBlobClient; };
                shimCloudBlobContainer.CreateIfNotExistsBlobRequestOptionsOperationContext = (options, operationContext) => { return true; };
                shimCloudBlobContainer.GetBlockBlobReferenceString = s =>
                {
                    Assert.AreEqual("default/2015/12/12/all/error/12-10-15.html", s);
                    return shimCloudBlockBlob;
                };
                
                shimCloudBlockBlob.PropertiesGet = () => { return new BlobProperties(); };
                shimCloudBlockBlob.UploadFromByteArrayAsyncByteArrayInt32Int32 = (bytes, i, arg3) => { return null; };

                ShimHttpRequest request = new ShimHttpRequest();
                ShimHttpContext context = new ShimHttpContext();
                ShimHttpContext.CurrentGet = () => { return context; };
                ShimHttpContext.AllInstances.RequestGet = (o) => { return request; };
                ShimHttpUtility.HtmlEncodeString = s => { return s; };
                ShimDateTime.NowGet = () => { return new DateTime(2015, 12, 12, 12, 10, 15); };

                request.UrlGet = () => { return new StubUri("http://p.nu.nl"); };

               // HttpContext.Current.Request.Url.Host;

                AzureBlobProvider azureBlobProvider = new AzureBlobProvider("fakeconnectionstring");
                azureBlobProvider.Save(_screenshot, "<html></html>", _url, _message, _methodName, EventType.Error);
            }
            
        }
    }
}
