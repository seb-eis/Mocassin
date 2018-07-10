using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace ICon.Mathematics.Permutation
{
    /// <summary>
    /// A generic permutation class that counter lexicographically and periodically generates the permutations of a two dimensional list
    /// </summary>
    public class SlotMachinePermuter<T1> : IPermutationProvider<T1> where T1 : IComparable<T1>
    {
        /// <summary>
        /// The current permutation value
        /// </summary>
        public T1[] Value { get; protected set; }

        /// <summary>
        /// The total number of existing permutations, including dublicates
        /// </summary>
        public long PermutationCount { get; protected set; }

        /// <summary>
        /// The number of active permutation slots (Is also the result length)
        /// </summary>
        public int ResultLength { get; protected set; }

        /// <summary>
        /// The internal slot machine to produce permutations
        /// </summary>
        protected List<PermuterSlot<T1>> Slots { get; set; }

        /// <summary>
        /// Creates new slot maschine from arbitrary number of generic enumerable sequences
        /// </summary>
        /// <param name="optionsSet"></param>
        public SlotMachinePermuter(params IEnumerable<T1>[] optionsSet)
        {
            Slots = new List<PermuterSlot<T1>>(capacity: optionsSet.Length);
            PermutationCount = 1;
            foreach (var item in optionsSet)
            {
                var slot = new PermuterSlot<T1>(item);
                Slots.Add(slot);
                PermutationCount *= slot.SlotSize;
            }
            Value = new T1[optionsSet.Length];
            ResultLength = optionsSet.Length;
        }

        /// <summary>
        /// Creates new slot maschine from a 2d field of enumerable sequences
        /// </summary>
        /// <param name="optionsSet"></param>
        public SlotMachinePermuter(IEnumerable<IEnumerable<T1>> optionsSet) : this(optionsSet.ToArray())
        {

        }

        /// <summary>
        /// Advances the slot machine by one step an returns the new permutation
        /// </summary>
        /// <returns></returns>
        public T1[] GetNext()
        {
            Next();
            return (T1[])Value.Clone();
        }

        /// <summary>
        /// Returns slot machine to the previous value and returns value
        /// </summary>
        /// <returns></returns>
        public T1[] GetPrevious()
        {
            Previous();
            return (T1[])Value.Clone();
        }

        /// <summary>
        /// Advances slot machine to next value
        /// </summary>
        public void Next()
        {
            Boolean advanceNext = true;
            for (Int32 i = 0; i < ResultLength; i++)
            {
                advanceNext = Slots[i].NextWithPeriodicCheck();
                Value[i] = Slots[i].Value;
                if (advanceNext == false)
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Returns slot machine to the previous value
        /// </summary>
        public void Previous()
        {
            Boolean reducePrevious = true;
            for (Int32 i = 0; i < ResultLength; i++)
            {
                reducePrevious = Slots[i].PreviousWithPeriodicCheck();
                Value[i] = Slots[i].Value;
                if (reducePrevious == false)
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Reset the machine to start conditions
        /// </summary>
        public void Reset()
        {
            Slots.ForEach(slot => slot.Reset());
        }

        /// <summary>
        /// Linq style for each method over all permutations in lexicographical order
        /// </summary>
        /// <param name="action"></param>
        public void ForEach(Action<T1[]> action)
        {
            foreach (var item in this)
            {
                action(item);
            }
        }

        /// <summary>
        /// Get an enumerator to iterate over all permutations (Warning: Resets the slot machine to start conditions)
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T1[]> GetEnumerator()
        {
            Reset();
            int permIndex = -1;
            while (++permIndex != PermutationCount)
            {
                yield return GetNext();
            }
        }

        /// <summary>
        /// Get an enumerator to iterate over all permutations as objects (Warning: Resets the slot machine to start conditions)
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
