using System;
using System.Collections.Generic;

namespace Mocassin.Framework.Extensions
{
    /// <summary>
    ///     ICon shared array extension class for generic array extensions methods
    /// </summary>
    public static class MocassinArrayExtensions
    {
        /// <summary>
        ///     Gets the dimensions of a 2D array as value tuple
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="array"></param>
        /// <returns></returns>
        public static (int Rows, int Cols) GetDimensions<T1>(this T1[,] array) =>
            array != null
                ? (array.GetUpperBound(0) + 1, array.GetUpperBound(1) + 1)
                : throw new ArgumentNullException(nameof(array));

        /// <summary>
        ///     Populates an array with a default value
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="values"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T1[] Populate<T1>(this T1[] values, T1 value)
        {
            for (var i = 0; i < values.Length; i++)
                values[i] = value;

            return values;
        }

        /// <summary>
        ///     Populates an integer array by a continuous counter starting at te provided index
        /// </summary>
        /// <param name="values"></param>
        /// <param name="start"></param>
        /// <returns></returns>
        public static int[] PopulateByCounter(this int[] values, int start)
        {
            for (var i = 0; i < values.Length; i++)
                values[i] = i + start;

            return values;
        }

        /// <summary>
        ///     Populates a one dimensional array with values from an enumerable sequence
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="target"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static T1[] Populate<T1>(this T1[] target, IEnumerable<T1> values)
        {
            var index = -1;
            foreach (var item in values)
                target[++index] = item;

            return target;
        }

        /// <summary>
        ///     Populates a one dimensional array with values from a provider delegate
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="target"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static T1[] Populate<T1>(this T1[] target, Func<T1> provider)
        {
            for (var i = 0; i < target.Length; i++)
                target[i] = provider();

            return target;
        }

        /// <summary>
        ///     Populates a two dimensional array with values from an enumerable sequence
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="target"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static T1[,] Populate<T1>(this T1[,] target, IEnumerable<T1> values)
        {
            var (rows, cols) = target.GetDimensions();

            using (var enumerator = values.GetEnumerator())
            {
                for (var i = 0; i < rows; i++)
                {
                    for (var j = 0; j < cols; j++)
                    {
                        enumerator.MoveNext();
                        target[i, j] = enumerator.Current;
                    }
                }
            }

            return target;
        }

        /// <summary>
        ///     Populates a two dimensional array with values from a provider delegate
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="target"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static T1[,] Populate<T1>(this T1[,] target, Func<T1> provider)
        {
            var (rows, cols) = target.GetDimensions();
            for (var i = 0; i < rows; i++)
            {
                for (var j = 0; j < cols; j++)
                    target[i, j] = provider();
            }

            return target;
        }

        /// <summary>
        ///     Populates a three dimensional array with values from an enumerable sequence
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="target"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static T1[,,] Populate<T1>(this T1[,,] target, IEnumerable<T1> values)
        {
            var (a, b, c) = (target.GetLength(0), target.GetLength(1), target.GetLength(2));

            using (var enumerator = values.GetEnumerator())
            {
                for (var i = 0; i < a; i++)
                {
                    for (var j = 0; j < b; j++)
                    {
                        for (var k = 0; k < c; k++)
                        {
                            enumerator.MoveNext();
                            target[i, j, k] = enumerator.Current;
                        }
                    }
                }
            }

            return target;
        }

        /// <summary>
        ///     Populates a three dimensional array with values from an enumerable sequence
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="target"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static T1[,,] Populate<T1>(this T1[,,] target, Func<T1> provider)
        {
            var (a, b, c) = (target.GetLength(0), target.GetLength(1), target.GetLength(2));

            for (var i = 0; i < a; i++)
            {
                for (var j = 0; j < b; j++)
                {
                    for (var k = 0; k < c; k++) target[i, j, k] = provider();
                }
            }

            return target;
        }

        /// <summary>
        ///     Determines the index skips for each dimension (except the last as it is always equal to 1) if the passed array
        ///     would be accessed as one dimensional
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static int[] MakeBlockItemCounts(this Array array)
        {
            var blocks = new int[array.Rank - 1];
            for (var i = array.Rank - 2; i >= 0; i--)
            {
                blocks[i] = array.GetUpperBound(i + 1) + 1;
                if (i != blocks.Length - 1) blocks[i] *= blocks[i + 1];
            }

            return blocks;
        }
    }
}