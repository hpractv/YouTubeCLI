using System;
using YouTubeCLI.Utilities;

namespace YouTubeCLI.Examples
{
    /// <summary>
    /// Test class to demonstrate cross-platform path handling
    /// </summary>
    public static class CrossPlatformTest
    {
        public static void TestPathHandling()
        {
            Console.WriteLine("=== Cross-Platform Path Handling Test ===");
            Console.WriteLine($"Current OS: {OSDetection.GetOSInfo()}");
            Console.WriteLine($"Path Separator: '{OSDetection.GetPathSeparator()}'");
            Console.WriteLine();

            // Test different path formats
            string[] testPaths = {
                "folder\\subfolder\\file.txt",     // Windows format
                "folder/subfolder/file.txt",       // Unix format
                "mixed\\path/with/both.txt",       // Mixed format
                "simple\\file.png",                // Simple Windows
                "simple/file.png"                  // Simple Unix
            };

            foreach (string path in testPaths)
            {
                string normalized = OSDetection.NormalizePath(path);
                Console.WriteLine($"Original:  {path}");
                Console.WriteLine($"Normalized: {normalized}");
                Console.WriteLine();
            }

            // Test OS-specific logic
            if (OSDetection.IsWindows())
            {
                Console.WriteLine("✅ Running on Windows - paths will use backslashes");
            }
            else if (OSDetection.IsMacOS())
            {
                Console.WriteLine("✅ Running on macOS - paths will use forward slashes");
            }
            else if (OSDetection.IsLinux())
            {
                Console.WriteLine("✅ Running on Linux - paths will use forward slashes");
            }
        }
    }
}
