using Newtonsoft.Json;

namespace AI.Gemini.DTO
{
    public sealed class PartDTO
    {
        [JsonProperty("text")] public string Text { get; set; }

        // You can add other type of "parts" like image etc.
    }
}