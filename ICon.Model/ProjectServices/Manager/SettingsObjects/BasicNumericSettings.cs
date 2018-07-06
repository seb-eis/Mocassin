using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace ICon.Model.ProjectServices
{
    /// <summary>
    /// Numeric settings class that stores numeric properties and tolerance information for the geometry calculations of the model process
    /// </summary>
    [DataContract]
    public class BasicNumericSettings
    {
        /// <summary>
        /// The maximum number of ULP steps that are allowed in numeric calculations using ULP comparisons
        /// </summary>
        [DataMember]
        public int UlpValue { get; set; }

        /// <summary>
        /// The maximum absolut tolerance in Angstrom that is allowed in numeric calculations using absolut comparisons
        /// </summary>
        [DataMember]
        public double RangeValue { get; set; }

        /// <summary>
        /// The maximum relative tolerance that is allowed in numeric calculations using relative comparisons
        /// </summary>
        [DataMember]
        public double FactorValue { get; set; }
    }
}
