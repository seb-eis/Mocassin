using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ICon.Mathematics.ValueTypes;

namespace ICon.Symmetry.SpaceGroups
{
    /// <summary>
    /// Base class for all symmetry operation implementations
    /// </summary>
    [DataContract]
    public abstract class SymmetryOperationBase : ISymmetryOperation
    {
        /// <summary>
        /// The tolerance value for trimming to the unit cell
        /// </summary>
        [DataMember]
        public double TrimTolerance { get; set; }

        /// <summary>
        /// Literal description of the operation
        /// </summary>
        [DataMember]
        public string Literal { get; set; }

        /// <summary>
        /// Applies the symetry operation to an unspecified coordinate point and creates new coordinate information
        /// </summary>
        /// <param name="original"></param>
        /// <returns></returns>
        public Fractional3D ApplyWithTrim(double orgA, double orgB, double orgC)
        {
            return ApplyUntrimmed(orgA, orgB, orgC).TrimToUnitCell(1.0e-10);
        }

        /// <summary>
        /// Applies the symmetry operation to an unspecified coordinate point and creates new coordinate information (Does not trim result into unit cell)
        /// </summary>
        /// <param name="original"></param>
        /// <returns></returns>
        public abstract Fractional3D ApplyUntrimmed(double orgA, double orgB, double orgC);

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
        /// Applies operation to a sequence of fractional vectors with a unit cell trim
        /// </summary>
        /// <param name="vectors"></param>
        /// <returns></returns>
        public IEnumerable<Fractional3D> ApplyWithTrim(IEnumerable<Fractional3D> vectors)
        {
            return vectors.Select(value => ApplyWithTrim(value));
        }

        /// <summary>
        /// Applies operation to a sequence of fractional vectors without a unit cell trim
        /// </summary>
        /// <param name="vectors"></param>
        /// <returns></returns>
        public IEnumerable<Fractional3D> ApplyUntrimmed(IEnumerable<Fractional3D> vectors)
        {
            return vectors.Select(value => ApplyUntrimmed(value));
        }

        /// <summary>
        /// Get the linearized operations array
        /// </summary>
        /// <returns></returns>
        public abstract double[] GetOperationsArray();
    }
}
