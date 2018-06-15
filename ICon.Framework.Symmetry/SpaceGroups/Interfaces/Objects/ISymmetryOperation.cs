using System;
using System.Collections.Generic;
using System.Text;

using ICon.Mathematics.ValueTypes;

namespace ICon.Symmetry.SpaceGroups
{
    /// <summary>
    /// Represents a symmetry operation for fractional vectors in a translation invariant system
    /// </summary>
    public interface ISymmetryOperation
    {
        /// <summary>
        /// Literal description of the operation in the (x,y,z) + (x_i,y_i,z_i) style
        /// </summary>
        string Literal { get; }

        /// <summary>
        /// The trim tolerance value
        /// </summary>
        double TrimTolerance { get; }

        /// <summary>
        /// Get the linearized 12 entry operations array
        /// </summary>
        /// <returns></returns>
        double[] GetOperationsArray();

        /// <summary>
        /// Applies the symetry operation to an unspecified coordinate point and creates new coordinate information
        /// </summary>
        /// <param name="original"></param>
        /// <returns></returns>
        Fractional3D ApplyWithTrim(Double orgA, Double orgB, Double orgC);

        /// <summary>
        /// Applies the symmetry operation to an unspecified coordinate point and creates new coordinate information (Does not trim result into unit cell)
        /// </summary>
        /// <param name="original"></param>
        /// <returns></returns>
        Fractional3D ApplyUntrimmed(Double orgA, Double orgB, Double orgC);

        /// <summary>
        /// Applies operation to a basic fractional vector
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        Fractional3D ApplyWithTrim(in Fractional3D vector);

        /// <summary>
        /// Applies operation to a basic fractional vector (Result is not trimmed to unit cell)
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        Fractional3D ApplyUntrimmed(in Fractional3D vector);

        /// <summary>
        /// Applies the symmetry operation to a 64 bit generic fractional vector (With trim to origin cell)
        /// </summary>
        /// <param name="original"></param>
        /// <returns></returns>
        T1 ApplyWithTrim<T1>(in T1 original) where T1 : struct, IFractional3D<T1>;

        /// <summary>
        /// Applies the symmetry operation to a 64 bit generic fractional vector (No trim to origin cell)
        /// </summary>
        /// <param name="original"></param>
        /// <returns></returns>
        T1 ApplyUntrimmed<T1>(in T1 original) where T1 : struct, IFractional3D<T1>;
    }
}
