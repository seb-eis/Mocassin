using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Framework.Extensions
{
    /// <summary>
    /// ICon shared array extension class for generic array extensions methods
    /// </summary>
    public static class IConArrayExtensions
    {
        /// <summary>
        /// Gets the dimensions of a 2D array as value tuple
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="array"></param>
        /// <returns></returns>
        public static (Int32 Rows, Int32 Cols) GetDimensions<T1>(this T1[,] array)
        {
            return (array != null) ? (array.GetUpperBound(0) + 1, array.GetUpperBound(1) + 1) : throw new ArgumentNullException(nameof(array));
        }
    }
}
