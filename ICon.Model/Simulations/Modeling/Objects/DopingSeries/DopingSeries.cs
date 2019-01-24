using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Mocassin.Framework.Constraints;
using Mocassin.Model.Basic;
using Mocassin.Model.Lattices;

namespace Mocassin.Model.Simulations
{
    /// <inheritdoc />
    [DataContract]
    public class DopingSeries : IDopingSeries
    {
        /// <inheritdoc />
        [DataMember]
        [UseTrackedReferences]
        public IDoping Doping { get; set; }

        /// <inheritdoc />
        [DataMember]
        public IValueSeries ValueSeries { get; set; }

        /// <inheritdoc />
        public IEnumerator<double> GetEnumerator()
        {
            return ValueSeries.GetEnumerator();
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ValueSeries.GetEnumerator();
        }
    }
}