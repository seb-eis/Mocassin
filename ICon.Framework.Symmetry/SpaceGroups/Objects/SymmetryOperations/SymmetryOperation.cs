using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using ICon.Mathematics.ValueTypes;

namespace ICon.Symmetry.SpaceGroups
{
    /// <summary>
    /// Serializable non entity version of the matrix based symmetry operation
    /// </summary>
    [DataContract]
    public class SymmetryOperation : SymmetryOperationBase
    {
        /// <summary>
        /// Linearized version of the 12 matrix operation entries (Not recommended to set manually)
        /// </summary>
        [DataMember]
        public double[] Operations { get; set; }

        /// <summary>
        /// Creates new symmetry operation and checks passed operation array for correct size
        /// </summary>
        /// <param name="operations"></param>
        public SymmetryOperation(double[] operations) : this()
        {
            if (operations.Length != 12)
            {
                throw new ArgumentException("Operation array has wrong number of entries", nameof(operations));
            }
            Operations = operations ?? throw new ArgumentNullException(nameof(operations));
        }

        /// <summary>
        /// Create new empty symmtery operation
        /// </summary>
        public SymmetryOperation()
        {
            TrimTolerance = 1.0e-10;
        }

        /// <summary>
        /// Applies the symmetry operation to an unspecified coordinate point and creates new coordinate information (Does not trim result into unit cell)
        /// </summary>
        /// <param name="original"></param>
        /// <returns></returns>
        public override Fractional3D ApplyUntrimmed(double orgA, double orgB, double orgC)
        {
            double coorA = (orgA * Operations[0] + orgB * Operations[1] + orgC * Operations[2] + Operations[3]);
            double coorB = (orgA * Operations[4] + orgB * Operations[5] + orgC * Operations[6] + Operations[7]);
            double coorC = (orgA * Operations[8] + orgB * Operations[9] + orgC * Operations[10] + Operations[11]);
            return new Fractional3D(coorA, coorB, coorC);
        }

        /// <summary>
        /// Get the linearized operations array
        /// </summary>
        /// <returns></returns>
        public override double[] GetOperationsArray()
        {
            return Operations;
        }
    }
}
