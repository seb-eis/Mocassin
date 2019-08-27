using System.Xml.Serialization;
using Mocassin.Model.Lattices;
using Mocassin.UI.Xml.Base;

namespace Mocassin.UI.Xml.Jobs
{
    /// <summary>
    ///     Describes an applicable doping by doping logic information and relative amount
    /// </summary>
    [XmlRoot("DopingValue")]
    public class DopingValueGraph : ProjectObjectGraph, IDuplicable<DopingValueGraph>
    {
        private double value;

        /// <summary>
        ///     Get or set the <see cref="ModelObjectReferenceGraph{T}"/> for the doping information
        /// </summary>
        [XmlElement("Doping")]
        public ModelObjectReferenceGraph<Doping> Doping { get; set; }

        /// <summary>
        ///     Get or set the value for the doping in the range [0;1]
        /// </summary>
        [XmlAttribute("Value")]
        public double Value
        {
            get => value;
            set => this.value = value > 1.0 ? 1.0 : value < 0 ? 0 : value;
        }

        /// <inheritdoc />
        public DopingValueGraph Duplicate()
        {
            var result = new DopingValueGraph
            {
                value = value,
                Doping = Doping.Duplicate()
            };
            return result;
        }

        /// <inheritdoc />
        object IDuplicable.Duplicate()
        {
            return Duplicate();
        }
    }
}