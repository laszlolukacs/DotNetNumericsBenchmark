// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScalarVsSimdArrayAverageBenchmark.cs" company="Laszlo Lukacs">
//   See LICENSE for details.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace DotNetNumericsBenchmark
{
    [SimpleJob(RuntimeMoniker.NetCoreApp31)]
    public class ScalarVsSimdArrayAverageBenchmark
    {
        private readonly double[] arrayA;
        private readonly double[] arrayB;

        public ScalarVsSimdArrayAverageBenchmark()
        {
            this.arrayA = new[]
            {
                17.38, 17.98, 18.58, 20.63, 22.67, 24.10, 25.53, 26.08, 26.63, 26.60, 26.57, 26.30, 26.02, 25.93, 25.84,
                25.71, 25.68, 24.63, 23.57, 21.72, 19.77, 17.63, 15.48, 13.60, 11.71, 14.42, 17.13, 26.37, 35.61, 47.27,
                58.92
            };

            this.arrayB = new[]
            {
                14.63, 15.77, 16.90, 18.79, 20.68, 23.58, 26.47, 29.75, 33.03, 33.14, 33.25, 30.97, 28.69, 26.40, 24.11,
                22.54, 20.97, 20.03, 19.09, 18.76, 18.42, 19.91, 21.40, 22.40, 23.57, 26.97, 30.36, 38.14, 45.92, 54.59,
                63.26
            };
        }

        [Benchmark]
        public double[] ArrayAverageScalar() => ArrayAverageCalculator.Average2Scalar(arrayA, arrayB);

        [Benchmark]
        public double[] ArrayAverageSimd() => ArrayAverageCalculator.Average2Simd(arrayA, arrayB);
    }
}
