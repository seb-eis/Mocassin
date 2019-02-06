using System;
using System.Runtime.Serialization;
using Mocassin.Mathematics.ValueTypes;

namespace Mocassin.Symmetry.SpaceGroups
{
    /// <summary>
    ///     Serializable non entity version of the matrix based symmetry operation
    /// </summary>
    [DataContract]
    public class SymmetryOperation : SymmetryOperationBase
    {
        /// <summary>
        ///     Linearized version of the 12 matrix operation entries
        /// </summary>
        [DataMember]
        public double[] Operations { get; set; }

        /// <summary>
        ///     Creates new symmetry operation and checks passed operation array for correct size
        /// </summary>
        /// <param name="operations"></param>
        public SymmetryOperation(double[] operations)
            : this()
        {
            if (operations == null)
                throw new ArgumentNullException(nameof(operations));

            if (operations.Length != 12) 
                throw new ArgumentException("Operation array has wrong number of entries", nameof(operations));

            Operations = operations;
        }

        /// <summary>
        ///     Create new empty symmetry operation
        /// </summary>
        public SymmetryOperation()
        {
            TrimTolerance = 1.0e-10;
        }

        /// <inheritdoc />
        public override Fractional3D ApplyUntrimmed(double orgA, double orgB, double orgC)
        {
            var a = orgA * Operations[0] + orgB * Operations[1] + orgC * Operations[2] + Operations[3];
            var b = orgA * Operations[4] + orgB * Operations[5] + orgC * Operations[6] + Operations[7];
            var c = orgA * Operations[8] + orgB * Operations[9] + orgC * Operations[10] + Operations[11];
            return new Fractional3D(a, b, c);
        }

        /// <inheritdoc />
        public override double[] GetOperationsArray()
        {
            return Operations;
        }
    }
}