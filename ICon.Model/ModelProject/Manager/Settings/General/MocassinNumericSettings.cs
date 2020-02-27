using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Mocassin.Model.ModelProject
{
    /// <summary>
    ///     Numeric settings class that stores numeric properties and tolerance information for the geometry calculations of
    ///     the model process
    /// </summary>
    [DataContract]
    [XmlRoot("MocassinNumericSettings")]
    public class MocassinNumericSettings
    {
        /// <summary>
        ///     The maximum number of ULP steps that are allowed in numeric calculations using ULP comparisons
        /// </summary>
        [DataMember]
        [XmlAttribute("UlpCompare")]
        public int UlpValue { get; set; } = 10;

        /// <summary>
        ///     The maximum absolute tolerance in Angstrom that is allowed in numeric calculations using absolute comparisons
        /// </summary>
        [DataMember]
        [XmlAttribute("RangeCompare")]
        public double RangeValue { get; set; } = 1.0e-6;

        /// <summary>
        ///     The maximum relative tolerance that is allowed in numeric calculations using relative comparisons
        /// </summary>
        [DataMember]
        [XmlAttribute("FactorCompare")]
        public double FactorValue { get; set; } = 1.0e-3;
    }
}