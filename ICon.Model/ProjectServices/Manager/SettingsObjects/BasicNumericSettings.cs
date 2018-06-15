using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace ICon.Model.ProjectServices
{
    /// <summary>
    /// Numeric settings class that stores numeric properties and tolerance information for the geometry calculations of the model process
    /// </summary>
    [Serializable]
    [DataContract]
    public class BasicNumericSettings
    {
        /// <summary>
        /// The maximum number of ULP steps that are allowed in numeric calculations using ULP comparisons
        /// </summary>
        [DataMember(Name ="ULP")]
        public Int32 CompUlp { get; set; }

        /// <summary>
        /// The maximum absolut tolerance in Angstrom that is allowed in numeric calculations using absolut comparisons
        /// </summary>
        [DataMember(Name ="Range")]
        public Double CompRange { get; set; }

        /// <summary>
        /// The maximum relative tolerance that is allowed in numeric calculations using relative comparisons
        /// </summary>
        [DataMember(Name ="Factor")]
        public Double CompFactor { get; set; }
    }
}
