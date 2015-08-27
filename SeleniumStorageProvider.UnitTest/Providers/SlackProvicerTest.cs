using System;
using System.Net.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RichardSzalay.MockHttp;
using SeleniumStorageProvider.Enum;
using SeleniumStorageProvider.Provider.Slack;
using SeleniumStorageProvider.UnitTest.Business;

namespace SeleniumStorageProvider.UnitTest.Providers
{
    [TestClass]
    public class SlackProvicerTest
    {
        [TestMethod]
        public void Get_ChannelId()
        {
            EmbeddedResource embeddedResource = new EmbeddedResource();
            string response = embeddedResource.Get("SeleniumStorageProvider.UnitTest.Response.ChannelList.json");

            MockHttpMessageHandler mockHttp = new MockHttpMessageHandler();
            var httpClient = new HttpClient(mockHttp);

            mockHttp.When("https://slack.com/api/channels.list?token=123456").Respond("application/json", response);
            mockHttp.When("https://slack.com/api/files.upload").Respond("application/json", "success message");

            SlackProvider slackProvider = new SlackProvider(httpClient);
            slackProvider.Save(embeddedResource.Get(), "<html>Pagesource</html>", "http://www.unittest.nl", "unittest message", "Slackprovider_can_post", EventType.Error);
            
        }
    }
}
