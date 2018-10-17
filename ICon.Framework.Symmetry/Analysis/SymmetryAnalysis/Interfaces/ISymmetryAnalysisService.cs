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
        /// <typeparam name="T1"></typeparam>
        /// <param name="massPoints"></param>
        /// <returns></returns>
        SymmetryIndicator GetSymmetryIndicator<T1>(IEnumerable<T1> massPoints) 
            where T1 : struct, ICartesianMassPoint3D<T1>;

        /// <summary>
        ///     Get an indicator object that can be used for approximated symmetry comparisons for mass point sets
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="massPoints"></param>
        /// <returns></returns>
        SymmetryIndicator GetSymmetryIndicatorAny<T1>(IEnumerable<T1> massPoints) 
            where T1 : struct, IMassPoint3D<T1>;

        /// <summary>
        ///     Performs an approximated symmetry comparison between two sequences of mass points and returns the comparison result
        ///     indicator (Fast, small chance of invalid result)
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="firstSet"></param>
        /// <param name="secondSet"></param>
        /// <returns></returns>
        SymmetryCompareIndicator CompareSymmetryApprox<T1>(IEnumerable<T1> firstSet, IEnumerable<T1> secondSet)
            where T1 : struct, ICartesianMassPoint3D<T1>;

        /// <summary>
        ///     Performs an approximated symmetry comparison between two sets of mass points in arbitrary coordinate systems
        ///     (Expensive due to polymorphic system transformation)
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="firstSet"></param>
        /// <param name="secondSet"></param>
        /// <returns></returns>
        SymmetryCompareIndicator CompareSymmetryApproxAny<T1, T2>(IEnumerable<T1> firstSet, IEnumerable<T2> secondSet)
            where T1 : struct, IMassPoint3D<T1> where T2 : struct, IMassPoint3D<T2>;

        /// <summary>
        ///     Performs an exact symmetry comparison between two sequences of vectors using the provided comparer for individual
        ///     comparison of the vectors (Slow but exact)
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="firstSet"></param>
        /// <param name="secondSet"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        bool CompareSymmetryExact<T1>(IEnumerable<T1> firstSet, IEnumerable<T1> secondSet, IComparer<T1> comparer)
            where T1 : struct, ICartesian3D<T1>;

        /// <summary>
        ///     Tries to transform the first passed set to the second and calculates the affiliated transformation matrix sequence
        ///     (Returns false if not possible)
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="comparer"></param>
        /// <param name="matrix"></param>
        /// <returns></returns>
        bool TryGetTransformationSequence<T1>(IEnumerable<T1> source, IEnumerable<T1> target, IComparer<T1> comparer,
            out IEnumerable<TransformMatrix2D> matrix) where T1 : struct, IVector3D<T1>;
    }
}