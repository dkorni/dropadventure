using System;
using UnityEngine;

namespace HomaGames.HomaBelly.Installer.Utilities
{
    /// <summary>
    /// Exception to be thrown by EditorHttpCaller when the API returns
    /// an error
    /// </summary>
    public class InstallerHttpCallerException : Exception
    {
        public enum ErrorStatus
        {
            UNKNOWN,
            TOKEN_ID_MISSING,
            MANIFEST_SDK_VERSION_NOT_ALLOWED,
            MANIFEST_NO_MATCH
        }

        public ErrorStatus Status { get; }
        public string StatusString { get; }

        public InstallerHttpCallerException(string status, string message) : base(message)
        {
            this.StatusString = status;
            Status = Enum.TryParse(message, out ErrorStatus s) ? s : ErrorStatus.UNKNOWN;
        }
    }
}