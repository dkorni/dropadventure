using System;
using System.IO;
using JetBrains.Annotations;
using UnityEditor;

namespace HomaGames.HomaBelly.Installer
{
    public class FileFromRegistry
    {
        [NotNull]
        public string Guid { get; }

        public FileFromRegistry(string guid)
        {
            Guid = guid;
        }

        public string ToRegistryLine()
        {
            return Guid;
        }
    }
}