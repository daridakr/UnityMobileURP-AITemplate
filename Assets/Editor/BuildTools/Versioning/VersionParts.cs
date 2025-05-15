using System;
using UnityEngine;

namespace Editors.Build.Versioning
{
    public struct VersionParts :
        IEquatable<VersionParts>
    {
        public int Major {get; private set; }
        public int Minor {get; private set; }
        public int Patch {get; private set; }

        private static readonly string LOG_PREFIX = $"[{nameof(VersionParts)}] ";

        public VersionParts(int major, int minor, int patch)
        {
            Major = Math.Max(0, major); // non negative
            Minor = Math.Max(0, minor);
            Patch = Math.Max(0, patch);
        }

        public VersionParts(Version systemVersion)
        {
            Major = 0;
            Minor = 1;
            Patch = 0;

            if (systemVersion != null)
            {
                Major = systemVersion.Major < 0 ? 0 : systemVersion.Major;
                Minor = systemVersion.Minor < 0 ? 0 : systemVersion.Minor;
                Patch = systemVersion.Build < 0 ? 0 : systemVersion.Build; // System.Version.Build is used for Patch
            }
        }

        public VersionParts IncrementPatch()
        {
            // bussiness logic, validation, etc.
            // for example: if Patch > 99 then Increment Minor & reset Patch.

            Patch++;

            return this;
        }

        public VersionParts IncrementMinor()
        {
            Minor++;
            Patch = 0;

            return this;
        }
        
        public VersionParts IncrementMajor()
        {
            Major++;
            Minor = 0;
            Patch = 0;

            return this;
        }

        public override string ToString() => $"{Major}.{Minor}.{Patch}";

        public bool Equals(VersionParts other) => Major == other.Major && Minor == other.Minor && Patch == other.Patch;
        public override bool Equals(object obj) => obj is VersionParts other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(Major, Minor, Patch);
        public static bool operator ==(VersionParts left, VersionParts right) => left.Equals(right);
        public static bool operator !=(VersionParts left, VersionParts right) => !(left == right);
    }
}