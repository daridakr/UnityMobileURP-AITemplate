using System;
using Zenject;
using System.Threading.Tasks;
using UnityEngine;

#if TEST || ENABLE_QA_LOGGING
using Test.Logging;
#endif

namespace Core.Coordination
{
    public sealed class ApplicationCoordinator :
        IApplicationCoordinator, IInitializable
    {
        #if TEST || ENABLE_QA_LOGGING
        private readonly IQALogger _qaLogger;
        #endif

        // You can add any async initializable services here.
        // private readonly IOtherAsyncService _otherService;

        private Task _initializationTask;
        public Task InitializationTask => _initializationTask;

        private const string LOG_PREFIX = "[AppCoordinator] ";

        public ApplicationCoordinator(
            #if TEST || ENABLE_QA_LOGGING
            IQALogger qALogger
            #endif
            // , IOtherAsyncService otherService ...
            )
        {
            _initializationTask = Task.CompletedTask;

            #if TEST || ENABLE_QA_LOGGING
            _qaLogger = qALogger ?? throw new ArgumentNullException(nameof(qALogger));
            #endif
            // _otherService = otherService ?? ...;
        }

        public void Initialize()
        {
            Debug.Log($"{LOG_PREFIX}Initialize called. Starting async service initialization...");

            _initializationTask = InitializeServicesAsync();
        }

        private async Task InitializeServicesAsync()
        {
            try
            {
                #if TEST || ENABLE_QA_LOGGING
                    await _qaLogger.InitializeAsync();
                    Debug.Log($"{LOG_PREFIX}QALogger Initialized.");
                #endif

                // await _otherService.InitializeAsync();
            }
            catch (Exception ex)
            {
                Debug.LogError($"{LOG_PREFIX}Asynchronous initialization FAILED!");
                Debug.LogException(ex);
                throw;
            }
        }
    }
}