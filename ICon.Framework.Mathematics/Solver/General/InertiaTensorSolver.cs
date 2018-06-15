using System;
using System.Collections.Generic;

using ICon.Mathematics.Comparers;
using ICon.Mathematics.Extensions;

namespace ICon.Mathematics.Solver
{
    /// <summary>
    /// Calculates the pricipal inertia tensor by the Oliver K. Smith trigonometric substitution
    /// </summary>
    public class InertiaTensorSolver
    {
        /// <summary>
        /// Solves eigenvalue problem of the generic inertia tensor and returns the diagonal entries of the principal inertia tensor
        /// </summary>
        /// <param name="tensor"></param>
        /// <returns></returns>
        public Double[] GetPrincipalTensorEntries(Double[,] tensor, IEqualityComparer<Double> comparer)
        {
            if (tensor.GetUpperBound(0) != 2 || tensor.GetUpperBound(1) != 2)
            {
                throw new ArgumentException(paramName: nameof(tensor), message: "Input tensor is not of size 3x3");
            }
            if (tensor.IsSymmetric(comparer) == false)
            {
                throw new ArgumentException(paramName: nameof(tensor), message: "Input tensor is not symmetric");
            }

            return CalculateEigenvalues(tensor, comparer);
        }

        /// <summary>
        /// Get the length of the principal axis inertia tensor diagonal vector
        /// </summary>
        /// <param name="tensor"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public double GetPrinipalTensorLength(double[,] tensor, IEqualityComparer<Double> comparer)
        {
            var entries = GetPrincipalTensorEntries(tensor, comparer);
            return Math.Sqrt(entries[0] * entries[0] + entries[1] * entries[1] + entries[2] * entries[2]);
        }

        /// <summary>
        /// Uses trigonemetric substitution to solve 3x3 symmetric matrix eigenvalue problem
        /// </summary>
        /// <param name="tensor"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        private Double[] CalculateEigenvalues(Double[,] tensor, IEqualityComparer<Double> comparer)
        {
            Double[] intValues = new Double[3] { tensor[0, 0], tensor[1, 1], tensor[2, 2] };

            Double devQuad = Quad(tensor[0, 1]) + Quad(tensor[0, 2]) + Quad(tensor[1, 2]);
            if (comparer.Equals(devQuad, 0.0))
            {
                Array.Sort(intValues);
                return intValues;
            }

            Double[] devValues = new Double[3] { 0.0, 0.0, 0.0 };
            Double intAngle = 0.0;
            Double intTrace = Trace(tensor) / 3.0;
            Double intQuad = Quad(tensor[0, 0] - intTrace) + Quad(tensor[1, 1] - intTrace) + Quad(tensor[2, 2] - intTrace) + 2.0 * devQuad;
            Double intQuadRoot = Math.Sqrt(intQuad / 6.0);
            Double intQuadRootInvese = 1.0 / intQuadRoot;

            intValues[0] = (tensor[0, 0] - intTrace) * intQuadRootInvese;
            intValues[1] = (tensor[1, 1] - intTrace) * intQuadRootInvese;
            intValues[2] = (tensor[2, 2] - intTrace) * intQuadRootInvese;
            devValues[0] = tensor[0, 1] * intQuadRootInvese;
            devValues[1] = tensor[0, 2] * intQuadRootInvese;
            devValues[2] = tensor[1, 2] * intQuadRootInvese;
            Double halfDeterminant = Determinant(intValues, devValues) / 2.0;

            if (halfDeterminant < -1.0 || comparer.Equals(halfDeterminant, -1.0))
            {
                intAngle = Math.PI / 3.0;
            }
            else if (halfDeterminant > 1.0 || comparer.Equals(halfDeterminant, 1.0))
            {
                intAngle = 0.0;
            }
            else
            {
                intAngle = Math.Acos(halfDeterminant) / 3.0;
            }

            intValues[0] = intTrace + (2.0 * intQuadRoot * Math.Cos(intAngle));
            intValues[2] = intTrace + (2.0 * intQuadRoot * Math.Cos(intAngle + (2.0 * Math.PI / 3.0)));
            intValues[1] = intTrace * 3.0 - intValues[0] - intValues[2];

            return intValues;
        }

        /// <summary>
        /// Calculates the determinant of a 3x3 double array
        /// </summary>
        /// <param name="tensor"></param>
        /// <returns></returns>
        private Double Determinant(Double[,] tensor)
        {
            return tensor[0, 0] * (tensor[1, 1] * tensor[2, 2] - tensor[1, 2] * tensor[2, 1])
                - tensor[0, 1] * (tensor[1, 0] * tensor[2, 2] - tensor[2, 0] * tensor[1, 2])
                + tensor[0, 2] * (tensor[1, 0] * tensor[2, 1] - tensor[2, 0] * tensor[1, 1]);
        }

        /// <summary>
        /// Calculates the determinant of a 3x3 symmetric array from the diagonal entries and the outer values
        /// </summary>
        /// <param name="tensor"></param>
        /// <returns></returns>
        private Double Determinant(Double[] diagonal, Double[] outer)
        {
            return diagonal[0] * (diagonal[1] * diagonal[2] - outer[2] * outer[2])
                - outer[0] * (outer[0] * diagonal[2] - outer[1] * outer[2])
                + outer[1] * (outer[0] * outer[2] - outer[1] * diagonal[1]);
        }

        /// <summary>
        /// Calculates the trace of a 3x3 double array
        /// </summary>
        /// <param name="tensor"></param>
        /// <returns></returns>
        private Double Trace(Double[,] tensor)
        {
            return tensor[0, 0] + tensor[1, 1] + tensor[2, 2];
        }

        /// <summary>
        /// Quadratic of a double value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private Double Quad(Double value)
        {
            return value * value;
        }
    }
}
