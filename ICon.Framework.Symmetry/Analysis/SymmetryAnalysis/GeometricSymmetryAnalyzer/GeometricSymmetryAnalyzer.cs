using System;
using System.Collections.Generic;
using System.Linq;

using ICon.Mathematics.Coordinates;
using ICon.Mathematics.ValueTypes;
using ICon.Mathematics.Solver;
using ICon.Framework.Extensions;

namespace ICon.Symmetry.Analysis
{
    /// <summary>
    /// Symmetry analyzer that compares mass point groups based upon geometric properties
    /// </summary>
    public class GeometricSymmetryAnalyzer
    {
        /// <summary>
        /// Takes a sequence of mass points and calculates the symmetry indicator value
        /// </summary>
        /// <param name="massPoints"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public SymmetryIndicator GetSymmetryIndicator<T1>(IEnumerable<T1> massPoints, IComparer<double> comparer) where T1 : struct, ICartesianMassPoint3D<T1>
        {
            return GetSymmetryIndicator(new PointMechanicsSolver().CreateGeometryInfo(massPoints, comparer), comparer);
        }

        /// <summary>
        /// Calculates symmetry indicator form geometry info object
        /// </summary>
        /// <param name="info"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public SymmetryIndicator GetSymmetryIndicator(MassPointGeomertyInfo info, IComparer<double> comparer)
        {
            if (info == null)
            {
                throw new ArgumentNullException(nameof(info));
            }
            if (comparer == null)
            {
                throw new ArgumentNullException(nameof(comparer));
            }

            double tensorLength = new InertiaTensorSolver().GetPrinipalTensorLength(info.MassCenterInertiaTensor.Values, comparer.ToEqualityComparer());

            double first = GetFirstHashValue(tensorLength, info.SumOfMassTimesDistance, comparer);
            double second = GetSecondHashValue(info.TotalMass, info.PointCount, comparer);
            return new SymmetryIndicator(first, second);
        }

        /// <summary>
        /// Get the first hash value from the tensor length and torsional moment. Corrects almost equal zero values to zero
        /// </summary>
        /// <param name="tensorLength"></param>
        /// <param name="torsionalMoment"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        protected double GetFirstHashValue(double tensorLength, double torsionalMoment, IComparer<double> comparer)
        {
            double combined = tensorLength * torsionalMoment;
            combined = (comparer.Compare(0.0, combined) == 0) ? 0.0 : combined;
            return combined;
        }

        /// <summary>
        /// Get the second hash vale from total mass and point count. Corrects almost equal zero values to zero
        /// </summary>
        /// <param name="totalMass"></param>
        /// <param name="pointCount"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        protected double GetSecondHashValue(double totalMass, double pointCount, IComparer<double> comparer)
        {
            double combined = totalMass * pointCount;
            combined = (comparer.Compare(0.0, combined) == 0) ? 0.0 : combined;
            return combined;
        }
    }
}
