using System;
using UnityEngine;

namespace Editors.Build.Configuration
{
    [Serializable]
    public struct StackTraceSettings
    {
        public StackTraceLogType Log;
        public StackTraceLogType Warning;
        public StackTraceLogType Error;
        public StackTraceLogType Assert;
        public StackTraceLogType Exception;

        public static readonly StackTraceSettings DebugDefault = new()
        {
            Log = StackTraceLogType.ScriptOnly,
            Warning = StackTraceLogType.ScriptOnly,
            Error = StackTraceLogType.ScriptOnly,
            Assert = StackTraceLogType.ScriptOnly,
            Exception = StackTraceLogType.ScriptOnly
        };

        public static readonly StackTraceSettings ReleaseDefault = new()
        {
            Log = StackTraceLogType.None,
            Warning = StackTraceLogType.None,
            Error = StackTraceLogType.ScriptOnly,
            Assert = StackTraceLogType.None,
            Exception = StackTraceLogType.ScriptOnly
        };
    }
}