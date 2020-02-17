using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Mocassin.Mathematics.Permutation
{
    /// <summary>
    ///     A generic permutation class that counter lexicographically and periodically generates the permutations of a two
    ///     dimensional list using a slot machine setup
    /// </summary>
    public class PermutationSlotMachine<T1> : IPermutationSource<T1> where T1 : IComparable<T1>
    {
        /// <inheritdoc />
        public T1[] Value { get; protected set; }

        /// <inheritdoc />
        public long PermutationCount { get; protected set; }

        /// <inheritdoc />
        public int ResultLength { get; protected set; }

        /// <summary>
        ///     The internal slot machine to produce permutations
        /// </summary>
        protected List<PermutationMachineSlot<T1>> Slots { get; set; }

        /// <summary>
        ///     Creates new slot machine from arbitrary number of generic enumerable sequences
        /// </summary>
        /// <param name="optionsSet"></param>
        public PermutationSlotMachine(params IEnumerable<T1>[] optionsSet)
        {
            Slots = new List<PermutationMachineSlot<T1>>(optionsSet.Length);
            PermutationCount = 1;
            foreach (var item in optionsSet)
            {
                var slot = new PermutationMachineSlot<T1>(item);
                Slots.Add(slot);
                PermutationCount *= slot.SlotSize;
            }

            Value = new T1[optionsSet.Length];
            ResultLength = optionsSet.Length;
        }

        /// <summary>
        ///     Creates new slot machine from a 2d field of enumerable sequences
        /// </summary>
        /// <param name="optionsSet"></param>
        public PermutationSlotMachine(IEnumerable<IEnumerable<T1>> optionsSet)
            : this(optionsSet.ToArray())
        {
        }

        /// <inheritdoc />
        public T1[] GetNext()
        {
            Next();
            return (T1[]) Value.Clone();
        }

        /// <inheritdoc />
        public T1[] GetPrevious()
        {
            Previous();
            return (T1[]) Value.Clone();
        }

        /// <inheritdoc />
        public void Next()
        {
            for (var i = 0; i < ResultLength; i++)
            {
                var advanceNext = Slots[i].NextWithPeriodicCheck();
                Value[i] = Slots[i].Value;
                if (!advanceNext)
                    break;
            }
        }

        /// <inheritdoc />
        public void Previous()
        {
            for (var i = 0; i < ResultLength; i++)
            {
                var reducePrevious = Slots[i].PreviousWithPeriodicCheck();
                Value[i] = Slots[i].Value;
                if (!reducePrevious)
                    break;
            }
        }

        /// <inheritdoc />
        public void Reset()
        {
            Slots.ForEach(slot => slot.Reset());
        }

        /// <inheritdoc />
        /// <remarks> Getting an enumerator from the permutation source triggers the internal reset </remarks>
        public IEnumerator<T1[]> GetEnumerator()
        {
            Reset();
            var permIndex = -1;
            while (++permIndex != PermutationCount)
                yield return GetNext();
        }

        /// <inheritdoc />
        /// <remarks> Getting an enumerator from the permutation source triggers the internal reset </remarks>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        ///     Linq style for each method over all permutations in lexicographical order
        /// </summary>
        /// <param name="action"></param>
        public void ForEach(Action<T1[]> action)
        {
            foreach (var item in this) action(item);
        }
    }
}