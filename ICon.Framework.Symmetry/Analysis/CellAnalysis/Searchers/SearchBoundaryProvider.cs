using System;
using System.Collections.Generic;
using System.Text;

using ICon.Mathematics.ValueTypes;
using ICon.Mathematics.Extensions;

namespace ICon.Symmetry.Analysis
{
    /// <summary>
    /// Contains boundary distance infomation for a unit cell (Hess norm distamces to plains AB, AC, BC) and distances between the plains
    /// </summary>
    public class SearchBoundaryProvider
    {
        /// <summary>
        /// Current distance to next AB plain
        /// </summary>
        public double DistanceToPlainAB { get; protected set; }

        /// <summary>
        /// Current distance to next AC plain
        /// </summary>
        public double DistanceToPlainAC { get; protected set; }

        /// <summary>
        /// Current distance to next BC plain
        /// </summary>
        public double DistanceToPlainBC { get; protected set; }

        /// <summary>
        /// Distance to between two AB plains
        /// </summary>
        public double PlainToPlainAB { get; protected set; }

        /// <summary>
        /// Distance between two AC plain
        /// </summary>
        public double PlainToPlainAC { get; protected set; }

        /// <summary>
        /// Distance between two BC plains
        /// </summary>
        public double PlainToPlainBC { get; protected set; }

        /// <summary>
        /// Create new boundary infor for provided start and base vectors of the unit cell
        /// </summary>
        /// <param name="start"></param>
        /// <param name="baseVectors"></param>
        public SearchBoundaryProvider(in Cartesian3D start, in (Cartesian3D A, Cartesian3D B, Cartesian3D C) baseVectors)
        {
            CalculateDistances(start, baseVectors);
        }

        /// <summary>
        /// Increase the distance to all plains by the specififed number of steps
        /// </summary>
        public void ChangeAllDistances(int steps)
        {
            ChangeDistanceToAB(steps);
            ChangeDistanceToAC(steps);
            ChangeDistanceToBC(steps);
        }

        /// <summary>
        /// Change the distance to plain AB by the specififed number of steps
        /// </summary>
        /// <param name="steps"></param>
        public void ChangeDistanceToAB(int steps)
        {
            DistanceToPlainAB += steps * PlainToPlainAB;
        }

        /// <summary>
        /// Change the distance to plain AC by the specififed number of steps
        /// </summary>
        /// <param name="steps"></param>
        public void ChangeDistanceToAC(int steps)
        {
            DistanceToPlainAC += steps * PlainToPlainAC;
        }

        /// <summary>
        /// Change the distance to plain BC by the specififed number of steps
        /// </summary>
        /// <param name="steps"></param>
        public void ChangeDistanceToBC(int steps)
        {
            DistanceToPlainBC += steps * PlainToPlainBC;
        }

        /// <summary>
        /// Checks if a radial length values is within the limitations of the current boundaries
        /// </summary>
        /// <param name="distance"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public bool DistanceWithinBoundaries(double distance, IComparer<double> comparer)
        {
            return comparer.Compare(distance, DistanceToPlainAB) <= 0
                && comparer.Compare(distance, DistanceToPlainAC) <= 0
                && comparer.Compare(distance, DistanceToPlainBC) <= 0;
        }

        /// <summary>
        /// Calculates the distance information from start and base vectors
        /// </summary>
        /// <param name="start"></param>
        /// <param name="baseVectors"></param>
        public void CalculateDistances(in Cartesian3D start, in (Cartesian3D A, Cartesian3D B, Cartesian3D C) baseVectors)
        {
            Cartesian3D normVectorToPlainAB = baseVectors.A.GetCrossProduct(baseVectors.B).GetNormalized();
            Cartesian3D normVectorToPlainAC = baseVectors.A.GetCrossProduct(baseVectors.C).GetNormalized();
            Cartesian3D normVectorToPlainBC = baseVectors.B.GetCrossProduct(baseVectors.C).GetNormalized();

            DistanceToPlainAB = Math.Abs(start * normVectorToPlainAB);
            DistanceToPlainAC = Math.Abs(start * normVectorToPlainAC);
            DistanceToPlainBC = Math.Abs(start * normVectorToPlainBC);

            DistanceToPlainAB = Math.Min(Math.Abs(baseVectors.C * normVectorToPlainAB - DistanceToPlainAB), DistanceToPlainAB);
            DistanceToPlainAC = Math.Min(Math.Abs(baseVectors.B * normVectorToPlainAC - DistanceToPlainAC), DistanceToPlainAC);
            DistanceToPlainBC = Math.Min(Math.Abs(baseVectors.A * normVectorToPlainBC - DistanceToPlainBC), DistanceToPlainBC);

            PlainToPlainAB = baseVectors.C.GetLength();
            PlainToPlainAC = baseVectors.B.GetLength();
            PlainToPlainBC = baseVectors.A.GetLength();
        }
    }
}
