using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AI.Gemini.Configuration;
using AI.Gemini.DTO;
using AI.Gemini.Interfaces;
using Core.Secrets;
using Newtonsoft.Json;
using UnityEngine;

namespace AI.Gemini.Services
{
    public sealed class GeminiService :
        IGeminiService
    {
        private readonly GeminiConfigSO _config;
        private readonly ISecretKeyProvider _secretProvider;
        private readonly IGeminiHttpClient _geminiHttpClient;

        private const int REQUEST_TIMEOUT_SECONDS = 30;
        private const string LOG_PREFIX = "[GeminiService] ";
        private const string JSON_MEDIA_TYPE = "application/json";

        public GeminiService(
            GeminiConfigSO config,
            ISecretKeyProvider secretProvider,
            IGeminiHttpClient geminiHttpClient)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _secretProvider = secretProvider ?? throw new ArgumentNullException(nameof(secretProvider));
            _geminiHttpClient = geminiHttpClient ?? throw new ArgumentNullException(nameof(geminiHttpClient));
        }

        public async Task<GeminiResult> SendPromptAsync(string prompt)
        {
            var (keySuccess, apiKey) = await TryGetApiKeyAsync();
            if (!keySuccess) return GeminiResult.Failure(apiKey);

            string validationError = ValidateRequest(prompt);
            if (validationError != null) return GeminiResult.Failure(validationError);

            (string requestUri, HttpContent requestContent) = PrepareHttpRequest(prompt, apiKey);

            using (requestContent)
            using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(REQUEST_TIMEOUT_SECONDS)))
                return await ExecuteAndProcessRequestAsync(requestUri, requestContent, cts.Token);
        }

        private async Task<(bool Success, string ApiKeyOrError)> TryGetApiKeyAsync()
        {
            var (keySuccess, apiKey) = await _secretProvider.TryGetSecretAsync(SecretsParams.KEY_GEMINI_API);

            if (!keySuccess || string.IsNullOrEmpty(apiKey))
            {
                string error = $"API Key '{SecretsParams.KEY_GEMINI_API}' could not be retrieved or is invalid.";
                Debug.LogError(LOG_PREFIX + error);
                return (false, error);
            }

            return (true, apiKey);
        }

        private string ValidateRequest(string prompt)
        {
            if (string.IsNullOrEmpty(_config.ApiEndpoint)) return "API Endpoint is not configured.";
            if (string.IsNullOrEmpty(prompt)) return "Prompt cannot be empty.";

            return null;
        }

        private (string uri, HttpContent content) PrepareHttpRequest(string prompt, string apiKey)
        {
            string requestUri = $"{_config.ApiEndpoint}?key={apiKey}";
            GeminiRequestDTO requestDto = CreateTextRequest(prompt);
            string jsonPayload = JsonConvert.SerializeObject(requestDto, Formatting.None,
                new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            StringContent requestContent = new(jsonPayload, Encoding.UTF8, JSON_MEDIA_TYPE);
            return (requestUri, requestContent);
        }

        private async Task<GeminiResult> ExecuteAndProcessRequestAsync(string uri, HttpContent content, CancellationToken token)
        {
            try
            {
                (HttpResponseMessage response, string jsonResponse) =
                await _geminiHttpClient.PostAndReadAsync(uri, content, token);

                if (response.IsSuccessStatusCode)
                return ProcessSuccessfulResponse(jsonResponse);
                else
                return ProcessErrorResponse(response, jsonResponse);
            }
            catch (TaskCanceledException ex) when (ex.CancellationToken == token)
            {
                Debug.LogError($"{LOG_PREFIX}Request timed out after {REQUEST_TIMEOUT_SECONDS}s. Msg: {ex.Message}");
                return GeminiResult.Failure("Request timed out.");
            }
            catch (HttpRequestException httpEx)
            {
                Debug.LogError($"{LOG_PREFIX}Network error: {httpEx.Message}");
                return GeminiResult.Failure($"Network error: {httpEx.Message}");
            }
        }

        private GeminiResult ProcessSuccessfulResponse(string jsonResponse)
        {
            try
            {
                GeminiResponseDTO geminiResponse = JsonConvert.DeserializeObject<GeminiResponseDTO>(jsonResponse);

                if (geminiResponse?.Error != null)
                {
                    string apiError = $"API Error: {geminiResponse.Error.Message}";
                    Debug.LogError($"{LOG_PREFIX}Gemini API Error (JSON): {geminiResponse.Error.Code} - {apiError}. Status: {geminiResponse.Error.Status}");
                    return GeminiResult.Failure(apiError);
                }
                else
                {
                    string responseText = ExtractTextFromResponse(geminiResponse);
                    return responseText != null
                        ? GeminiResult.Success(responseText)
                        : GeminiResult.Failure("Unexpected response format (No text found).");
                }
            }
            catch (JsonException jsonEx)
            {
                Debug.LogError($"{LOG_PREFIX}JSON Error parsing successful response: {jsonEx.Message}");
                return GeminiResult.Failure("JSON parsing error in successful response.");
            }
        }

        private GeminiResult ProcessErrorResponse(HttpResponseMessage response, string jsonResponse)
        {
            string specificError = TryParseErrorFromBody(jsonResponse) ?? response.ReasonPhrase;
            string httpError = $"HTTP Error {(int)response.StatusCode}: {specificError}";
            Debug.LogError($"{LOG_PREFIX}Gemini HTTP Error: {(int)response.StatusCode} - {response.ReasonPhrase}.\nBody: {jsonResponse}");
            return GeminiResult.Failure(httpError);
        }

        private GeminiRequestDTO CreateTextRequest(string textPrompt)
        {
            return new GeminiRequestDTO
            {
                Contents = new List<ContentDTO>
                {
                    new() { Parts = new List<PartDTO> { new() { Text = textPrompt } } }
                    // Role = "user" // Role can be added if needed for chat history etc.
                }
                // You could potentially add GenerationConfig here from _config
                // GenerationConfig = new GenerationConfigDTO { Temperature = _config.Temperature /*, etc. */ }
            };
        }

        private string ExtractTextFromResponse(GeminiResponseDTO response)
        {
            if (response?.Candidates != null && response.Candidates.Count > 0)
            {
                CandidateDTO firstCandidate = response.Candidates[0];

                if (firstCandidate?.Content?.Parts != null && firstCandidate.Content.Parts.Count > 0)
                    return firstCandidate.Content.Parts[0].Text;
            }
            return null; // No text found.
        }

        private string TryParseErrorFromBody(string jsonBody)
        {
             if (string.IsNullOrEmpty(jsonBody)) return null;
             try
             {
                 var errorResponse = JsonConvert.DeserializeObject<GeminiResponseDTO>(jsonBody);
                 return errorResponse?.Error?.Message;
             }
             catch
             {
                 return null; // Ignore parsing errors, return null
             }
        }
    }
}