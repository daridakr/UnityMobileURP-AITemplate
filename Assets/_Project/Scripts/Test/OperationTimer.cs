#if TEST
using System;
using System.Diagnostics;

namespace Test
{
    public struct OperationTimer :
        IDisposable
    {
        private readonly Stopwatch _stopwatch;
        private readonly string _operationLogName;
        public long ResultElapsed { get; private set; }

        private const string LOG_PREFIX = "[OperationTimer] ";
        private const string NULL_OPERATION_NAME = "Unnamed Operation";
        private const int LATENCY_DEFAULT_VALUE = -1;

        public OperationTimer(string operationName)
        {
            _operationLogName = operationName ?? NULL_OPERATION_NAME;
            ResultElapsed = LATENCY_DEFAULT_VALUE;

            _stopwatch = Stopwatch.StartNew();

            UnityEngine.Debug.Log($"[OperationTimer] Starting: '{_operationLogName}'...");
        }

        public static OperationTimer Start(string operationName) => new(operationName);

        public void Dispose()
        {
            _stopwatch.Stop();

            ResultElapsed = _stopwatch.ElapsedMilliseconds;

            UnityEngine.Debug.Log($"{LOG_PREFIX}{_operationLogName}: {ResultElapsed} ms.");
        }
    }
}
#endif