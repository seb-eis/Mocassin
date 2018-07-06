using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

using ICon.Model.Basic;
using ICon.Framework.Constraints;
using ICon.Model.Lattices;
using System.Collections;

namespace ICon.Model.Simulations
{
    /// <summary>
    /// Describes a doping series by abstract doping information and a value series
    /// </summary>
    [DataContract]
    public class DopingSeries : IDopingSeries
    {
        /// <summary>
        /// The abstract doping information for the series
        /// </summary>
        [DataMember]
        [LinkableByIndex]
        public IDoping Doping { get; set; }

        /// <summary>
        /// The actual concentration values of the series
        /// </summary>
        [DataMember]
        public IValueSeries ValueSeries { get; set; }

        /// <summary>
        /// Get the generic enumerator for the value series
        /// </summary>
        /// <returns></returns>
        public IEnumerator<double> GetEnumerator()
        {
            return ValueSeries.GetEnumerator();
        }

        /// <summary>
        /// Get the non-generic enumerator for the value series
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ValueSeries.GetEnumerator();
        }
    }
}
