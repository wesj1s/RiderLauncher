namespace RiderLauncher
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Windows;

    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(params string[] options)
        {
            var existing = Process.GetProcessesByName("rider64");
            if (existing.Any())
            {
                SetForegroundWindow(existing.FirstOrDefault().MainWindowHandle);
                return;
            }

            var elevated = false;
            IEnumerable<string> args = options;
            if ((options != null) && options.Any(o => o.Equals("-elevated")))
            {
                elevated = true;
                args = args.Except(new[] { "-elevated" });
            }

            // assumes this is being run from a sibling directory of the rider executable
            var riderRoot = Path.GetDirectoryName(Directory.GetCurrentDirectory());
            Console.WriteLine($"Launching latest Rider from {riderRoot}");

            var directories = Directory.EnumerateDirectories(riderRoot);
            var buildTxtVer = directories
                .SelectMany(d => Directory.EnumerateFiles(d, "build.txt", SearchOption.TopDirectoryOnly))
                .ToDictionary(b => b, GetBuildTxtVersion);
            var maxVer = buildTxtVer.Max(kv => kv.Value);
            var maxPath = buildTxtVer.LastOrDefault(kv => kv.Value == maxVer).Key;
            if (maxPath == null)
            {
                Console.WriteLine("Path not found in " + riderRoot);
            }
            else
            {
                var path = Path.Combine(Path.GetDirectoryName(maxPath), @"bin\rider64.exe");
                Console.WriteLine("Starting " + path);

                var startInfo = new ProcessStartInfo(path)
                {
                    UseShellExecute = true,
                    WindowStyle = ProcessWindowStyle.Normal,
                    RedirectStandardOutput = false,
                    Verb = elevated ? "runas" : "open",
                    Arguments = args.Any() ? args.Aggregate((a,b) => string.Concat(a, " ", b)) : string.Empty
                };
                try
                {
                    Process.Start(startInfo);
                }
                catch (Win32Exception)
                {
                }
            }
        }

        public static Version GetBuildTxtVersion(string buildTxtFileName)
        {
            var buildText = File.ReadAllText(buildTxtFileName);
            var versionText = buildText.Split('-').LastOrDefault();
            return new Version(versionText ?? string.Empty);
        }

        [DllImport("User32.dll")]
        static extern IntPtr SetForegroundWindow(IntPtr hWnd);
    }
}
