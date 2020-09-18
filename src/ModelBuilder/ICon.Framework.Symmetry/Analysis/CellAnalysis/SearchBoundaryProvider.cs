using System;
using System.Collections.Generic;
using Mocassin.Mathematics.Extensions;
using Mocassin.Mathematics.ValueTypes;

namespace Mocassin.Symmetry.Analysis
{
    /// <summary>
    ///     Contains boundary distance information for a unit cell (Hess norm distances to plains AB, AC, BC) and distances
    ///     between the plains
    /// </summary>
    public class SearchBoundaryProvider
    {
        /// <summary>
        ///     Current distance to next AB plain
        /// </summary>
        public double DistanceToAbPlain { get; protected set; }

        /// <summary>
        ///     Current distance to next AC plain
        /// </summary>
        public double DistanceToAcPlain { get; protected set; }

        /// <summary>
        ///     Current distance to next BC plain
        /// </summary>
        public double DistanceToBcPlain { get; protected set; }

        /// <summary>
        ///     Distance to between two AB plains
        /// </summary>
        public double PlainToPlainDistanceAb { get; protected set; }

        /// <summary>
        ///     Distance between two AC plain
        /// </summary>
        public double PlainToPlainDistanceAc { get; protected set; }

        /// <summary>
        ///     Distance between two BC plains
        /// </summary>
        public double PlainToPlainDistanceBc { get; protected set; }

        /// <summary>
        ///     Create new boundary info for provided start and base vectors of the unit cell
        /// </summary>
        /// <param name="start"></param>
        /// <param name="baseVectors"></param>
        public SearchBoundaryProvider(in Cartesian3D start, in (Cartesian3D A, Cartesian3D B, Cartesian3D C) baseVectors)
        {
            CalculateDistances(start, baseVectors);
        }

        /// <summary>
        ///     Increase the distance to all plains by the specified number of steps
        /// </summary>
        public void ChangeAllDistances(int steps)
        {
            ChangeDistanceToAbPlain(steps);
            ChangeDistanceToAcPlain(steps);
            ChangeDistanceToBcPlain(steps);
        }

        /// <summary>
        ///     Change the distance to plain AB by the specified number of steps
        /// </summary>
        /// <param name="steps"></param>
        public void ChangeDistanceToAbPlain(int steps)
        {
            DistanceToAbPlain += steps * PlainToPlainDistanceAb;
        }

        /// <summary>
        ///     Change the distance to plain AC by the specified number of steps
        /// </summary>
        /// <param name="steps"></param>
        public void ChangeDistanceToAcPlain(int steps)
        {
            DistanceToAcPlain += steps * PlainToPlainDistanceAc;
        }

        /// <summary>
        ///     Change the distance to plain BC by the specified number of steps
        /// </summary>
        /// <param name="steps"></param>
        public void ChangeDistanceToBcPlain(int steps)
        {
            DistanceToBcPlain += steps * PlainToPlainDistanceBc;
        }

        /// <summary>
        ///     Checks if a radial length values is within the limitations of the current boundaries
        /// </summary>
        /// <param name="distance"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public bool DistanceWithinBoundaries(double distance, IComparer<double> comparer) =>
            comparer.Compare(distance, DistanceToAbPlain) <= 0
            && comparer.Compare(distance, DistanceToAcPlain) <= 0
            && comparer.Compare(distance, DistanceToBcPlain) <= 0;

        /// <summary>
        ///     Calculates the distance information from start and base vectors
        /// </summary>
        /// <param name="start"></param>
        /// <param name="baseVectors"></param>
        public void CalculateDistances(in Cartesian3D start, in (Cartesian3D A, Cartesian3D B, Cartesian3D C) baseVectors)
        {
            var normVectorToPlainAb = baseVectors.A.GetCrossProduct(baseVectors.B).GetNormalized();
            var normVectorToPlainAc = baseVectors.A.GetCrossProduct(baseVectors.C).GetNormalized();
            var normVectorToPlainBc = baseVectors.B.GetCrossProduct(baseVectors.C).GetNormalized();

            DistanceToAbPlain = Math.Abs(start * normVectorToPlainAb);
            DistanceToAcPlain = Math.Abs(start * normVectorToPlainAc);
            DistanceToBcPlain = Math.Abs(start * normVectorToPlainBc);

            DistanceToAbPlain = Math.Min(Math.Abs(baseVectors.C * normVectorToPlainAb - DistanceToAbPlain), DistanceToAbPlain);
            DistanceToAcPlain = Math.Min(Math.Abs(baseVectors.B * normVectorToPlainAc - DistanceToAcPlain), DistanceToAcPlain);
            DistanceToBcPlain = Math.Min(Math.Abs(baseVectors.A * normVectorToPlainBc - DistanceToBcPlain), DistanceToBcPlain);

            PlainToPlainDistanceAb = baseVectors.C.GetLength();
            PlainToPlainDistanceAc = baseVectors.B.GetLength();
            PlainToPlainDistanceBc = baseVectors.A.GetLength();
        }
    }
}