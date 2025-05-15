using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using AI.Gemini.Interfaces;

namespace AI.Gemini.Services
{
    public sealed class GeminiHttpClient :
        IGeminiHttpClient, IDisposable
    {
        private readonly HttpClient _httpClient;
        private bool _disposed;

        private const string JSON_MEDIA_TYPE = "application/json";

        // Can be empty or accept HttpClientFactory in more complex scenarios.
        public GeminiHttpClient()
        {            
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(JSON_MEDIA_TYPE));
            // _httpClient.DefaultRequestHeaders.Add("User-Agent", "UnityGameClient/1.0"); // Example

            // You can set Timeout here if you don't use CancellationTokenSource externally.
            // _httpClient.Timeout = TimeSpan.FromSeconds(REQUEST_TIMEOUT_SECONDS);

            _disposed = false;
        }

        public async Task<(HttpResponseMessage response, string jsonResponse)> PostAndReadAsync(
            string uri, HttpContent content, CancellationToken token)
        {
            HttpResponseMessage response = await _httpClient.PostAsync(uri, content, token);
            string jsonResponse = await response.Content.ReadAsStringAsync();
            return (response, jsonResponse);
        }

        public void Dispose()
        {
            if (_disposed) return;

            _httpClient?.Dispose();
            _disposed = true;

            GC.SuppressFinalize(this);
        }
    }
}