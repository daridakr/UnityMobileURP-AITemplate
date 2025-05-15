using System;
using System.Threading.Tasks;
using Core.Coordination;
using Services.SceneLoading;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using Zenject;

namespace Core.Bootstraps
{
    public sealed class Bootstrap :
        IInitializable, IDisposable
    {
        private readonly BootstrapConfigSO _configuration;
        private readonly IApplicationCoordinator _appCoordinator;
        private readonly IAddressableSceneLoader _sceneLoader;

        private string MainSceneAddress => _configuration.MainSceneAddress;
        private LoadSceneMode MainSceneLoadMode => _configuration.MainSceneLoadMode;
        private bool ActivateOnLoad => _configuration.ActivateOnLoad;

        private const string LOG_PREFIX = "[Bootstrap] ";

        public Bootstrap(
            BootstrapConfigSO configuration,
            IAddressableSceneLoader sceneLoader,
            IApplicationCoordinator appCoordinator)
        {
            _appCoordinator = appCoordinator ?? throw new ArgumentNullException(nameof(appCoordinator));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _sceneLoader = sceneLoader ?? throw new ArgumentNullException(nameof(sceneLoader));
        }

        public void Initialize() => RunBootstrapFlowAsync();

        private async void RunBootstrapFlowAsync()
        {
            try
            {
                Debug.Log($"{LOG_PREFIX}Starting bootstrap flow...");

                Debug.Log($"{LOG_PREFIX}Waiting for essential services initialization...");
                await _appCoordinator.InitializationTask;
                Debug.Log($"{LOG_PREFIX}Essential services initialized successfully.");

                await LoadCoreContentAsync();

                Debug.Log($"{LOG_PREFIX}Bootstrap flow completed.");
            }
            catch (Exception ex)
            {
                // Critical start error - show user or exit
                Debug.LogError($"{LOG_PREFIX}CRITICAL ERROR during bootstrap process!");
                Debug.LogException(ex);
                // TODO: Implement displaying an error message to the player
                // or safe shutdown of the application.
                // for example: ShowCriticalErrorScreen("Failed to initialize application.");
            }
        }

        private async Task LoadCoreContentAsync()
        {
            if (_sceneLoader == null)
            {
                Debug.LogError($"{LOG_PREFIX}Dependencies not injected! Ensure ProjectContext is running and bindings are correct.");
                return;
            }

            Debug.Log($"{LOG_PREFIX}Initializing Addressables...");
            await Addressables.InitializeAsync().Task;
            Debug.Log($"{LOG_PREFIX}Addressables Initialized. Loading Main Scene ('{MainSceneAddress}')...");

            SceneInstance? loadedSceneInstance = await _sceneLoader.LoadSceneAsync(MainSceneAddress, MainSceneLoadMode, ActivateOnLoad);

            if (loadedSceneInstance.HasValue)
                Debug.Log($"{LOG_PREFIX}Main Scene ('{loadedSceneInstance.Value.Scene.name}') Loaded successfully.");
            else
                throw new Exception($"{LOG_PREFIX}Failed to load Main Scene using address: {MainSceneAddress}");
        }

        public void Dispose()
        {
            // main scene unload ?
        }
    }
}