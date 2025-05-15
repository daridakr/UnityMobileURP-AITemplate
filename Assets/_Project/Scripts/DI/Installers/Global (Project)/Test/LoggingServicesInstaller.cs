#if TEST || ENABLE_QA_LOGGING
using Test.Logging;
#endif
using UnityEngine;
using Zenject;

namespace DI.Installers
{
    [CreateAssetMenu(fileName = nameof(LoggingServicesInstaller), menuName = "DI/Installers/Logging Services Installer")]
    public sealed class LoggingServicesInstaller : ScriptableObjectInstaller<LoggingServicesInstaller>
    {
        private const string LOG_PREFIX = "[LoggingServicesInstaller] ";

        public override void InstallBindings()
        {
            #if TEST || ENABLE_QA_LOGGING
                Container.BindInterfacesAndSelfTo<FileQALogger>().AsSingle();
                Debug.Log($"{LOG_PREFIX}FileQALogger bound.");
            #else
                //Container.Bind<IQALogger>().To<NullQALogger>().AsSingle();
                Debug.Log($"{LOG_PREFIX}QALogger not bound in release build.");
            #endif
        }
    }
}