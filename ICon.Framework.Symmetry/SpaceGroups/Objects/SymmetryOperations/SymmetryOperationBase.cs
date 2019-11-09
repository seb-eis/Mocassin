using System.Collections.Generic;
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
        /// <inheritdoc />
        [DataMember]
        public double TrimTolerance { get; set; }

        /// <inheritdoc />
        [DataMember]
        public string Literal { get; set; }

        /// <inheritdoc />
        public Fractional3D ApplyWithTrim(double orgA, double orgB, double orgC)
        {
            return ApplyUntrimmed(orgA, orgB, orgC).TrimToUnitCell(TrimTolerance);
        }

        /// <inheritdoc />
        public Fractional3D ApplyWithTrim(in Fractional3D vector, out Fractional3D trimVector)
        {
            var untrimmed = ApplyUntrimmed(vector);
            var trimmed = untrimmed.TrimToUnitCell(TrimTolerance);
            trimVector = trimmed - untrimmed;
            return trimmed;
        }

        /// <inheritdoc />
        public abstract Fractional3D ApplyUntrimmed(double orgA, double orgB, double orgC);

        /// <inheritdoc />
        public Fractional3D ApplyWithTrim(in Fractional3D vector)
        {
            return ApplyWithTrim(vector.A, vector.B, vector.C);
        }

        /// <inheritdoc />
        public Fractional3D ApplyUntrimmed(in Fractional3D vector)
        {
            return ApplyUntrimmed(vector.A, vector.B, vector.C);
        }

        /// <inheritdoc />
        public Fractional3D ApplyWithTrim(IFractional3D original)
        {
            return new Fractional3D(ApplyWithTrim(original.A, original.B, original.C).Coordinates);
        }

        /// <inheritdoc />
        public Fractional3D ApplyUntrimmed(IFractional3D original)
        {
            return new Fractional3D(ApplyUntrimmed(original.A, original.B, original.C).Coordinates);
        }

        /// <inheritdoc />
        public IEnumerable<Fractional3D> ApplyWithTrim(IEnumerable<Fractional3D> vectors)
        {
            return vectors.Select(value => ApplyWithTrim(value));
        }

        /// <inheritdoc />
        public IEnumerable<Fractional3D> ApplyUntrimmed(IEnumerable<Fractional3D> vectors)
        {
            return vectors.Select(value => ApplyUntrimmed(value));
        }

        /// <inheritdoc />
        public abstract double[] GetOperationsArray();
    }
}