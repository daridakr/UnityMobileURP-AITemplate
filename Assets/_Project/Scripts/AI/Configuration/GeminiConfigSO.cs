using UnityEngine;

namespace AI.Gemini.Configuration
{
    [CreateAssetMenu(fileName = "GeminiConfig", menuName = "AI/Gemini API Configuration", order = 0)]
    public sealed class GeminiConfigSO : ScriptableObject
    {
        private const string DEFAULT_GEMINI_PRO_ENDPOINT = "https://generativelanguage.googleapis.com/v1beta/models/gemini-pro:generateContent";

        [Header("API Endpoint")]
        [Tooltip("API endpoint URL, including the model name.")]
        [SerializeField] private string _apiEndpoint = DEFAULT_GEMINI_PRO_ENDPOINT;

        [Header("Request Parameters")]
        [Range(0f, 1f)]
        [SerializeField] private float _temperature = 0.7f;

        public string ApiEndpoint => _apiEndpoint;
        public float Temperature => _temperature;

        private void OnValidate()
        {
            if (string.IsNullOrEmpty(_apiEndpoint)) _apiEndpoint = DEFAULT_GEMINI_PRO_ENDPOINT;
        }
    }
}