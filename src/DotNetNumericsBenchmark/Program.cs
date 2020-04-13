// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Laszlo Lukacs">
//   See LICENSE for details.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Management;
using System.Numerics;
using System.Text;
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
            Console.WriteLine($"DotNetNumericsPerfTest {typeof(Program).Assembly.GetName().Version}");

            string processorName = "Unknown CPU", clockSpeed = "unknown";
            using (var win32Processor = new ManagementObjectSearcher("select * from Win32_Processor"))
            {
                foreach (var managementObject in win32Processor.Get())
                {
                    clockSpeed = managementObject["CurrentClockSpeed"].ToString();
                    processorName = managementObject["Name"].ToString();
                }
            }

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
    }
}
