using System;

namespace Editors.Build.Configuration
{
    [Flags]
    public enum ScriptDefineSymbols
    {
        NONE = 0,
        TEST = 1 << 0,
        DEVELOPMENT_BUILD = 1 << 1,
        ENABLE_QA_LOGGING = 1 << 2,
        ENABLE_PROFILER = 1 << 3,
        //CUSTOM_FEATURE_A = 1 << 4,
    }
}