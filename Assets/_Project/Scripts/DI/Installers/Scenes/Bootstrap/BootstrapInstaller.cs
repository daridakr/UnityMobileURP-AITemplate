using Core.Bootstraps;
using UnityEngine;
using Zenject;

namespace DI.Installers
{
    public sealed class BootstrapInstaller : MonoInstaller
    {
        [Header("Configuration Assets")]
        [Tooltip("Assign the BootstrapConfig ScriptableObject Asset.")]
        [SerializeField] private BootstrapConfigSO _bootstrapConfigAsset;

        public override void InstallBindings()
        {
            if (_bootstrapConfigAsset == null)
            {
                Debug.LogError($"{nameof(BootstrapConfigSO)} asset is not assigned in the {nameof(BootstrapInstaller)} asset! Cannot bind {nameof(Bootstrap)}.");
                return;
            }
            
            Container.Bind<BootstrapConfigSO>().FromInstance(_bootstrapConfigAsset).AsSingle();
            Container.BindInterfacesAndSelfTo<Bootstrap>().AsSingle().NonLazy();
        }
    }
}