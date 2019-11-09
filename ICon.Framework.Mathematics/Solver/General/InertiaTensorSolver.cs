using System;
using System.Collections.Generic;
using Mocassin.Mathematics.Extensions;

namespace Mocassin.Mathematics.Solver
{
    /// <summary>
    ///     Calculates the principal inertia tensor by the Oliver K. Smith trigonometric substitution
    /// </summary>
    public class InertiaTensorSolver
    {
        /// <summary>
        ///     Solves eigenvalue problem of the generic inertia tensor and returns the diagonal entries of the principal inertia
        ///     tensor
        /// </summary>
        /// <param name="tensor"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public double[] GetPrincipalTensorEntries(double[,] tensor, IComparer<double> comparer)
        {
            if (tensor.GetUpperBound(0) != 2 || tensor.GetUpperBound(1) != 2)
                throw new ArgumentException("Input tensor is not of size 3x3", nameof(tensor));

            if (!tensor.IsSymmetric(comparer))
                throw new ArgumentException("Input tensor is not symmetric", nameof(tensor));

            return CalculateEigenvalues(tensor, comparer);
        }

        /// <summary>
        ///     Get the length of the principal axis inertia tensor diagonal vector
        /// </summary>
        /// <param name="tensor"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public double GetPrincipalTensorLength(double[,] tensor, IComparer<double> comparer)
        {
            var entries = GetPrincipalTensorEntries(tensor, comparer);
            return Math.Sqrt(entries[0] * entries[0] + entries[1] * entries[1] + entries[2] * entries[2]);
        }

        /// <summary>
        ///     Uses trigonometric substitution to solve 3x3 symmetric matrix eigenvalue problem
        /// </summary>
        /// <param name="tensor"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        private static double[] CalculateEigenvalues(double[,] tensor, IComparer<double> comparer)
        {
            var intValues = new[] {tensor[0, 0], tensor[1, 1], tensor[2, 2]};

            var devQuad = Quad(tensor[0, 1]) + Quad(tensor[0, 2]) + Quad(tensor[1, 2]);
            if (comparer.Compare(devQuad, 0.0) == 0)
            {
                Array.Sort(intValues);
                return intValues;
            }

            var devValues = new[] {0.0, 0.0, 0.0};
            double intAngle;
            var intTrace = Trace(tensor) / 3.0;
            var intQuad = Quad(tensor[0, 0] - intTrace) + Quad(tensor[1, 1] - intTrace) + Quad(tensor[2, 2] - intTrace) + 2.0 * devQuad;
            var intQuadRoot = Math.Sqrt(intQuad / 6.0);
            var intQuadRootInverse = 1.0 / intQuadRoot;

            intValues[0] = (tensor[0, 0] - intTrace) * intQuadRootInverse;
            intValues[1] = (tensor[1, 1] - intTrace) * intQuadRootInverse;
            intValues[2] = (tensor[2, 2] - intTrace) * intQuadRootInverse;
            devValues[0] = tensor[0, 1] * intQuadRootInverse;
            devValues[1] = tensor[0, 2] * intQuadRootInverse;
            devValues[2] = tensor[1, 2] * intQuadRootInverse;
            var halfDeterminant = Determinant(intValues, devValues) / 2.0;

            if (halfDeterminant < -1.0 || comparer.Compare(halfDeterminant, -1.0) == 0)
                intAngle = Math.PI / 3.0;
            else if (halfDeterminant > 1.0 || comparer.Compare(halfDeterminant, 1.0) == 0)
                intAngle = 0.0;
            else
                intAngle = Math.Acos(halfDeterminant) / 3.0;

            intValues[0] = intTrace + 2.0 * intQuadRoot * Math.Cos(intAngle);
            intValues[2] = intTrace + 2.0 * intQuadRoot * Math.Cos(intAngle + 2.0 * Math.PI / 3.0);
            intValues[1] = intTrace * 3.0 - intValues[0] - intValues[2];

            return intValues;
        }

        /// <summary>
        ///     Calculates the determinant of a 3x3 symmetric array from the diagonal entries and the outer values
        /// </summary>
        /// <param name="diagonal"></param>
        /// <param name="outer"></param>
        /// <returns></returns>
        private static double Determinant(IReadOnlyList<double> diagonal, IReadOnlyList<double> outer)
        {
            return diagonal[0] * (diagonal[1] * diagonal[2] - outer[2] * outer[2])
                   - outer[0] * (outer[0] * diagonal[2] - outer[1] * outer[2])
                   + outer[1] * (outer[0] * outer[2] - outer[1] * diagonal[1]);
        }

        /// <summary>
        ///     Calculates the trace of a 3x3 double array
        /// </summary>
        /// <param name="tensor"></param>
        /// <returns></returns>
        private static double Trace(double[,] tensor)
        {
            return tensor[0, 0] + tensor[1, 1] + tensor[2, 2];
        }

        /// <summary>
        ///     Quadratic of a double value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static double Quad(double value)
        {
            return value * value;
        }
    }
}