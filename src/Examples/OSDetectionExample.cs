using System;
using YouTubeCLI.Utilities;

namespace YouTubeCLI.Examples
{
    /// <summary>
    /// Example class showing how to use OS detection in your application
    /// </summary>
    public static class OSDetectionExample
    {
        public static void DemonstrateOSDetection()
        {
            Console.WriteLine("=== OS Detection Examples ===");

            // Basic OS detection
            Console.WriteLine($"Is Windows: {OSDetection.IsWindows()}");
            Console.WriteLine($"Is macOS: {OSDetection.IsMacOS()}");
            Console.WriteLine($"Is Linux: {OSDetection.IsLinux()}");

            // Get OS information
            Console.WriteLine($"OS Name: {OSDetection.GetOSName()}");
            Console.WriteLine($"OS Info: {OSDetection.GetOSInfo()}");
            Console.WriteLine($"Architecture: {OSDetection.GetArchitecture()}");

            // Path handling examples
            Console.WriteLine($"Path Separator: '{OSDetection.GetPathSeparator()}'");

            // Path normalization examples
            string windowsPath = "folder\\subfolder\\file.txt";
            string unixPath = "folder/subfolder/file.txt";

            Console.WriteLine($"Windows path normalized: {OSDetection.NormalizePath(windowsPath)}");
            Console.WriteLine($"Unix path normalized: {OSDetection.NormalizePath(unixPath)}");

            // Conditional logic based on OS
            if (OSDetection.IsWindows())
            {
                Console.WriteLine("Running Windows-specific logic");
            }
            else if (OSDetection.IsMacOS())
            {
                Console.WriteLine("Running macOS-specific logic");
            }
            else if (OSDetection.IsLinux())
            {
                Console.WriteLine("Running Linux-specific logic");
            }
        }
    }
}
