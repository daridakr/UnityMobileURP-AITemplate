using AI.Gemini.UI;
using System;
using UnityEngine;
using Zenject;

namespace Core.Coordination.Scenes
{
    /// <summary>
    /// Coordinates the setup and connection of View and Presenter for the Gemini Example Scene.
    /// Ensures initialization happens in the correct order after dependencies are ready.
    /// </summary>
    public sealed class GeminiExampleSceneCoordinator :
        IInitializable, IDisposable
    {
        private readonly UIGeminiExampleView _view;
        private readonly UIGeminiExamplePresenterFactory _presenterFactory;

        private UIGeminiExamplePresenter _presenterInstance; 
        
        private const string LOG_PREFIX = "[GeminiExampleSceneCoordinator] ";

        public GeminiExampleSceneCoordinator(
            UIGeminiExampleView view,
            UIGeminiExamplePresenterFactory presenterFactory)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));
            _presenterFactory = presenterFactory ?? throw new ArgumentNullException(nameof(presenterFactory));
        }

        public void Initialize()
        {
            if (_presenterInstance != null)
            {
                Debug.LogWarning(" Initialize called more than once.", _view);
                return;
            }

            Debug.Log($"{LOG_PREFIX}Initializing UI...", _view);

            try
            {
                InitializeUI();

                 Debug.Log($"{LOG_PREFIX}UI Initialized Successfully.", _view);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex, _view);
            }
        }

        private void InitializeUI()
        {
            _view.InitializeElements();
            _presenterInstance = _presenterFactory.Create();
            _presenterInstance.InitializeView(_view);
        }

        private void DisposeUI()
        {
            (_presenterInstance as IDisposable)?.Dispose();
            _presenterInstance = null;

            _view.Dispose();
        }

        public void Dispose()
        {
           DisposeUI();
        }
    }
}