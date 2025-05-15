using System;
using UnityEngine;

namespace Editors.Build.Configuration
{
    [Serializable]
    public struct AndroidBuildSettings
    {
        [SerializeField] private bool _useCustomKeystore;
        [SerializeField] private bool _buildAppBundle;

        public bool UseCustomKeystore => _useCustomKeystore;
        public bool BuildAppBundle => _buildAppBundle;

        public static readonly AndroidBuildSettings DebugDefault = new()
        {
            _useCustomKeystore = false,
            _buildAppBundle = false
        };

        public static readonly AndroidBuildSettings ReleaseDefault = new()
        {
            _useCustomKeystore = true,
            _buildAppBundle = true
        };
    }
}