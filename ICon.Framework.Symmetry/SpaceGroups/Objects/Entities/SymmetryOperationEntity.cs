using System;
using System.Text;
using System.Xml.Serialization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using ICon.Framework.Extensions;
using ICon.Mathematics.Extensions;
using ICon.Mathematics.ValueTypes;

namespace ICon.Symmetry.SpaceGroups
{
    /// <summary>
    /// Symmetry operation entity class containing 3x4 matrix translational information for fractional coordinate vectors and can be stored in a database
    /// </summary>
    [Serializable]
    public class SymmetryOperationEntity : SymmetryOperationBase
    {
        /// <summary>
        /// Default trim tolerance value of 1.0e-10
        /// </summary>
        [XmlIgnore]
        [NotMapped]
        public new double TrimTolerance => 1.0e-10;

        /// <summary>
        /// The operation matrix
        /// </summary>
        [XmlIgnore]
        [NotMapped]
        public double[,] OperationMatrix { get; set; }

        /// <summary>
        /// The database context ID
        /// </summary>
        [Key]
        [XmlIgnore]
        public int ContextID { get; set; }

        /// <summary>
        /// The space group context ID
        /// </summary>
        [XmlIgnore]
        public int SpaceGroupContextID { get; set; }

        /// <summary>
        /// The affiliated space group
        /// </summary>
        [ForeignKey(nameof(SpaceGroupContextID))]
        [XmlIgnore]
        public SpaceGroupEntity SpaceGroup { get; set; }

        /// <summary>
        /// The comma seperated symmetry opeartion string for serialization and deserialization
        /// </summary>
        [XmlAttribute("Operation")]
        public string OperationString
        {
            get { return SerializeToAttributeString(); }
            set { DeserializeFromAttributeString(value); }
        }

        /// <summary>
        /// Applies the symmetry operation to an unspecified coordinate point and creates new coordinate information (Does not trim result into unit cell)
        /// </summary>
        /// <param name="original"></param>
        /// <returns></returns>
        public override Fractional3D ApplyUntrimmed(double orgA, double orgB, double orgC)
        {
            double coorA = (orgA * OperationMatrix[0, 0] + orgB * OperationMatrix[0, 1] + orgC * OperationMatrix[0, 2] + OperationMatrix[0, 3]);
            double coorB = (orgA * OperationMatrix[1, 0] + orgB * OperationMatrix[1, 1] + orgC * OperationMatrix[1, 2] + OperationMatrix[1, 3]);
            double coorC = (orgA * OperationMatrix[2, 0] + orgB * OperationMatrix[2, 1] + orgC * OperationMatrix[2, 2] + OperationMatrix[2, 3]);
            return new Fractional3D(coorA, coorB, coorC);
        }

        /// <summary>
        /// Creates a string from the symmetry operation matrix
        /// </summary>
        /// <param name="seperator"></param>
        /// <returns></returns>
        public string SerializeToAttributeString(char seperator = ',')
        {
            var builder = new StringBuilder(capacity: 100);
            for (int row = 0; row < OperationMatrix.GetUpperBound(0) + 1; row++)
            {
                for (int col = 0; col < OperationMatrix.GetUpperBound(1) + 1; col++)
                {
                    builder.Append(OperationMatrix[row, col].PrimitiveToString() + ",");
                }
            }
            builder.Remove(builder.Length - 1, 1);
            return builder.ToString();
        }

        /// <summary>
        /// Deserializes the operation matrix from a string of separated values
        /// </summary>
        /// <param name="serial"></param>
        /// <param name="seperator"></param>
        public void DeserializeFromAttributeString(string serial, char seperator = ',')
        {
            serial.TrySplitToSpecificSubstringCount(12, seperator, out string[] splitted);
            double[,] operationMatrix = new double[3, 4];
            int counter = 0;
            for (int row = 0; row < operationMatrix.GetUpperBound(0) + 1; row++)
            {
                for (int col = 0; col < operationMatrix.GetUpperBound(1) + 1; col++)
                {
                    operationMatrix[row, col] = splitted[counter].ToPrimitive<double>();
                    counter++;
                }
            }
            OperationMatrix = operationMatrix;
        }

        /// <summary>
        /// Gets a copy of the stored operation matrix as a linear array
        /// </summary>
        /// <returns></returns>
        public override double[] GetOperationsArray()
        {
            var result = new double[12];
            Buffer.BlockCopy(OperationMatrix, 0, result, 0, 12 * sizeof(double));
            return result;
        }
    }
}
