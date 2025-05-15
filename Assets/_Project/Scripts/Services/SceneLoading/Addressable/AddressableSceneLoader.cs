using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Services.SceneLoading
{
    public sealed class AddressableSceneLoader :
        IAddressableSceneLoader
    {
        private readonly Dictionary<string, AsyncOperationHandle<SceneInstance>> _loadedScenes;

        public AddressableSceneLoader() =>
            _loadedScenes = new Dictionary<string, AsyncOperationHandle<SceneInstance>>();

        public async Task<SceneInstance?> LoadSceneAsync(string sceneAddress, LoadSceneMode loadMode = LoadSceneMode.Single, bool activateOnLoad = true)
        {
            if (string.IsNullOrEmpty(sceneAddress)) return null;

            if (_loadedScenes.ContainsKey(sceneAddress))
            {
                if (_loadedScenes[sceneAddress].IsValid() && _loadedScenes[sceneAddress].Result.Scene.isLoaded)
                    return _loadedScenes[sceneAddress].Result;
                else _loadedScenes.Remove(sceneAddress);
            }

            AsyncOperationHandle<SceneInstance> handleToLoadScene = Addressables.LoadSceneAsync(sceneAddress, loadMode, activateOnLoad);
            await handleToLoadScene.Task;

            if (handleToLoadScene.Status == AsyncOperationStatus.Succeeded)
            {
                _loadedScenes[sceneAddress] = handleToLoadScene;
                return handleToLoadScene.Result;
            }
            else
            {
                Addressables.Release(handleToLoadScene);
                return null;
            }
        }

        public async Task UnloadSceneAsync(SceneInstance sceneInstance)
        {
            string addressToUnload = null;
            AsyncOperationHandle<SceneInstance> handleToRelease = default;

            foreach (KeyValuePair<string, AsyncOperationHandle<SceneInstance>> loadedScene in _loadedScenes)
            {
                if (loadedScene.Value.IsValid() && loadedScene.Value.Result.Scene == sceneInstance.Scene)
                {
                    addressToUnload = loadedScene.Key;
                    handleToRelease = loadedScene.Value;
                    break;
                }
            }

            if (addressToUnload == null || !handleToRelease.IsValid()) return;

            AsyncOperationHandle<SceneInstance> handleToUnload = Addressables.UnloadSceneAsync(handleToRelease, true);
            await handleToUnload.Task;

            if (handleToUnload.Status == AsyncOperationStatus.Succeeded)
                Debug.Log($"Scene '{addressToUnload}' unloaded successfully.");

            _loadedScenes.Remove(addressToUnload);
        }
    }
}