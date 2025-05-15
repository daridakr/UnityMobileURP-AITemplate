using System.Collections.Generic;
using Newtonsoft.Json;

namespace AI.Gemini.DTO
{
    public sealed class CandidateDTO
    {
        [JsonProperty("content")] public ContentDTO Content { get; set; }
        [JsonProperty("finishReason")] public string FinishReason { get; set; }
        [JsonProperty("index")] public int Index { get; set; }
        [JsonProperty("safetyRatings")] public List<SafetyRatingDTO> SafetyRatings { get; set; }
    }
}