using Services.SceneLoading;
using UnityEngine;
using Zenject;

namespace DI.Installers
{
    [CreateAssetMenu(fileName = nameof(SceneServicesInstaller), menuName = "DI/Installers/Scene Services Installer")]
    public sealed class SceneServicesInstaller : ScriptableObjectInstaller<SceneServicesInstaller>
    {
        public override void InstallBindings()
        {   
            Container.Bind<IAddressableSceneLoader>().To<AddressableSceneLoader>().AsSingle();
        }
    }
}