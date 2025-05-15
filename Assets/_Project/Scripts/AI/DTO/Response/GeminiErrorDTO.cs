using Newtonsoft.Json;

namespace AI.Gemini.DTO
{
     // Structure for errors returned in JSON response.
    public sealed class GeminiErrorDTO
    {
        [JsonProperty("code")] public int Code { get; set; }
        [JsonProperty("message")] public string Message { get; set; }
        [JsonProperty("status")] public string Status { get; set; }
    }
}