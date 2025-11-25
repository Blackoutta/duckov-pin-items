using UnityEngine;

namespace PinItems
{
    internal static class PinLogger
    {
        private const string Prefix = "[PinItems]";

        internal static void Info(string message) => Debug.Log($"{Prefix} {message}");
        internal static void Warn(string message) => Debug.LogWarning($"{Prefix} {message}");
        internal static void Error(string message) => Debug.LogError($"{Prefix} {message}");
    }
}
