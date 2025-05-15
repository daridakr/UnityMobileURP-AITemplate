using System;
using UnityEngine;

namespace Editors.Build.Configuration
{
    [Serializable]
    public struct IOSBuildSettings
    {
        [SerializeField] private string _developerTeamID;

        public string DeveloperTeamID => _developerTeamID;
        // Future iOS specific settings:
        // public string ProvisioningProfileID;
        // public string SigningCertificate;

        public static readonly IOSBuildSettings Default = new() { _developerTeamID = string.Empty };
    }
}