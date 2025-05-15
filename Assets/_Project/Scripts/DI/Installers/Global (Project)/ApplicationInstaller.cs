using Core.Coordination;
using UnityEngine;
using Zenject;

namespace DI.Installers
{
    [CreateAssetMenu(fileName = "_" + nameof(ApplicationInstaller), menuName = "DI/Installers/_Application Installer")]
    public sealed class ApplicationInstaller : ScriptableObjectInstaller<ApplicationInstaller>
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<ApplicationCoordinator>().AsSingle();
        }
    }
}