using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
// ReSharper disable CompareOfFloatsByEqualityOperator

namespace Mocassin.Framework.Constraints
{
    /// <inheritdoc />
    [DataContract]
    public class ValueSeries : IValueSeries
    {
        /// <inheritdoc />
        [DataMember]
        public double LowerLimit { get; set; }

        /// <inheritdoc />
        [DataMember]
        public double UpperLimit { get; set; }

        /// <summary>
        ///     The increment step of the series
        /// </summary>
        [DataMember]
        public double Increment { get; set; }

        /// <summary>
        ///     Create new value series with lower limit, upper limit and the increment value. Auto sorts lower and upper by value
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
        ///     Create new default value series
        /// </summary>
        public ValueSeries()
        {
        }

        /// <inheritdoc />
        public int GetValueCount()
        {
            if (Increment == 0 && LowerLimit == UpperLimit)
                return 1;

            return (int) (Math.Round(Math.Abs(UpperLimit - LowerLimit)) / Math.Abs(Increment)) + 1;
        }

        /// <inheritdoc />
        public IEnumerable<double> GetValues()
        {
            for (var i = 0; i < GetValueCount(); i++)
                yield return LowerLimit + Increment * i;
        }

        /// <inheritdoc />
        public IEnumerator<double> GetEnumerator()
        {
            return GetValues().GetEnumerator();
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        ///     Creates a pseudo value series that contains only a single value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static ValueSeries MakeSingle(double value)
        {
            return new ValueSeries(value, value, 0);
        }
    }
}