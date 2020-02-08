using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using Mocassin.Mathematics.ValueTypes;

namespace Mocassin.Symmetry.SpaceGroups
{
    /// <summary>
    ///     Base class for all symmetry operation implementations
    /// </summary>
    [DataContract]
    public abstract class SymmetryOperationBase : ISymmetryOperation
    {
        /// <summary>
        ///     The <see cref="SymmetryOperationCore" /> backing field
        /// </summary>
        [NotMapped] private SymmetryOperationCore operationCore;

        /// <inheritdoc />
        [NotMapped]
        public ref SymmetryOperationCore Core => ref operationCore;

        /// <inheritdoc />
        public bool FlipsOrientation => Core.FlipsOrientation(TrimTolerance);

        /// <inheritdoc />
        public abstract double TrimTolerance { get; set; }

        /// <inheritdoc />
        [DataMember]
        public string Literal { get; set; }

        /// <inheritdoc />
        public Fractional3D TrimTransform(double orgA, double orgB, double orgC)
        {
            return Core.TrimTransform(orgA, orgB, orgC, TrimTolerance);
        }

        /// <inheritdoc />
        public Fractional3D TrimTransform(in Fractional3D vector, out Fractional3D trimVector)
        {
            var untrimmed = Transform(vector);
            var trimmed = untrimmed.TrimToUnitCell(TrimTolerance);
            trimVector = trimmed - untrimmed;
            return trimmed;
        }

        /// <inheritdoc />
        public Fractional3D Transform(double orgA, double orgB, double orgC)
        {
            return Core.Transform(orgA, orgB, orgC);
        }

        /// <inheritdoc />
        public Fractional3D TrimTransform(in Fractional3D vector)
        {
            return Core.TrimTransform(vector, TrimTolerance);
        }

        /// <inheritdoc />
        public Fractional3D Transform(in Fractional3D vector)
        {
            return Core.Transform(vector);
        }

        /// <inheritdoc />
        public Fractional3D TrimTransform(IFractional3D original)
        {
            return TrimTransform(original.A, original.B, original.C);
        }

        /// <inheritdoc />
        public Fractional3D Transform(IFractional3D original)
        {
            return Transform(original.A, original.B, original.C);
        }

        /// <inheritdoc />
        public IEnumerable<Fractional3D> TrimTransform(IEnumerable<Fractional3D> vectors)
        {
            return vectors.Select(vector => Core.TrimTransform(vector, TrimTolerance));
        }

        /// <inheritdoc />
        public IEnumerable<Fractional3D> Transform(IEnumerable<Fractional3D> vectors)
        {
            return vectors.Select(vector => Core.Transform(vector));
        }

        /// <inheritdoc />
        public double[] GetOperationsArray()
        {
            return Core.ToArray();
        }

        /// <summary>
        ///     Sets the <see cref="SymmetryOperationCore" /> from an implementing class
        /// </summary>
        /// <param name="core"></param>
        protected void SetCore(in SymmetryOperationCore core)
        {
            operationCore = core;
        }
    }
}