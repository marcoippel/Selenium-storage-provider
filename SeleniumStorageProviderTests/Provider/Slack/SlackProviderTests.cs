using System;
using System.Net;
using System.Net.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SeleniumStorageProvider.Enum;
using SeleniumStorageProvider.Provider.Slack;
using SeleniumStorageProvider.Wrappers;
using SeleniumStorageProviderTests.Business;

namespace SeleniumStorageProviderTests.Provider.Slack
{
    [TestClass]
    public class SlackProviderTests
    {
        private EmbeddedResource EmbeddedResource { get; set; }

        private string MethodName { get; set; }
        private string Message { get; set; }
        private string Url { get; set; }
        private string ChannelId { get; set; }
        private byte[] Screenshot { get; set; }

        [TestInitialize]
        public void Setup()
        {
            EmbeddedResource = new EmbeddedResource();

            MethodName = "Slackprovider_can_post";
            Message = "unittest message";
            Url = "http://www.unittest.nl";
            ChannelId = "DBFGTYU";
            Screenshot = EmbeddedResource.GetAsByteArray("SeleniumStorageProviderTests.Response.ScreenShot.PNG");
        }

        [TestCategory("appveyor")]
        [TestMethod]
        public void Post_Screenshot()
        {
            Console.WriteLine("Start Post_Screenshot");
            string response = EmbeddedResource.GetAsString("SeleniumStorageProviderTests.Response.ChannelList.json");

            Mock<IHttpClientWrapper> httpClientMock = new Mock<IHttpClientWrapper>();
            IHttpClientWrapper httpClient = httpClientMock.Object;

            httpClientMock.Setup(t => t.GetStringAsync("https://slack.com/api/channels.list?token=123456")).Returns(response);
            httpClientMock.Setup(t => t.PostAsync(It.IsAny<string>(), It.IsAny<MultipartFormDataContent>())).Returns(new HttpResponseMessage(HttpStatusCode.OK)).Verifiable("Not called");

            SlackProvider slackProvider = new SlackProvider(httpClient);
            slackProvider.Save(Screenshot, string.Empty, Url, Message, MethodName, EventType.Error);

            httpClientMock.Verify(
                x =>
                    x.PostAsync(It.Is<string>(y => y == "https://slack.com/api/files.upload"),
                        It.IsNotNull<MultipartFormDataContent>()), Times.Once);

            Console.WriteLine("End Post_Screenshot");
        }
    }
}
