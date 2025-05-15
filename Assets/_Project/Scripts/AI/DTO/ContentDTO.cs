using System.Collections.Generic;
using Newtonsoft.Json;

namespace AI.Gemini.DTO
{
    public sealed class ContentDTO
    {
        [JsonProperty("parts")] public List<PartDTO> Parts { get; set; }

        // You can add role (user/model)
        // [JsonProperty("role")] public string Role { get; set; }
    }
}