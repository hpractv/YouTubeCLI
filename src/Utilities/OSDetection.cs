using System;
using System.Runtime.InteropServices;

namespace YouTubeCLI.Utilities
{
    public static class OSDetection
    {
        /// <summary>
        /// Detects if the application is running on Windows
        /// </summary>
        /// <returns>True if running on Windows, false otherwise</returns>
        public static bool IsWindows()
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        }

        /// <summary>
        /// Detects if the application is running on macOS
        /// </summary>
        /// <returns>True if running on macOS, false otherwise</returns>
        public static bool IsMacOS()
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
        }

        /// <summary>
        /// Detects if the application is running on Linux
        /// </summary>
        /// <returns>True if running on Linux, false otherwise</returns>
        public static bool IsLinux()
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
        }

        /// <summary>
        /// Gets the current operating system name
        /// </summary>
        /// <returns>String representation of the current OS</returns>
        public static string GetOSName()
        {
            if (IsWindows())
                return "Windows";
            else if (IsMacOS())
                return "macOS";
            else if (IsLinux())
                return "Linux";
            else
                return "Unknown";
        }

        /// <summary>
        /// Gets the current operating system architecture
        /// </summary>
        /// <returns>String representation of the current architecture</returns>
        public static string GetArchitecture()
        {
            return RuntimeInformation.OSArchitecture.ToString();
        }

        /// <summary>
        /// Gets detailed OS information
        /// </summary>
        /// <returns>Detailed OS information string</returns>
        public static string GetOSInfo()
        {
            return $"{GetOSName()} {GetArchitecture()}";
        }

        /// <summary>
        /// Gets the appropriate path separator for the current OS
        /// </summary>
        /// <returns>Path separator character</returns>
        public static char GetPathSeparator()
        {
            return IsWindows() ? '\\' : '/';
        }

        /// <summary>
        /// Normalizes a path to use the correct separators for the current OS
        /// </summary>
        /// <param name="path">The path to normalize</param>
        /// <returns>Normalized path with correct separators</returns>
        public static string NormalizePath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return path;

            var correctSeparator = GetPathSeparator();
            var incorrectSeparator = IsWindows() ? '/' : '\\';

            // Replace incorrect separators with correct ones
            var normalized = path.Replace(incorrectSeparator, correctSeparator);

            // Handle mixed separators by ensuring consistent separators
            if (IsWindows())
            {
                // On Windows, ensure we use backslashes consistently
                normalized = normalized.Replace('/', '\\');
            }
            else
            {
                // On Mac/Linux, ensure we use forward slashes consistently
                normalized = normalized.Replace('\\', '/');
            }

            return normalized;
        }
    }
}
