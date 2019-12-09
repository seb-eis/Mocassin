using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Xml.Serialization;
using Mocassin.Framework.Extensions;

namespace Mocassin.Symmetry.SpaceGroups
{
    /// <summary>
    ///     Symmetry operation entity class containing 3x4 matrix translational information for fractional coordinate vectors
    ///     and can be stored in a database
    /// </summary>
    [XmlRoot]
    public class SymmetryOperationEntity : SymmetryOperationBase
    {
        /// <summary>
        ///     Default trim tolerance value of 1.0e-10
        /// </summary>
        [XmlIgnore]
        [NotMapped]
        public override double TrimTolerance { get; set; } = 1.0e-10;

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

        /// <summary>
        ///     Creates a string from the symmetry operation matrix
        /// </summary>
        /// <param name="separator"></param>
        /// <param name="formatProvider"></param>
        /// <returns></returns>
        public string SerializeToAttributeString(char separator = ',', IFormatProvider formatProvider = null)
        {
            const string format = "{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11}";
            formatProvider ??= CultureInfo.InvariantCulture;
            return string.Format(formatProvider, format, Core.M11, Core.M12, Core.M13, Core.M14, Core.M21, Core.M22, Core.M23, Core.M24, Core.M31, Core.M32,
                Core.M33,
                Core.M34);
        }

        /// <summary>
        ///     Deserializes the operation matrix from a string of separated values
        /// </summary>
        /// <param name="serial"></param>
        /// <param name="separator"></param>
        /// <param name="formatProvider"></param>
        public void DeserializeFromAttributeString(string serial, char separator = ',', IFormatProvider formatProvider = null)
        {
            formatProvider ??= CultureInfo.InvariantCulture;
            serial.TrySplitToSpecificSubstringCount(12, separator, out var split);
            var core = new SymmetryOperationCore(
                double.Parse(split[0], formatProvider),
                double.Parse(split[1], formatProvider),
                double.Parse(split[2], formatProvider),
                double.Parse(split[3], formatProvider),
                double.Parse(split[4], formatProvider),
                double.Parse(split[5], formatProvider),
                double.Parse(split[6], formatProvider),
                double.Parse(split[7], formatProvider),
                double.Parse(split[8], formatProvider),
                double.Parse(split[9], formatProvider),
                double.Parse(split[10], formatProvider),
                double.Parse(split[11], formatProvider));
            SetCore(core);
        }
    }
}