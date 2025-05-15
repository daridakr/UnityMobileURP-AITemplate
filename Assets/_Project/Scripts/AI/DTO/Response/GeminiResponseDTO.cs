using System.Collections.Generic;
using Newtonsoft.Json;

namespace AI.Gemini.DTO
{
    // Response structure from Gemini API (generateContent)
    public sealed class GeminiResponseDTO
    {
        [JsonProperty("candidates")] public List<CandidateDTO> Candidates { get; set; }
        [JsonProperty("promptFeedback")] public PromptFeedbackDTO PromptFeedback { get; set; }
        [JsonProperty("error")] public GeminiErrorDTO Error { get; set; } // To catch an error at the JSON response level (if that's what the API returns).
    }
}