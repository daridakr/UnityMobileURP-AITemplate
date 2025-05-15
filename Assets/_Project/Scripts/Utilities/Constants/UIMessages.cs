namespace Utilities.Constants
{
    // for easier management/localization
    public static class UIMessages
    {
        public static class GeminiRequestStatus
        {
            public const string Idle = "Ready.";
            public const string Sending = "Sending request...";
            public const string Success = "Success!";
            public const string Failed = "Failed!";
        }

        public static class ValidationErrors
        {
            public const string Prefix = "Error: ";
            public const string PromptEmpty = "Prompt cannot be empty.";
            public const string ServiceUnavailable = "Service is unavailable.";
        }

        public static class Formatting
        {
            #if TEST
            public const string LatencyFormat = "Latency: {0} ms";
            public const string TimingNotAvailable = "Timing N/A";
            #endif
        }
    }
}