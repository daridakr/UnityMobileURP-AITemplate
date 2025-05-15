using System.Collections.Generic;
using Newtonsoft.Json;

namespace AI.Gemini.DTO
{
    public sealed class PromptFeedbackDTO
    {
        [JsonProperty("safetyRatings")] public List<SafetyRatingDTO> SafetyRatings { get; set; }
    }
}