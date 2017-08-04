// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Options.cs" company="Laszlo Lukacs">
//   See LICENSE for details.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace DotNetNumericsPerfTest
{
    using System.Text;
    using CommandLine;

    /// <summary>
    /// Contains the available command line options.
    /// </summary>
    public class Options
    {
        /// <summary>
        /// Gets or sets the number of test iterations.
        /// </summary>
        [Option('i', "iterations", DefaultValue = 10000000, HelpText = "Number of calculation iterations to run. Default value is 10000000.")]
        public int Iterations { get; set; }

        /// <summary>
        /// Gets the usage.
        /// </summary>
        /// <returns>The help message on usege.</returns>
        [HelpOption]
        public string GetUsage()
        {
            // this without using CommandLine.Text
            //  or using HelpText.AutoBuild
            var usage = new StringBuilder();
            usage.AppendLine("\nUsage:");
            usage.AppendLine("\tDotNetNumerics [-i Iterations]\n");
            usage.AppendLine("where");
            usage.AppendLine("\tIterations\tThe number of calculation iterations to run. Default value is 10000000.\n");
            return usage.ToString();
        }
    }
}
