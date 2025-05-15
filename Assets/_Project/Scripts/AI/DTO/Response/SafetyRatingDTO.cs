using Newtonsoft.Json;

namespace AI.Gemini.DTO
{
    public sealed class SafetyRatingDTO
    {
        [JsonProperty("category")] public string Category { get; set; }
        [JsonProperty("probability")] public string Probability { get; set; } // Usually returned as a string (e.g., "NEGLIGIBLE").
    }
}