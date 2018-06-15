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
    public class SymmetryOperationEntity : ISymmetryOperation
    {
        /// <summary>
        /// Default trim tolerance value of 1.0e-10
        /// </summary>
        [XmlIgnore]
        [NotMapped]
        public double TrimTolerance => 1.0e-10;

        /// <summary>
        /// The operation matrix
        /// </summary>
        [XmlIgnore]
        [NotMapped]
        public double[,] OperationMatrix { get; set; }

        /// <summary>
        /// The string literal representation of the encoded symmetry operation
        /// </summary>
        [XmlAttribute("Literal")]
        public string Literal { get; set; } = "";

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
        public String OperationString
        {
            get { return SerializeToAttributeString(); }
            set { DeserializeFromAttributeString(value); }
        }

        /// <summary>
        /// Applies the symetry operation to an unspecified coordinate point and creates new coordinate information
        /// </summary>
        /// <param name="original"></param>
        /// <returns></returns>
        public Fractional3D ApplyWithTrim(Double orgA, Double orgB, Double orgC)
        {
            return ApplyUntrimmed(orgA, orgB, orgC).TrimToUnitCell(TrimTolerance);
        }

        /// <summary>
        /// Applies the symmetry operation to an unspecified coordinate point and creates new coordinate information (Does not trim result into unit cell)
        /// </summary>
        /// <param name="original"></param>
        /// <returns></returns>
        public Fractional3D ApplyUntrimmed(Double orgA, Double orgB, Double orgC)
        {
            Double coorA = (orgA * OperationMatrix[0, 0] + orgB * OperationMatrix[0, 1] + orgC * OperationMatrix[0, 2] + OperationMatrix[0, 3]);
            Double coorB = (orgA * OperationMatrix[1, 0] + orgB * OperationMatrix[1, 1] + orgC * OperationMatrix[1, 2] + OperationMatrix[1, 3]);
            Double coorC = (orgA * OperationMatrix[2, 0] + orgB * OperationMatrix[2, 1] + orgC * OperationMatrix[2, 2] + OperationMatrix[2, 3]);
            return new Fractional3D(coorA, coorB, coorC);
        }

        /// <summary>
        /// Applies operation to a basic fractional vector
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public Fractional3D ApplyWithTrim(in Fractional3D vector)
        {
            return ApplyWithTrim(vector.A, vector.B, vector.C);
        }

        /// <summary>
        /// Applies operation to a basic fractional vector (Result is not trimmed to unit cell)
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public Fractional3D ApplyUntrimmed(in Fractional3D vector)
        {
            return ApplyUntrimmed(vector.A, vector.B, vector.C);
        }

        /// <summary>
        /// Applies the symmetry operation to a 64 bit generic fractional vector (With trim to origin cell)
        /// </summary>
        /// <param name="original"></param>
        /// <returns></returns>
        public T1 ApplyWithTrim<T1>(in T1 original) where T1 : struct, IFractional3D<T1>
        {
            return original.CreateNew(ApplyWithTrim(original.A, original.B, original.C).Coordinates);
        }

        /// <summary>
        /// Applies the symmetry operation to a 64 bit generic fractional vector (No trim to origin cell)
        /// </summary>
        /// <param name="original"></param>
        /// <returns></returns>
        public T1 ApplyUntrimmed<T1>(in T1 original) where T1 : struct, IFractional3D<T1>
        {
            return original.CreateNew(ApplyUntrimmed(original.A, original.B, original.C).Coordinates);
        }

        /// <summary>
        /// Creates a string from the symmetry operation matrix
        /// </summary>
        /// <param name="seperator"></param>
        /// <returns></returns>
        public String SerializeToAttributeString(Char seperator = ',')
        {
            var builder = new StringBuilder(capacity: 100);
            for (Int32 row = 0; row < OperationMatrix.GetUpperBound(0) + 1; row++)
            {
                for (Int32 col = 0; col < OperationMatrix.GetUpperBound(1) + 1; col++)
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
        public void DeserializeFromAttributeString(String serial, Char seperator = ',')
        {
            serial.TrySplitToSpecificSubstringCount(12, seperator, out String[] splitted);
            Double[,] operationMatrix = new Double[3, 4];
            Int32 counter = 0;
            for (Int32 row = 0; row < operationMatrix.GetUpperBound(0) + 1; row++)
            {
                for (Int32 col = 0; col < operationMatrix.GetUpperBound(1) + 1; col++)
                {
                    operationMatrix[row, col] = splitted[counter].ToPrimitive<Double>();
                    counter++;
                }
            }
            OperationMatrix = operationMatrix;
        }

        /// <summary>
        /// Gets a copy of the stored operation matrix as a linear array
        /// </summary>
        /// <returns></returns>
        public double[] GetOperationsArray()
        {
            var result = new double[12];
            Buffer.BlockCopy(OperationMatrix, 0, result, 0, 12 * sizeof(double));
            return result;
        }
    }
}
