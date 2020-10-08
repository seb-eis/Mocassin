using System.Xml.Serialization;
using Mocassin.Model.Lattices;
using Mocassin.UI.Data.Base;

namespace Mocassin.UI.Data.Jobs
{
    /// <summary>
    ///     Describes an applicable doping by doping logic information and relative amount
    /// </summary>
    [XmlRoot]
    public class DopingValueData : ProjectDataObject, IDuplicable<DopingValueData>
    {
        private ModelObjectReference<Doping> doping;
        private double value;

        /// <summary>
        ///     Get or set the <see cref="ModelObjectReference{T}" /> for the doping information
        /// </summary>
        [XmlElement]
        public ModelObjectReference<Doping> Doping
        {
            get => doping;
            set => SetProperty(ref doping, value);
        }

        /// <summary>
        ///     Get or set the value for the doping in the range [0;1]
        /// </summary>
        [XmlAttribute]
        public double Value
        {
            get => value;
            set => SetProperty(ref this.value, value > 1.0 ? 1.0 : value < 0 ? 0 : value);
        }

        /// <inheritdoc />
        public DopingValueData Duplicate()
        {
            var result = new DopingValueData
            {
                value = value,
                Doping = Doping.Duplicate()
            };
            return result;
        }

        /// <inheritdoc />
        object IDuplicable.Duplicate() => Duplicate();
    }
}