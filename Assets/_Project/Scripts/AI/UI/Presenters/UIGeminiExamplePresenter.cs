using System;
using AI.Gemini.Interfaces;
using Utilities.Constants;
using UnityEngine;

#if TEST || ENABLE_QA_LOGGING
using Test.Logging;
#endif
#if TEST
using Test;
#endif

namespace AI.Gemini.UI
{
    public sealed class UIGeminiExamplePresenter :
        IDisposable
    {
        private readonly IGeminiService _geminiService;
        
        private UIGeminiExampleView _view;
        private bool _isSendingRequest = false;

        #if TEST || ENABLE_QA_LOGGING
        private readonly IQALogger _qaLogger;
        #endif

        private const string LOG_PREFIX = "[UIGeminiExamplePresenter] ";
        private const int MAX_LOGGED_PROMPT_LENGTH = 50;

        public UIGeminiExamplePresenter(
            IGeminiService geminiService
            #if TEST || ENABLE_QA_LOGGING
            , IQALogger qaLogger
            #endif
            )
        {
            _geminiService = geminiService ?? throw new ArgumentNullException(nameof(geminiService));

            #if TEST || ENABLE_QA_LOGGING
            _qaLogger = qaLogger ?? throw new ArgumentNullException(nameof(qaLogger));
            #endif
        }

        public void InitializeView(UIGeminiExampleView view)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));
            _view.SendPromptClicked += HandleSendPrompt;

            _view.Render(UIGeminiExampleViewModel.Default);

            Debug.Log($"{LOG_PREFIX}Initialized.");
        }
        
        private async void HandleSendPrompt(string prompt)
        {
            if (_view == null) { Debug.Log($"{LOG_PREFIX}View is null!"); return; }
            if (_isSendingRequest) return;

            if (string.IsNullOrWhiteSpace(prompt))
            {
                _view.Render(UIGeminiExampleViewModel.ErrorState(
                    UIMessages.ValidationErrors.PromptEmpty, prompt, _view.ResponseText));

                return;
            }

            _isSendingRequest = true;
            _view.Render(UIGeminiExampleViewModel.LoadingState(prompt));
            Debug.Log($"{LOG_PREFIX}Sending prompt: '{prompt?.Substring(0, Math.Min(prompt?.Length ?? 0, MAX_LOGGED_PROMPT_LENGTH))}...'");

            GeminiResult geminiResult = new();
            long overallLatency = -1; // ms

            #if TEST
                var overallRequestTimer = OperationTimer.Start("TOTAL API Operation Latency");
            #endif

            try
            {
                geminiResult = await _geminiService.SendPromptAsync(prompt);
            }
            catch (Exception ex)
            {
                Debug.LogError($"{LOG_PREFIX}Exception during SendPromptAsync call. Latency before exception: {overallLatency} ms. Error: {ex.Message}");
                Debug.LogException(ex);

                geminiResult = GeminiResult.Failure($"An unexpected exception occurred in Presenter: {ex.GetType().Name}");
            }
            finally
            {
                #if TEST
                    try { overallRequestTimer.Dispose(); overallLatency = overallRequestTimer.ResultElapsed; }
                    catch (Exception disposeException) { Debug.LogException(disposeException, _view); }
                #endif

                _isSendingRequest = false;
            }

            #if TEST || ENABLE_QA_LOGGING
                // do NOT wait for logger method to complete (fire-and-forget), so as not to block UI updates.
                 _ = _qaLogger.LogQaAsync(prompt, geminiResult.ResponseText, geminiResult.IsSuccess, overallLatency, geminiResult.ErrorMessage);
            #endif

            Debug.Log($"{LOG_PREFIX}Request finished. Success: {geminiResult.IsSuccess}.");

            // If View is destroyed during the request.
            if (_view == null) { Debug.LogWarning($"{LOG_PREFIX}View became null after request finished."); return; }

            UIGeminiExampleViewModel viewModelResult;

            if (geminiResult.IsSuccess)
                viewModelResult = UIGeminiExampleViewModel.SuccessState(geminiResult.ResponseText, overallLatency, prompt);
            else
                viewModelResult = UIGeminiExampleViewModel.ErrorState(
                    geminiResult.ErrorMessage,
                    prompt,
                    _view.ResponseText
                    #if TEST
                    , overallLatency
                    #endif
                    );

            _view.Render(viewModelResult);
        }
        
        public void Dispose()
        {
            if(_view == null) return;

            _view.SendPromptClicked -= HandleSendPrompt;
            _view = null;

            #if TEST || ENABLE_QA_LOGGING
               (_qaLogger as IDisposable)?.Dispose();
            #endif
        }
    }
}