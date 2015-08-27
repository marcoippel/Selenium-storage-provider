using System;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using SeleniumStorageProvider.Enum;
using SeleniumStorageProvider.Interfaces;

namespace SeleniumStorageProvider.Provider.Slack
{
    public class SlackProvider : IStorageProvider
    {
        private readonly HttpClient _httpClient;
        private string ChannelId { get; set; }
        private const string SlackUrl = "https://slack.com/api";

        private string Token {
            get
            {
                string token = ConfigurationManager.AppSettings["Slack:Token"];
                if (string.IsNullOrEmpty(token))
                {
                    throw new Exception("No appsetting Slack:Token found");
                }
                return token;
            }
        }
        
        private string Channel
        {
            get
            {
                string channel = ConfigurationManager.AppSettings["Slack:Channel"];
                if (string.IsNullOrEmpty(channel))
                {
                    throw new Exception("No appsetting Slack:Channel found");
                }
                return channel.StartsWith("#") ? channel.Replace("#", string.Empty) : channel;
            }
        }

        public SlackProvider() : this(new HttpClient())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SlackProvider"/> class.
        /// </summary>
        public SlackProvider(HttpClient httpClient)
        {
            _httpClient = httpClient;
            ChannelId = GetSlackChannelId();
        }

        /// <summary>
        /// Gets the slack channel identifier.
        /// </summary>
        /// <returns>The id of the slack channel</returns>
        private string GetSlackChannelId()
        {
            Task<string> response = _httpClient.GetStringAsync(string.Format("{0}/channels.list?token={1}", SlackUrl, Token));
            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();

            SlackChannelModel slackChannels = javaScriptSerializer.Deserialize<SlackChannelModel>(response.Result);

            Channel slackChannel = slackChannels.channels.SingleOrDefault(x => x.name == Channel);
            if (slackChannel != null)
            {
                return slackChannel.id;
            }
           

            return null;
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
        public void Save(byte[] screenshot, string pageSource, string url, string message, string methodName, EventType eventType)
        {
            HttpContent tokenContent = new StringContent(Token);
            HttpContent titleContent = new StringContent(methodName);
            HttpContent messageContent = new StringContent(string.Format("Message: {0}\r\n Url: {1}\r\n", message, url));
            HttpContent channelContent = new StringContent(ChannelId);
            HttpContent bytesContent = new ByteArrayContent(screenshot);

            //using (var client = new HttpClient())
            var formData = new MultipartFormDataContent
            {
                {tokenContent, "token"},
                {titleContent, "title"},
                {messageContent, "initial_comment"},
                {titleContent, "initial_comment"},
                {bytesContent, "file", "screenshot.jpg"},
                {channelContent, "channels"}
            };
            
            Task<string> respone = _httpClient.PostAsync(string.Format("{0}/files.upload", SlackUrl), formData).Result.Content.ReadAsStringAsync();

            formData.Dispose();
            _httpClient.Dispose();
        }
    }
}
