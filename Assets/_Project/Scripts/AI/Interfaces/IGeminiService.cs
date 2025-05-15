using System.Threading.Tasks;

namespace AI.Gemini.Interfaces
{
    public interface IGeminiService
    {
        public Task<GeminiResult> SendPromptAsync(string prompt);

        // Potential future methods:
        // public Task<GeminiResult> SendChatAsync(List<ContentDTO> chatHistory);
        // public Task<SomeOtherResult> AnalyzeImageAsync(byte[] imageData);
    }
}