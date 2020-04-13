// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ArrayAverageCalculator.cs" company="Laszlo Lukacs">
//   See LICENSE for details.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Numerics;

namespace DotNetNumericsBenchmark
{
    public static class ArrayAverageCalculator
    {
        /// <summary>
        /// Gets the average of the specified arrays using scalar calculations.
        /// </summary>
        /// <param name="lhs">Left hand side.</param>
        /// <param name="rhs">Right hand side.</param>
        /// <returns>A collection containing the average values of the specified arrays.</returns>
        public static double[] Average2Scalar(double[] lhs, double[] rhs)
        {
            var result = new double[lhs.Length];
            int i;
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
        public static double[] Average2Simd(double[] lhs, double[] rhs)
        {
            var simdLength = Vector<double>.Count;
            var result = new double[lhs.Length];
            var divider = new Vector<double>(2.0);
            int i;
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
        /// Gets the average of the specified 3 arrays using SIMD instructions.
        /// </summary>
        /// <param name="lhs">Left hand side.</param>
        /// <param name="mhs">Middle hand side.</param>
        /// <param name="rhs">Right hand side.</param>
        /// <returns>
        /// A collection containing the average values of the specified arrays.
        /// </returns>
        public static double[] Average3Simd(double[] lhs, double[] mhs, double[] rhs)
        {
            var simdLength = Vector<double>.Count;
            var result = new double[lhs.Length];
            var divider = new Vector<double>(3.0);
            int i;
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
    }
}
