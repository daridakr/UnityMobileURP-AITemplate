namespace AI.Gemini.Interfaces
{
    public readonly struct GeminiResult
    {
        public bool IsSuccess { get; }
        public string ResponseText { get; }
        public string ErrorMessage { get; }

        private GeminiResult(bool success, string response, string error)
        {
            IsSuccess = success;
            ResponseText = response;
            ErrorMessage = error;
        }

        public static GeminiResult Success(string response) => new(true, response, null);
        public static GeminiResult Failure(string error) => new(false, null, error);
    }
}