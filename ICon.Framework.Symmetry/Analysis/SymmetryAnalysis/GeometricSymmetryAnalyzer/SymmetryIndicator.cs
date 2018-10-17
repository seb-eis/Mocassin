using System;
using System.Collections.Generic;
using System.Text;

using Mocassin.Framework.Collections;
using Mocassin.Mathematics.Extensions;

namespace Mocassin.Symmetry.Analysis
{
    /// <summary>
    /// Geometry hash object that is used to do an approximated symmetry comparisons between groups of mass points based on hash values
    /// </summary>
    public readonly struct SymmetryIndicator
    {
        /// <summary>
        /// The first part of the hash value for almost equal comparisons
        /// </summary>
        public double First { get; }

        /// <summary>
        /// The second part of the hash value for almost equal comparisons
        /// </summary>
        public double Second { get; }

        /// <summary>
        /// Creates a new symmetry indicator from two symmetry indicator values
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        public SymmetryIndicator(double first, double second) : this()
        {
            First = first;
            Second = second;
        }

        /// <summary>
        /// Overwrites objects ToString() method
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"Symmetry Indicator ({First}, {Second})";
        }

        /// <summary>
        /// Creates a compare adapter object for the indicator from a double comparer interface
        /// </summary>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public static Comparer<SymmetryIndicator> MakeComparer(IComparer<double> comparer)
        {
            int Compare(SymmetryIndicator lhs, SymmetryIndicator rhs)
            {
                int first = comparer.Compare(lhs.First, rhs.First);
                if (first == 0)
                {
                    return comparer.Compare(lhs.Second, rhs.Second);
                }
                return first;
            }
            return Comparer<SymmetryIndicator>.Create(Compare);
        }
    }
}
