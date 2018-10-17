using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
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
        /// <summary>
        ///     Default trim tolerance value of 1.0e-10
        /// </summary>
        [XmlIgnore]
        [NotMapped]
        public new double TrimTolerance => 1.0e-10;

        /// <summary>
        ///     The operation matrix
        /// </summary>
        [XmlIgnore]
        [NotMapped]
        public double[,] OperationMatrix { get; set; }

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
            var a = orgA * OperationMatrix[0, 0] + orgB * OperationMatrix[0, 1] + orgC * OperationMatrix[0, 2] + OperationMatrix[0, 3];
            var b = orgA * OperationMatrix[1, 0] + orgB * OperationMatrix[1, 1] + orgC * OperationMatrix[1, 2] + OperationMatrix[1, 3];
            var c = orgA * OperationMatrix[2, 0] + orgB * OperationMatrix[2, 1] + orgC * OperationMatrix[2, 2] + OperationMatrix[2, 3];
            return new Fractional3D(a, b, c);
        }

        /// <summary>
        ///     Creates a string from the symmetry operation matrix
        /// </summary>
        /// <param name="separator"></param>
        /// <returns></returns>
        public string SerializeToAttributeString(char separator = ',')
        {
            var builder = new StringBuilder(100);
            for (var row = 0; row < OperationMatrix.GetUpperBound(0) + 1; row++)
            {
                for (var col = 0; col < OperationMatrix.GetUpperBound(1) + 1; col++)
                    builder.Append(OperationMatrix[row, col].PrimitiveToString() + ",");
            }

            builder.Remove(builder.Length - 1, 1);
            return builder.ToString();
        }

        /// <summary>
        ///     Deserializes the operation matrix from a string of separated values
        /// </summary>
        /// <param name="serial"></param>
        /// <param name="separator"></param>
        public void DeserializeFromAttributeString(string serial, char separator = ',')
        {
            serial.TrySplitToSpecificSubstringCount(12, separator, out var split);
            var operationMatrix = new double[3, 4];
            var counter = 0;
            for (var row = 0; row < operationMatrix.GetUpperBound(0) + 1; row++)
            {
                for (var col = 0; col < operationMatrix.GetUpperBound(1) + 1; col++)
                {
                    operationMatrix[row, col] = split[counter].ToPrimitive<double>();
                    counter++;
                }
            }

            OperationMatrix = operationMatrix;
        }

        /// <inheritdoc />
        public override double[] GetOperationsArray()
        {
            var result = new double[12];
            Buffer.BlockCopy(OperationMatrix, 0, result, 0, 12 * sizeof(double));
            return result;
        }
    }
}