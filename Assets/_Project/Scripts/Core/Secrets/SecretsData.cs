namespace Core.Secrets
{
    public sealed class SecretsData
    {
        public string GeminiApiKey { get; set; } = SecretsParams.DEFAULT_API_KEY_PLACEHOLDER;
        // public string AnotherServiceApiKey { get; set; } = SecretsConstants.DEFAULT_API_KEY_PLACEHOLDER;

        public bool IsGeminiKeyValid() =>
            !string.IsNullOrEmpty(GeminiApiKey) && GeminiApiKey != SecretsParams.DEFAULT_API_KEY_PLACEHOLDER;
    }
}