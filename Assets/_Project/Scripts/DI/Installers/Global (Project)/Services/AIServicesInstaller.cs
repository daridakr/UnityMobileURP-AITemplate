using UnityEngine;
using Zenject;
using AI.Gemini.Interfaces;
using AI.Gemini.Services;
using AI.Gemini.Configuration;

#if TEST
using Test.Gemini;
#endif

namespace DI.Installers
{
    [CreateAssetMenu(fileName = nameof(AIServicesInstaller), menuName = "DI/Installers/AI Services Installer")]
    public sealed class AIServicesInstaller : ScriptableObjectInstaller<AIServicesInstaller>
    {
        [Header("Configuration Assets")]
        [Tooltip("Assign the GeminiConfig ScriptableObject Asset.")]
        [SerializeField] private GeminiConfigSO _geminiConfigAsset;

        private const string LOG_PREFIX = "[AIServicesInstaller] ";

        public override void InstallBindings()
        {
            if (_geminiConfigAsset == null)
            {
                Debug.LogError($"{nameof(GeminiConfigSO)} asset is not assigned in the {nameof(AIServicesInstaller)} asset! Cannot bind {nameof(IGeminiService)}.");
                return;
            }
            else Container.Bind<GeminiConfigSO>().FromInstance(_geminiConfigAsset).AsSingle();

            Container.Bind<IGeminiHttpClient>().To<GeminiHttpClient>().AsSingle();

            #if TEST
                Container.Decorate<IGeminiHttpClient>().With<TimedGeminiHttpClient>();
                Debug.Log($"{LOG_PREFIX}TEST build: Applied TimedGeminiHttpClientDecorator.");
            #else
                Debug.Log($"{LOG_PREFIX}RELEASE build: IGeminiService bound directly to GeminiService (using default GeminiHttpClient).");
            #endif
            
            Container.Bind<IGeminiService>().To<GeminiService>().AsSingle();
        }
    }
}