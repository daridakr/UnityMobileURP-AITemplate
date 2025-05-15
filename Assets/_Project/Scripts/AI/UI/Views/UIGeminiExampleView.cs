using System;
using UnityEngine;
using UnityEngine.UIElements;
using UI.Views;
using Utilities.Attributes;

namespace AI.Gemini.UI
{
    [RequireComponent(typeof(UIDocument))]
    public sealed class UIGeminiExampleView : UIBaseView
    {
        [BindUIElement(PromptInputName)] private TextField _promptInput;
        [BindUIElement(SendButtonName)] private Button _sendButton;
        [BindUIElement(ResponseOutputName)] private Label _responseOutput;
        [BindUIElement(StatusLabelName)] private Label _statusLabel;
        #if TEST
        [BindUIElement(LatencyLabelName)] private Label _latencyLabel;
        #endif

#region Element Names Consts
        private const string PromptInputName = "prompt-input";
        private const string SendButtonName = "send-button";
        private const string ResponseOutputName = "response-output";
        private const string StatusLabelName = "status-label";
        #if TEST
        private const string LatencyLabelName = "latency-label";
        #endif
#endregion

        public string ResponseText => _responseOutput?.text ?? "";
        public string PromptText => _promptInput?.text ?? "";

        public event Action<string> SendPromptClicked;

        protected override void OnElementsInitialized() => _sendButton.clicked += OnSendButtonClickedInternal;

        public void Render(UIGeminiExampleViewModel viewModel)
        {
            if (_promptInput != null) _promptInput.value = viewModel.PromptText;
            if (_responseOutput != null) _responseOutput.text = viewModel.ResponseText;
            if (_statusLabel != null) _statusLabel.text = viewModel.StatusText;
            #if TEST
            if (_latencyLabel != null) _latencyLabel.text = viewModel.LastLatencyText;
            #endif

            _sendButton?.SetEnabled(viewModel.IsSendButtonEnabled);

            // status label style classes ...
        }

        private void OnSendButtonClickedInternal() => SendPromptClicked.Invoke(PromptText);

        protected override void OnDisposed()
        {
            if (_sendButton == null) return;
            
            _sendButton.clicked -= OnSendButtonClickedInternal;
        }
    }
}