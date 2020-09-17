using System.Collections.Generic;
using Mocassin.Mathematics.ValueTypes;

namespace Mocassin.Symmetry.Analysis
{
    /// <summary>
    ///     Symmetry analysis service that supports comparisons of groups of mass points in cartesian coordinates (e.g. cell
    ///     positions, atoms,...)
    /// </summary>
    public interface ISymmetryAnalysisService
    {
        /// <summary>
        ///     The comparer for symmetry indicators used within the approximated symmetry comparisons
        /// </summary>
        IComparer<SymmetryIndicator> IndicatorComparer { get; }

        /// <summary>
        ///     Get an indicator object that can be used for approximated symmetry comparisons for cartesian mass point sets
        /// </summary>
        /// <param name="massPoints"></param>
        /// <returns></returns>
        SymmetryIndicator GetSymmetryIndicator(IEnumerable<CartesianMassPoint3D> massPoints);

        /// <summary>
        ///     Performs an approximated symmetry comparison between two sequences of mass points and returns the comparison result
        ///     indicator (Fast, small chance of invalid result)
        /// </summary>
        /// <param name="firstSet"></param>
        /// <param name="secondSet"></param>
        /// <returns></returns>
        SymmetryCompareIndicator CompareSymmetryApprox(IEnumerable<CartesianMassPoint3D> firstSet,
            IEnumerable<CartesianMassPoint3D> secondSet);
    }
}