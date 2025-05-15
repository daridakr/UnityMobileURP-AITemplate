#if TEST
using Utilities;
#endif
using Utilities.Constants;

namespace AI.Gemini.UI
{
    public readonly struct UIGeminiExampleViewModel
    {
        public readonly string PromptText;
        public readonly string ResponseText;
        public readonly string StatusText;
        public readonly bool IsSendButtonEnabled;
        
        #if TEST
        public readonly string LastLatencyText;
        private const int LATENCY_DEFAULT_VALUE = -1;
        #endif

        public UIGeminiExampleViewModel(
            string statusText,
            string responseText,
            bool isSendEnabled,
            string promptText = ""
            #if TEST
            , long requestLatency = LATENCY_DEFAULT_VALUE
            #endif
            )
        {
            StatusText = statusText;
            ResponseText = responseText;
            IsSendButtonEnabled = isSendEnabled;
            PromptText = promptText;

            #if TEST
            LastLatencyText = FormatUtils.FormatLatency(requestLatency);
            #endif
        }

        public static UIGeminiExampleViewModel Default =>
        new(
            statusText: UIMessages.GeminiRequestStatus.Idle,
            responseText: string.Empty,
            isSendEnabled: true,
            promptText: string.Empty
        );

        public static UIGeminiExampleViewModel LoadingState(string currentPrompt) =>
        new(
            statusText: UIMessages.GeminiRequestStatus.Sending,
            responseText: string.Empty,
            isSendEnabled: false,
            promptText: currentPrompt
        );

        public static UIGeminiExampleViewModel SuccessState(
            string response,
            long  latency,
            string prompt = "") =>
        new(
            statusText: UIMessages.GeminiRequestStatus.Success,
            responseText: response,
            isSendEnabled: true,
            promptText: prompt
            #if TEST
            , requestLatency: latency
            #endif
        );

        public static UIGeminiExampleViewModel ErrorState(
            string errorMessage,
            string currentPrompt,
            string lastResponse = ""
            #if TEST
            , long latency = LATENCY_DEFAULT_VALUE
            #endif
            ) =>
        new(
            statusText: $"{UIMessages.ValidationErrors.Prefix}{errorMessage}",
            responseText: lastResponse,
            isSendEnabled: true,
            promptText: currentPrompt
            #if TEST
            , requestLatency: latency
            #endif
        );

        // You can add additional static fabric methods for other conditions/states here.
    }
}