// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Options.cs" company="Laszlo Lukacs">
//   See LICENSE for details.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using CommandLine;

namespace DotNetNumericsBenchmark
{
    /// <summary>
    /// Contains the available command line options.
    /// </summary>
    public class Options
    {
        /// <summary>
        /// Gets or sets the number of test iterations.
        /// </summary>
        [Option('i', "iterations", Default = 10000000, HelpText = "Number of calculation iterations to run. Default value is 10000000.")]
        public int Iterations { get; set; }
    }
}
