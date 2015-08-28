using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SeleniumStorageProvider.Wrappers
{
    public class HttpClientWrapper : IHttpClientWrapper
    {
        private readonly HttpClient _client;

        public HttpClientWrapper() : this(new HttpClient())
        {
        }

        public HttpClientWrapper(HttpClient client)
        {
            _client = client;
        }

        public string GetStringAsync(string uri)
        {
            return _client.GetStringAsync(uri).Result;
        }

        public HttpResponseMessage PostAsync(string requestUri, HttpContent content)
        {
            return _client.PostAsync(requestUri, content).Result;
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}
