using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.Collections;

namespace ICon.Framework.Constraints
{
    /// <summary>
    /// Simple double precision value series that defines lower bound, upper bound and an increment step
    /// </summary>
    [DataContract]
    public class ValueSeries : IValueSeries
    {
        /// <summary>
        /// The lower limit of the series. Values are greater or equal to this value
        /// </summary>
        [DataMember]
        public double LowerLimit { get; set; }

        /// <summary>
        /// The upper limit of the series. Values are lesser or equal to this value
        /// </summary>
        [DataMember]
        public double UpperLimit { get; set; }

        /// <summary>
        /// The increment step of the series  
        /// </summary>
        [DataMember]
        public double Increment { get; set; }

        /// <summary>
        /// Create new value series with lower limit, upper limit and the increment value. Auto sorts lower and upper by value
        /// </summary>
        /// <param name="lowerLimit"></param>
        /// <param name="upperLimit"></param>
        /// <param name="increment"></param>
        public ValueSeries(double lowerLimit, double upperLimit, double increment)
        {
            LowerLimit = Math.Min(lowerLimit, UpperLimit);
            UpperLimit = Math.Max(lowerLimit, upperLimit);
            Increment = increment;
        }

        /// <summary>
        /// Create new default value series
        /// </summary>
        public ValueSeries()
        {
        }

        /// <summary>
        /// Get the number of values in the series which is greater or equal to one
        /// </summary>
        /// <returns></returns>
        public int GetValueCount()
        {
            if (Increment == 0 && LowerLimit == UpperLimit)
            {
                return 1;
            }
            return (int)(Math.Round(Math.Abs(UpperLimit - LowerLimit)) / Math.Abs(Increment)) + 1;
        }

        /// <summary>
        /// Get an enumerable sequence of all values describes by the series starting at the lower limit
        /// </summary>
        /// <returns></returns>
        public IEnumerable<double> GetValues()
        {
            for (int i = 0; i < GetValueCount(); i++)
            {
                yield return LowerLimit + Increment * i;
            }
        }

        /// <summary>
        /// Get an enumerator for all values of the series
        /// </summary>
        /// <returns></returns>
        public IEnumerator<double> GetEnumerator()
        {
            return GetValues().GetEnumerator();
        }

        /// <summary>
        /// Get a non-generic enumerator for all values of the sequence
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Creates a pseudo value series that contains only a single value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static ValueSeries MakeSingle(double value)
        {
            return new ValueSeries(value, value, 0);
        }
    }
}
