using UnityEngine;
using Zenject;
using AI.Gemini.UI;
using Core.Coordination.Scenes;

namespace DI.Installers
{
    public sealed class GeminiExampleInstaller : MonoInstaller
    {
        [Header("Scene References")]
        [Tooltip("Drag the GameObject with the UIGeminiExampleView component from your scene hierarchy HERE.")]
        [SerializeField] private UIGeminiExampleView _geminiViewInstance;

        public override void InstallBindings()
        {
            if (_geminiViewInstance == null)
            {
                // ILoggerService
                Debug.LogError("UIGeminiExampleView instance is not assigned in the GeminiExampleSceneInstaller! Drag the UI GameObject onto this field.");
                return;
            }

            Container.Bind<UIGeminiExampleView>().FromInstance(_geminiViewInstance).AsSingle();
            Container.BindFactory<UIGeminiExamplePresenter, UIGeminiExamplePresenterFactory>().AsSingle();
            Container.BindInterfacesAndSelfTo<GeminiExampleSceneCoordinator>().AsSingle().NonLazy();
        }
    }
}