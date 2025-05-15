using Core.Secrets;
using UnityEngine;
using Zenject;

namespace DI.Installers
{
    [CreateAssetMenu(fileName = nameof(SecretsServicesInstaller), menuName = "DI/Installers/Secrets Services Installer")]
    public sealed class SecretsServicesInstaller : ScriptableObjectInstaller<SecretsServicesInstaller>
    {
        public override void InstallBindings()
        {   
            Container.Bind<ISecretKeyProvider>().To<JsonSecretKeyProvider>().AsSingle();
        }
    }
}