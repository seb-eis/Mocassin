using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Xml.Serialization;
using Mocassin.Framework.Extensions;
using Mocassin.Mathematics.ValueTypes;

namespace Mocassin.Symmetry.SpaceGroups
{
    /// <summary>
    ///     Symmetry operation entity class containing 3x4 matrix translational information for fractional coordinate vectors
    ///     and can be stored in a database
    /// </summary>
    [XmlRoot]
    public class SymmetryOperationEntity : SymmetryOperationBase
    {
        [NotMapped]
        private double Entry00 { get; set; }

        [NotMapped]
        private double Entry01 { get; set; }

        [NotMapped]
        private double Entry02 { get; set; }

        [NotMapped]
        private double Entry03 { get; set; }

        [NotMapped]
        private double Entry10 { get; set; }

        [NotMapped]
        private double Entry11 { get; set; }

        [NotMapped]
        private double Entry12 { get; set; }

        [NotMapped]
        private double Entry13 { get; set; }

        [NotMapped]
        private double Entry20 { get; set; }

        [NotMapped]
        private double Entry21 { get; set; }

        [NotMapped]
        private double Entry22 { get; set; }

        [NotMapped]
        private double Entry23 { get; set; }

        /// <summary>
        ///     Default trim tolerance value of 1.0e-10
        /// </summary>
        [XmlIgnore]
        [NotMapped]
        public new double TrimTolerance => 1.0e-10;

        /// <summary>
        ///     The database context ID
        /// </summary>
        [Key]
        [XmlIgnore]
        public int ContextId { get; set; }

        /// <summary>
        ///     The space group context ID
        /// </summary>
        [XmlIgnore]
        [Column("SpaceGroupContextID")]
        public int SpaceGroupId { get; set; }

        /// <summary>
        ///     The affiliated space group
        /// </summary>
        [ForeignKey(nameof(SpaceGroupId))]
        [XmlIgnore]
        public SpaceGroupEntity SpaceGroup { get; set; }

        /// <summary>
        ///     The comma separated symmetry operation string for serialization and deserialization
        /// </summary>
        [XmlAttribute("Operation")]
        public string OperationString
        {
            get => SerializeToAttributeString();
            set => DeserializeFromAttributeString(value);
        }

        /// <inheritdoc />
        public override Fractional3D ApplyUntrimmed(double orgA, double orgB, double orgC)
        {
            var a = orgA * Entry00 + orgB * Entry01 + orgC * Entry02 + Entry03;
            var b = orgA * Entry10 + orgB * Entry11 + orgC * Entry12 + Entry13;
            var c = orgA * Entry20 + orgB * Entry21 + orgC * Entry22 + Entry23;
            return new Fractional3D(a, b, c);
        }

        /// <summary>
        ///     Creates a string from the symmetry operation matrix
        /// </summary>
        /// <param name="separator"></param>
        /// <param name="formatProvider"></param>
        /// <returns></returns>
        public string SerializeToAttributeString(char separator = ',', IFormatProvider formatProvider = null)
        {
            const string format = "{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11}";
            formatProvider = formatProvider ?? CultureInfo.InvariantCulture;
            return string.Format(formatProvider, format, Entry00, Entry01, Entry02, Entry03, Entry10, Entry11, Entry12, Entry13, Entry20, Entry21, Entry22,
                Entry23);
        }

        /// <summary>
        ///     Deserializes the operation matrix from a string of separated values
        /// </summary>
        /// <param name="serial"></param>
        /// <param name="separator"></param>
        /// <param name="formatProvider"></param>
        public void DeserializeFromAttributeString(string serial, char separator = ',', IFormatProvider formatProvider = null)
        {
            formatProvider = formatProvider ?? CultureInfo.InvariantCulture;
            serial.TrySplitToSpecificSubstringCount(12, separator, out var split);
            Entry00 = double.Parse(split[0], formatProvider);
            Entry01 = double.Parse(split[1], formatProvider);
            Entry02 = double.Parse(split[2], formatProvider);
            Entry03 = double.Parse(split[3], formatProvider);
            Entry10 = double.Parse(split[4], formatProvider);
            Entry11 = double.Parse(split[5], formatProvider);
            Entry12 = double.Parse(split[6], formatProvider);
            Entry13 = double.Parse(split[7], formatProvider);
            Entry20 = double.Parse(split[8], formatProvider);
            Entry21 = double.Parse(split[9], formatProvider);
            Entry22 = double.Parse(split[10], formatProvider);
            Entry23 = double.Parse(split[11], formatProvider);
        }

        /// <inheritdoc />
        public override double[] GetOperationsArray()
        {
            return new[]
            {
                Entry00, Entry01, Entry02, Entry03,
                Entry10, Entry11, Entry12, Entry13,
                Entry20, Entry21, Entry22, Entry23
            };
        }
    }
}