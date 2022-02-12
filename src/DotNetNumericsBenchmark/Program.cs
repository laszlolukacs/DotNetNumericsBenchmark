// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Laszlo Lukacs">
//   See LICENSE for details.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.IO;
using System.Management;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using BenchmarkDotNet.Running;

namespace DotNetNumericsBenchmark
{
    /// <summary>
    /// The main class of the program.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The entry point of the program.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public static void Main(string[] args)
        {
            Console.WriteLine($"DotNetNumericsBenchmark {typeof(Program).Assembly.GetName().Version}");

            GetProcessorInfo(out var processorName, out var clockSpeed);
            var output = new StringBuilder();
            output.AppendLine($"Starting execution at {DateTime.Now} on {Environment.MachineName} running {Environment.OSVersion.VersionString}");

            var currentLine = $"CPU: {processorName}, cores: {Environment.ProcessorCount}, speed: {clockSpeed} MHz";
            output.AppendLine(currentLine);
            Console.WriteLine(currentLine);

            var simdLength = Vector<double>.Count;
            var simdAvailable = Vector.IsHardwareAccelerated;
            currentLine = $"CPU SIMD instructions present: {simdAvailable}";
            output.AppendLine(currentLine);
            Console.WriteLine(currentLine);

            currentLine = $"CPU SIMD length: {sizeof(double) * simdLength * 8} bits = {simdLength} of {typeof(double).FullName} ({sizeof(double) * 8} bits each)";
            output.AppendLine(currentLine);
            Console.WriteLine(currentLine);

            var summary = BenchmarkRunner.Run<ScalarVsSimdArrayAverageBenchmark>();
        }

        private static void GetProcessorInfo(out string processorName, out string clockSpeed)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                GetProcessorInfoWindows(out processorName, out clockSpeed);
            }
            else
            {
                GetProcessorInfoUnix(out processorName, out clockSpeed);
            }
        }

        private static void GetProcessorInfoWindows(out string processorName, out string clockSpeed)
        {
            processorName = "Unknown CPU";
            clockSpeed = "unknown";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                using var win32Processor = new ManagementObjectSearcher("select * from Win32_Processor");
                foreach (var managementObject in win32Processor.Get())
                {
                    clockSpeed = managementObject["CurrentClockSpeed"].ToString();
                    processorName = managementObject["Name"].ToString();
                }
            }
            else
            {
                throw new PlatformNotSupportedException("Windows Management Instrumentation (WMI) queries require Windows platform");
            }
        }

        private static void GetProcessorInfoUnix(out string processorName, out string clockSpeed)
        {
            processorName = "Unknown CPU";
            clockSpeed = "unknown";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
                || RuntimeInformation.IsOSPlatform(OSPlatform.OSX)
                || RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD))
            {
                var cpuModelNameRegex = new Regex(@"^model name\s+:\s+(.+)", RegexOptions.Compiled);
                var cpuMhzRegex = new Regex(@"^cpu MHz\s+:\s+(.+)", RegexOptions.Compiled);
                string[] cpuInfoLines = File.ReadAllLines(@"/proc/cpuinfo");
                foreach (string cpuInfoLine in cpuInfoLines)
                {
                    if (cpuMhzRegex.IsMatch(cpuInfoLine))
                    {
                        string cpuMhz = cpuMhzRegex.Match(cpuInfoLine).Groups[1].Value;
                        if (double.TryParse(cpuMhz, out var cpuMhzValue) && cpuMhzValue > 0.0)
                        {
                            clockSpeed = Convert.ToInt32(Math.Round(cpuMhzValue, 0)).ToString();
                        }
                    }

                    if (cpuModelNameRegex.IsMatch(cpuInfoLine))
                    {
                        processorName = cpuModelNameRegex.Match(cpuInfoLine).Groups[1].Value;
                    }
                }
            }
            else
            {
                throw new PlatformNotSupportedException("Reading /proc/cpuinfo requires Unix-like OS");
            }
        }
    }
}
