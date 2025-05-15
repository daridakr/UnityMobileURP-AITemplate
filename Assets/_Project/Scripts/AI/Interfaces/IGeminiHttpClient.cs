using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace AI.Gemini.Interfaces
{
    /// <summary>
    /// Interface for a client responsible for making the actual HTTP request to Gemini API.
    /// </summary>
    public interface IGeminiHttpClient
    {
        public Task<(HttpResponseMessage response, string jsonResponse)> PostAndReadAsync(
            string uri, HttpContent content, CancellationToken token);
    }
}