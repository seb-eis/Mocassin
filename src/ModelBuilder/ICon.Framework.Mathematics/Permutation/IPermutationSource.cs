﻿using System.Collections.Generic;

namespace Mocassin.Mathematics.Permutation
{
    /// <summary>
    ///     Represents a permutation source for a specific type set
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    public interface IPermutationSource<out T1> : IEnumerable<T1[]>
    {
        /// <summary>
        ///     Get the number of possible permutations
        /// </summary>
        long PermutationCount { get; }

        /// <summary>
        ///     Get the length of the result permutation
        /// </summary>
        int ResultLength { get; }

        /// <summary>
        ///     Get the current value array
        /// </summary>
        T1[] Value { get; }

        /// <summary>
        ///     Get the next permutation value
        /// </summary>
        /// <returns></returns>
        T1[] GetNext();

        /// <summary>
        ///     Get the previous permutation value
        /// </summary>
        /// <returns></returns>
        T1[] GetPrevious();

        /// <summary>
        ///     Increases internal state to next permutation
        /// </summary>
        void Next();

        /// <summary>
        ///     Decreases internal state to the previous permutation
        /// </summary>
        void Previous();

        /// <summary>
        ///     Reset to start state
        /// </summary>
        void Reset();
    }
}