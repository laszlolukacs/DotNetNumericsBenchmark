// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Laszlo Lukacs">
//   See LICENSE for details.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace DotNetNumericsPerfTest
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Management;
    using System.Numerics;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

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
            Console.WriteLine($"DotNetNumericsPerfTest {typeof(Options).Assembly.GetName().Version}");

            var iterations = 10000000;
            var options = new Options();
            if (CommandLine.Parser.Default.ParseArguments(args, options))
            {
                iterations = options.Iterations;
            }
            else
            {
                // after the help is displayed, exit the application
                Environment.Exit(0);
            }

            string processorName = "Unknown CPU", clockSpeed = "unknown";
            using (ManagementObjectSearcher win32Processor = new ManagementObjectSearcher("select * from Win32_Processor"))
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

            var specimenP = Program.GetSpecimenP();
            var specimenQ = Program.GetSpecimenQ();

            var arrayP = specimenP.Values.ToArray();
            var arrayQ = specimenQ.Values.ToArray();

            // performs JIT-ting
            Program.Run(arrayP, arrayQ, 2, false, null);

            // performs the actual test
            Program.Run(arrayP, arrayQ, iterations, true, output);

            File.WriteAllText(@"./DotNetNumericsPerfTest.log", output.ToString());

            Thread.Sleep(3333);
        }

        /// <summary>
        /// Runs the test calculations.
        /// </summary>
        /// <param name="array1">The array1.</param>
        /// <param name="array2">The array2.</param>
        /// <param name="iterations">The iterations.</param>
        /// <param name="measurement">if set to <c>true</c> [measurement].</param>
        /// <param name="logging">The logging.</param>
        public static void Run(double[] array1, double[] array2, int iterations, bool measurement, StringBuilder logging = null)
        {
            var currentline = string.Empty;

            var results = Program.ArrayAverage2(array1, array2);
            var resultsSimd = Program.SimdArrayAverage2(array1, array2);

            var stopwatch = new Stopwatch();

            stopwatch.Start();
            for (int i = 0; i < iterations; i++)
            {
                results = Program.ArrayAverage2(array1, array2);
            }

            stopwatch.Stop();
            if (measurement)
            {
                currentline = $"Finished with {iterations} iterations using scalar instructions: {stopwatch.ElapsedMilliseconds} ms";
                Console.WriteLine(currentline);
                logging?.AppendLine(currentline);
            }

            stopwatch.Restart();
            for (int i = 0; i < iterations; i++)
            {
                resultsSimd = Program.SimdArrayAverage2(array1, array2);
            }

            stopwatch.Stop();
            if (measurement)
            {
                currentline = $"Finished with {iterations} iterations using SIMD instructions: {stopwatch.ElapsedMilliseconds} ms";
                Console.WriteLine(currentline);
                logging?.AppendLine(currentline);
            }

            stopwatch.Restart();
            Parallel.For(0, iterations, (i, state) =>
                {
                    results = Program.ArrayAverage2(array1, array2);
                });

            stopwatch.Stop();
            if (measurement)
            {
                currentline = $"Finished with {iterations} parallel iterations using scalar instructions: {stopwatch.ElapsedMilliseconds} ms";
                Console.WriteLine(currentline);
                logging?.AppendLine(currentline);
            }

            stopwatch.Restart();
            Parallel.For(0, iterations, (i, state) =>
                {
                    resultsSimd = Program.SimdArrayAverage2(array1, array2);
                });
            stopwatch.Stop();
            if (measurement)
            {
                currentline = $"Finished with {iterations} parallel iterations using SIMD instructions: {stopwatch.ElapsedMilliseconds} ms";
                Console.WriteLine(currentline);
                logging?.AppendLine(currentline);
            }
        }

        /// <summary>
        /// Gets the average of the specified arrays using scalar calculations.
        /// </summary>
        /// <param name="lhs">Left hand side.</param>
        /// <param name="rhs">Right hand side.</param>
        /// <returns>A collection containing the average values of the specified arrays.</returns>
        public static double[] ArrayAverage2(double[] lhs, double[] rhs)
        {
            var result = new double[lhs.Length];
            var i = 0;
            for (i = 0; i < lhs.Length; i++)
            {
                result[i] = (lhs[i] + rhs[i]) / 2.0;
            }

            return result;
        }

        /// <summary>
        /// Gets the average of the specified arrays using SIMD instructions.
        /// </summary>
        /// <param name="lhs">Left hand side.</param>
        /// <param name="rhs">Right hand side.</param>
        /// <returns>A collection containing the average values of the specified arrays.</returns>
        public static double[] SimdArrayAverage2(double[] lhs, double[] rhs)
        {
            var simdLength = Vector<double>.Count;
            var result = new double[lhs.Length];
            var divider = new Vector<double>(2.0);
            var i = 0;
            for (i = 0; i <= lhs.Length - simdLength; i += simdLength)
            {
                var va = new Vector<double>(lhs, i);
                var vb = new Vector<double>(rhs, i);
                ((va + vb) / divider).CopyTo(result, i);
            }

            for (; i < lhs.Length; ++i)
            {
                result[i] = (lhs[i] + rhs[i]) / 2.0;
            }

            return result;
        }

        /// <summary>
        /// Gets the average of the specified arrays using SIMD instructions.
        /// </summary>
        /// <param name="lhs">Left hand side.</param>
        /// <param name="mhs">Middle hand side.</param>
        /// <param name="rhs">Right hand side.</param>
        /// <returns>
        /// A collection containing the average values of the specified arrays.
        /// </returns>
        public static double[] SimdArrayAverage3(double[] lhs, double[] mhs, double[] rhs)
        {
            var simdLength = Vector<double>.Count;
            var result = new double[lhs.Length];
            var divider = new Vector<double>(3.0);
            var i = 0;
            for (i = 0; i <= lhs.Length - simdLength; i += simdLength)
            {
                var va = new Vector<double>(lhs, i);
                var vb = new Vector<double>(mhs, i);
                var vc = new Vector<double>(rhs, i);
                ((va + vb + vc) / divider).CopyTo(result, i);
            }

            for (; i < lhs.Length; ++i)
            {
                result[i] = (lhs[i] + mhs[i] + rhs[i]) / 3.0;
            }

            return result;
        }

        /// <summary>
        /// Gets specimen P.
        /// </summary>
        /// <returns>A collection containing the measurements for specimen P.</returns>
        public static IDictionary<int, double> GetSpecimenP()
        {
            return new Dictionary<int, double>
            {
                { 400, 17.38 },
                { 410, (17.38 + 18.58) / 2 },
                { 420, 18.58 },
                { 430, (18.58 + 22.67) / 2 },
                { 440, 22.67 },
                { 450, (22.67 + 25.53) / 2 },
                { 460, 25.53 },
                { 470, (25.53 + 26.63) / 2 },
                { 480, 26.63 },
                { 490, (26.63 + 26.57) / 2 },
                { 500, 26.57 },
                { 510, (26.57 + 26.02) / 2 },
                { 520, 26.02 },
                { 530, (26.02 + 25.84) / 2 },
                { 540, 25.84 },
                { 550, (25.84 + 25.58) / 2 },
                { 560, 25.68 },
                { 570, (25.68 + 23.57) / 2 },
                { 580, 23.57 },
                { 590, (23.57 + 19.77) / 2 },
                { 600, 19.77 },
                { 610, (19.77 + 15.48) / 2 },
                { 620, 15.48 },
                { 630, (15.48 + 11.71) / 2 },
                { 640, 11.71 },
                { 650, (11.71 + 17.13) / 2 },
                { 660, 17.13 },
                { 670, (17.13 + 35.61) / 2 },
                { 680, 35.61 },
                { 690, (35.61 + 58.92) / 2 },
                { 700, 58.92 }
            };
        }

        /// <summary>
        /// Gets specimen Q.
        /// </summary>
        /// <returns>A collection containing the measurements for specimen Q.</returns>
        public static IDictionary<int, double> GetSpecimenQ()
        {
            return new Dictionary<int, double>
            {
                { 400, 14.63 },
                { 410, (14.63 + 16.90) / 2 },
                { 420, 16.90 },
                { 430, (16.90 + 20.68) / 2 },
                { 440, 20.68 },
                { 450, (20.68 + 26.47) / 2 },
                { 460, 26.47 },
                { 470, (26.47 + 33.03) / 2 },
                { 480, 33.03 },
                { 490, (33.03 + 33.25) / 2 },
                { 500, 33.25 },
                { 510, (33.25 + 28.69) / 2 },
                { 520, 28.69 },
                { 530, (28.69 + 24.11) / 2 },
                { 540, 24.11 },
                { 550, (24.11 + 20.97) / 2},
                { 560, 20.97 },
                { 570, (20.97 + 19.09) / 2 },
                { 580, 19.09 },
                { 590, (19.09 + 18.42) / 2 },
                { 600, 18.42 },
                { 610, (18.42 + 21.40) / 2 },
                { 620, 21.40 },
                { 630, (21.40 + 23.57) / 2 },
                { 640, 23.57 },
                { 650, (23.57 + 30.36) / 2 },
                { 660, 30.36 },
                { 670, (30.36 + 45.92) / 2 },
                { 680, 45.92 },
                { 690, (45.92 + 63.26) / 2 },
                { 700, 63.26 }
            };
        }
    }
}
