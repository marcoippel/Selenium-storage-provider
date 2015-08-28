using System.Net.Http;
using System.Threading.Tasks;

namespace SeleniumStorageProvider.Wrappers
{
    public interface IHttpClientWrapper
    {
        string GetStringAsync(string uri);
        HttpResponseMessage PostAsync(string requestUri, HttpContent content);
        void Dispose();
    }
}