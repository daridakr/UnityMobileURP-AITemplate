using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

namespace Core.Bootstraps
{
    [CreateAssetMenu(fileName = "BootstrapConfig", menuName = "Bootstrap/Bootstrap Configuration", order = 0)]
    public sealed class BootstrapConfigSO : ScriptableObject
    {
        [Header("Main Scene Load Configuration")]
        [Tooltip("Assign the Addressable Asset Reference for the main scene here.")]
        [SerializeField] private AssetReference _mainSceneReference;
        [SerializeField] private LoadSceneMode _loadSceneMode;
        [Tooltip("If false, the scene will load but not activate (for background loading). The SceneInstance returned has an Activate() method that can be called to do this at a later point.")]
        [SerializeField] private bool _activateOnLoad = true;

        public string MainSceneAddress
        {
            get
            {
                if (_mainSceneReference == null || !_mainSceneReference.RuntimeKeyIsValid())
                {
                    Debug.LogError("[Bootstrap] Main Scene AssetReference is not assigned or invalid!");
                    return null;
                }
                return _mainSceneReference.RuntimeKey.ToString();
            }
        }
        public LoadSceneMode MainSceneLoadMode => _loadSceneMode;
        public bool ActivateOnLoad => _activateOnLoad;
    }
}