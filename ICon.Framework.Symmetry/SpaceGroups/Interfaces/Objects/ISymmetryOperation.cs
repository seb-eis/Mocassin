using System.Collections.Generic;
using Mocassin.Mathematics.ValueTypes;

namespace Mocassin.Symmetry.SpaceGroups
{
    /// <summary>
    ///     Represents a symmetry operation for fractional vectors in a translation invariant system
    /// </summary>
    public interface ISymmetryOperation
    {
        /// <summary>
        ///     Literal description of the operation in the (x,y,z) + (x_i,y_i,z_i) style
        /// </summary>
        string Literal { get; }

        /// <summary>
        ///     The trim tolerance value
        /// </summary>
        double TrimTolerance { get; }

        /// <summary>
        ///     Get the linearized 12 entry operations array, the entries are ordered as row_1_column_1, row_1_column_2,...
        /// </summary>
        /// <returns></returns>
        double[] GetOperationsArray();

        /// <summary>
        ///     Applies the symmetry operation to an unspecified coordinate point and creates new coordinate information
        /// </summary>
        /// <param name="orgA"></param>
        /// <param name="orgB"></param>
        /// <param name="orgC"></param>
        /// <returns></returns>
        Fractional3D ApplyWithTrim(double orgA, double orgB, double orgC);

        /// <summary>
        ///     Applies the symmetry operation to an unspecified coordinate point and creates new coordinate information (Does not
        ///     trim result into unit cell)
        /// </summary>
        /// <param name="orgA"></param>
        /// <param name="orgB"></param>
        /// <param name="orgC"></param>
        /// <returns></returns>
        Fractional3D ApplyUntrimmed(double orgA, double orgB, double orgC);

        /// <summary>
        ///     Applies operation to a basic fractional vector with a unit cell trim
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        Fractional3D ApplyWithTrim(in Fractional3D vector);

        /// <summary>
        ///     Applies the symmetry operation to the passed vector, trims it into the unit cell and returns the applied shift
        ///     to create the trim
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="trimVector"></param>
        /// <returns></returns>
        Fractional3D ApplyWithTrim(in Fractional3D vector, out Fractional3D trimVector);

        /// <summary>
        ///     Applies operation to a sequence of fractional vectors with a unit cell trim
        /// </summary>
        /// <param name="vectors"></param>
        /// <returns></returns>
        IEnumerable<Fractional3D> ApplyWithTrim(IEnumerable<Fractional3D> vectors);

        /// <summary>
        ///     Applies operation to a basic fractional vector (Result is not trimmed to unit cell)
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        Fractional3D ApplyUntrimmed(in Fractional3D vector);

        /// <summary>
        ///     Applies operation to a sequence of fractional vectors without a unit cell trim
        /// </summary>
        /// <param name="vectors"></param>
        /// <returns></returns>
        IEnumerable<Fractional3D> ApplyUntrimmed(IEnumerable<Fractional3D> vectors);

        /// <summary>
        ///     Applies the symmetry operation to a 64 bit generic fractional vector (With trim to origin cell)
        /// </summary>
        /// <param name="original"></param>
        /// <returns></returns>
        T1 ApplyWithTrim<T1>(in T1 original) where T1 : struct, IFractional3D<T1>;

        /// <summary>
        ///     Applies the symmetry operation to a 64 bit generic fractional vector (No trim to origin cell)
        /// </summary>
        /// <param name="original"></param>
        /// <returns></returns>
        T1 ApplyUntrimmed<T1>(in T1 original) where T1 : struct, IFractional3D<T1>;
    }
}