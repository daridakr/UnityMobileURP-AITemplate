#if TEST
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AI.Gemini.Interfaces;

namespace Test.Gemini
{
    public sealed class TimedGeminiHttpClient :
        IGeminiHttpClient
    {
        private readonly IGeminiHttpClient _wrappedClient;

        private const string OPERATION_TIMER_NAME = "Network API Call (Gemini request) Latency";

        public TimedGeminiHttpClient(IGeminiHttpClient wrappedClient) =>
            _wrappedClient = wrappedClient ?? throw new ArgumentNullException(nameof(wrappedClient));

        public async Task<(HttpResponseMessage response, string jsonResponse)> PostAndReadAsync(
            string uri, HttpContent content, CancellationToken token)
        {
            using var timer = OperationTimer.Start(OPERATION_TIMER_NAME);

            return await _wrappedClient.PostAndReadAsync(uri, content, token);
        }
    }
}
#endif