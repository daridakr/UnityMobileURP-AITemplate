using System.Threading.Tasks;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Services.SceneLoading
{
    public interface  IAddressableSceneLoader
    {
        public Task<SceneInstance?> LoadSceneAsync(string sceneAddress, LoadSceneMode loadMode = LoadSceneMode.Single, bool activateOnLoad = true);
        public Task UnloadSceneAsync(SceneInstance sceneInstance);
    }
}