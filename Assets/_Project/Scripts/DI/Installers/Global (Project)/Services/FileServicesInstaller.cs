using Services.Files;
using UnityEngine;
using Zenject;

namespace DI.Installers
{
    [CreateAssetMenu(fileName = nameof(FileServicesInstaller), menuName = "DI/Installers/File Services Installer")]
    public sealed class FileServicesInstaller : ScriptableObjectInstaller<FileServicesInstaller>
    {
        public override void InstallBindings()
        {
            Container.Bind<IPersistentFileService>().To<PersistentStreamFileService>().AsSingle();
            Container.Bind<ITransientFileService>().To<TransientStreamFileService>().AsSingle();
        }
    }
}