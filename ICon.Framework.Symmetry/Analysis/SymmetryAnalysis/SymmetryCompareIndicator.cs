using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Mocassin.Symmetry.Analysis
{
    /// <summary>
    ///     Contains the result of an approximated symmetry comparison between two geometric objects
    /// </summary>
    public readonly struct SymmetryCompareIndicator
    {
        /// <summary>
        ///     Flag that indicates if the approximated symmetry comparisons indicates equal symmetry
        /// </summary>
        private bool IsSame { get; }

        /// <summary>
        ///     The symmetry indicator of the first analyzed geometric object
        /// </summary>
        private SymmetryIndicator FirstIndicator { get; }

        /// <summary>
        ///     The symmetry indicator of the second analyzed geometric object
        /// </summary>
        private SymmetryIndicator SecondIndicator { get; }

        /// <summary>
        ///     Create ney symmetry comparison object with flag and the two indicators
        /// </summary>
        /// <param name="isSame"></param>
        /// <param name="firstIndicator"></param>
        /// <param name="secondIndicator"></param>
        public SymmetryCompareIndicator(bool isSame, SymmetryIndicator firstIndicator, SymmetryIndicator secondIndicator)
            : this()
        {
            IsSame = isSame;
            FirstIndicator = firstIndicator;
            SecondIndicator = secondIndicator;
        }

        /// <summary>
        ///     Creates new symmetry comparison object with two indicators and a comparer to automatically set the equality flag
        /// </summary>
        /// <param name="firstIndicator"></param>
        /// <param name="secondIndicator"></param>
        /// <param name="comparer"></param>
        public SymmetryCompareIndicator(SymmetryIndicator firstIndicator, SymmetryIndicator secondIndicator,
            IComparer<SymmetryIndicator> comparer)
            : this()
        {
            FirstIndicator = firstIndicator;
            SecondIndicator = secondIndicator;
            IsSame = CheckEquivalency(comparer);
        }

        /// <summary>
        ///     Checks for equivalency of the currently set values, sets the flag to false if both objects contain only zeros
        /// </summary>
        /// <param name="comparer"></param>
        /// <returns></returns>
        [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
        public bool CheckEquivalency(IComparer<SymmetryIndicator> comparer)
        {
            if (FirstIndicator.First == 0 && FirstIndicator.Second == 0 && SecondIndicator.First == 0 &&
                SecondIndicator.Second == 0)
                return false;

            return comparer.Compare(FirstIndicator, SecondIndicator) == 0;
        }
    }
}