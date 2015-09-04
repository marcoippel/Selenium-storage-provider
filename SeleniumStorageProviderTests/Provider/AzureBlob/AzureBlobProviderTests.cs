using System;
using System.Fakes;
using System.Web.Fakes;
using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
        private string Url { get; set; }
        private EmbeddedResource EmbeddedResource { get; set; }
        private string Message { get; set; }
        private string MethodName { get; set; }
        private byte[] Screenshot { get; set; }

        [TestInitialize]
        public void Setup()
        {
            EmbeddedResource = new EmbeddedResource();
            MethodName = "AzureBlobProvider_can_post";
            Url = "http://www.unittest.nl";
            Screenshot = EmbeddedResource.GetAsByteArray("SeleniumStorageProviderTests.Response.ScreenShot.PNG");
            Message = "unittest message";
        }
        
        [TestMethod()]
        public void SaveTest()
        {
            using (ShimsContext.Create())
            {
                var shimCloudBlobContainer = ShimCloudBlobContainer;
                ShimCloudStorageAccount.ParseString = s => new ShimCloudStorageAccount
                {
                    CreateCloudBlobClient = () => ShimCloudBlobClient(shimCloudBlobContainer)
                }; 

                ShimHttpContext.CurrentGet = () => new ShimHttpContext();
                ShimHttpContext.AllInstances.RequestGet = (o) => ShimHttpRequest;
                ShimHttpUtility.HtmlEncodeString = s => s;
                ShimDateTime.NowGet = () => new DateTime(2015, 12, 12, 12, 10, 15);

                ShimCloudBlockBlob shimCloudBlockBlob = new ShimCloudBlockBlob
                {
                    PropertiesGet = () => new BlobProperties(),
                    UploadFromByteArrayAsyncByteArrayInt32Int32 = (bytes, i, arg3) =>
                    {
                        Assert.AreEqual(250223, bytes.Length);
                        return null;
                    }
                };

                shimCloudBlobContainer.GetBlockBlobReferenceString = s =>
                {
                    Assert.AreEqual("default/2015/12/12/AzureBlobProvider_can_post/error/12-10-15.html", s);
                    return shimCloudBlockBlob;
                };

                AzureBlobProvider azureBlobProvider = new AzureBlobProvider("fakeconnectionstring");
                azureBlobProvider.Save(Screenshot, "<html></html>", Url, Message, MethodName, EventType.Error);
            }
            
        }

        private static ShimHttpRequest ShimHttpRequest
        {
            get
            {
                ShimHttpRequest request = new ShimHttpRequest
                {
                    UrlGet = () => new StubUri("http://p.nu.nl")
                };
                return request;
            }
        }
        
        private static ShimCloudBlobContainer ShimCloudBlobContainer
        {
            get
            {
                ShimCloudBlobContainer shimCloudBlobContainer = new ShimCloudBlobContainer
                {
                    CreateIfNotExistsBlobRequestOptionsOperationContext = (options, operationContext) => true
                };
                return shimCloudBlobContainer;
            }
        }

        private static ShimCloudBlobClient ShimCloudBlobClient(ShimCloudBlobContainer shimCloudBlobContainer)
        {
            ShimCloudBlobClient shimCloudBlobClient = new ShimCloudBlobClient
            {
                GetContainerReferenceString = s => shimCloudBlobContainer
            };
            return shimCloudBlobClient;
        }
        
    }
}
