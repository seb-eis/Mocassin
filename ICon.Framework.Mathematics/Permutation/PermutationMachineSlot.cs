using System;
using System.Collections.Generic;
using System.Linq;

namespace Mocassin.Mathematics.Permutation
{
    /// <summary>
    ///     Single periodic permutation machine slot that lexicographically and periodically goes through all entries of a list
    /// </summary>
    public class PermutationMachineSlot<T1>
    {
        /// <summary>
        ///     The list of possible values
        /// </summary>
        public T1[] ValueOptions { get; protected set; }

        /// <summary>
        ///     The number of possible values
        /// </summary>
        public int SlotSize { get; protected set; }

        /// <summary>
        ///     The current index of the value options
        /// </summary>
        public int CurrentIndex { get; protected set; }

        /// <summary>
        ///     The current active value
        /// </summary>
        public T1 Value { get; protected set; }

        /// <summary>
        ///     Creates a new permutation machine slot from array, set to the last existing value
        /// </summary>
        /// <param name="valueOptions"></param>
        /// <param name="comparer"></param>
        public PermutationMachineSlot(IEnumerable<T1> valueOptions, IComparer<T1> comparer = null)
        {
            var valueArray = valueOptions.ToArray();
            if (valueArray.Length == 0)
                throw new ArgumentException(paramName: nameof(valueOptions), message: "The number of value options is zero");
            Array.Sort(valueArray, comparer ?? Comparer<T1>.Default);
            ValueOptions = valueArray;
            SlotSize = ValueOptions.Length;
            CurrentIndex = SlotSize - 1;
        }

        /// <summary>
        ///     Advances to the next value and returns the value
        /// </summary>
        /// <returns></returns>
        public T1 GetNext()
        {
            Next();
            return Value;
        }

        /// <summary>
        ///     Returns to the previous value and returns the value
        /// </summary>
        /// <returns></returns>
        public T1 GetPrevious()
        {
            Previous();
            return Value;
        }

        /// <summary>
        ///     Advances the slot to the next value
        /// </summary>
        public void Next()
        {
            CurrentIndex++;
            if (CurrentIndex == SlotSize)
                CurrentIndex = 0;

            Value = ValueOptions[CurrentIndex];
        }

        /// <summary>
        ///     Returns the slot to the previous value
        /// </summary>
        public void Previous()
        {
            CurrentIndex--;
            if (CurrentIndex < 0)
                CurrentIndex = SlotSize - 1;

            Value = ValueOptions[CurrentIndex];
        }

        /// <summary>
        ///     Set to the next value, returns true if the upper limit was reached and the slot reset to start value
        /// </summary>
        /// <returns></returns>
        public bool NextWithPeriodicCheck()
        {
            Next();
            return CurrentIndex == 0;
        }

        /// <summary>
        ///     Set to the previous value, returns true if the lower limit was reached and the slot reset to the final value
        /// </summary>
        /// <returns></returns>
        public bool PreviousWithPeriodicCheck()
        {
            Previous();
            return CurrentIndex == SlotSize - 1;
        }

        /// <summary>
        ///     Resets the slot to start conditions (i.e. last existing state)
        /// </summary>
        public void Reset()
        {
            while (PreviousWithPeriodicCheck() == false)
            {
            }
        }
    }
}