using System.Collections.Generic;
using Newtonsoft.Json;

namespace AI.Gemini.DTO
{
    // Request structure for Gemini API (generateContent).
    public sealed class GeminiRequestDTO
    {
        [JsonProperty("contents")] public List<ContentDTO> Contents { get; set; }

        // You can add other request parameters here (safetySettings, generationConfig).
        // [JsonProperty("generationConfig")] public GenerationConfigDTO GenerationConfig { get; set; }
        // [JsonProperty("safetySettings")] public List<SafetySettingDTO> SafetySettings { get; set; }
    }
}