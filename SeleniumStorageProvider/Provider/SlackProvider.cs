using System;
using System.IO;
using System.Net;
using System.Net.Http;
using SeleniumStorageProvider.Enum;
using SeleniumStorageProvider.Interfaces;

namespace SeleniumStorageProvider.Provider
{
    public class SlackProvider : IStorageProvider
    {
        public void Save(byte[] screenshot, string pageSource, string type, string message, string methodName, EventType eventType)
        {
            HttpContent tokenContent = new StringContent("Token");
            HttpContent titleContent = new StringContent(methodName);
            HttpContent messageContent = new StringContent(string.Format("Message: \r\n{0}", message));
            HttpContent channelContent = new StringContent("ChannelId");
            HttpContent bytesContent = new ByteArrayContent(screenshot);

            using (var client = new HttpClient())
            using (var formData = new MultipartFormDataContent())
            {
                formData.Add(tokenContent, "token");
                formData.Add(titleContent, "title");
                formData.Add(messageContent, "initial_comment");
                formData.Add(titleContent, "initial_comment");
                formData.Add(bytesContent, "file", "screenshot.jpg");
                formData.Add(channelContent, "channels");

                
                var response = client.PostAsync("https://slack.com/api/files.upload", formData).Result;
                if (!response.IsSuccessStatusCode)
                {
                    var a = response.Content.ReadAsStreamAsync().Result;
                }

                var b = response.Content.ReadAsStreamAsync().Result;
            }
        }
    }
}
