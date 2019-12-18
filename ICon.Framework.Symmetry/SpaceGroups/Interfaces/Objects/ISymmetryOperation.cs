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
        ///     Get the <see cref="SymmetryOperationCore"/> reference
        /// </summary>
        ref SymmetryOperationCore Core { get; }

        /// <summary>
        ///     Literal description of the operation in the (x,y,z) + (x_i,y_i,z_i) style
        /// </summary>
        string Literal { get; }

        /// <summary>
        ///     Get a boolean flag if the operation causes an geometry orientation flip
        /// </summary>
        bool FlipsOrientation { get; }

        /// <summary>
        ///     The trim tolerance value
        /// </summary>
        double TrimTolerance { get; }

        /// <summary>
        ///     Applies the symmetry operation to an unspecified coordinate point and creates new coordinate information that is trimmed to the 0,0,0 unit cell
        /// </summary>
        /// <param name="orgA"></param>
        /// <param name="orgB"></param>
        /// <param name="orgC"></param>
        /// <returns></returns>
        Fractional3D TrimTransform(double orgA, double orgB, double orgC);

        /// <summary>
        ///     Applies the symmetry operation to an unspecified coordinate point and creates new coordinate information (Does not
        ///     trim result into unit cell)
        /// </summary>
        /// <param name="orgA"></param>
        /// <param name="orgB"></param>
        /// <param name="orgC"></param>
        /// <returns></returns>
        Fractional3D Transform(double orgA, double orgB, double orgC);

        /// <summary>
        ///     Applies operation to a basic fractional vector with a unit cell trim
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        Fractional3D TrimTransform(in Fractional3D vector);

        /// <summary>
        ///     Applies the symmetry operation to the passed vector, trims it into the unit cell and returns the applied shift
        ///     to create the trim
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="trimVector"></param>
        /// <returns></returns>
        Fractional3D TrimTransform(in Fractional3D vector, out Fractional3D trimVector);

        /// <summary>
        ///     Applies operation to a sequence of fractional vectors with a unit cell trim
        /// </summary>
        /// <param name="vectors"></param>
        /// <returns></returns>
        IEnumerable<Fractional3D> TrimTransform(IEnumerable<Fractional3D> vectors);

        /// <summary>
        ///     Applies operation to a basic fractional vector (Result is not trimmed to unit cell)
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        Fractional3D Transform(in Fractional3D vector);

        /// <summary>
        ///     Applies operation to a sequence of fractional vectors without a unit cell trim
        /// </summary>
        /// <param name="vectors"></param>
        /// <returns></returns>
        IEnumerable<Fractional3D> Transform(IEnumerable<Fractional3D> vectors);

        /// <summary>
        ///     Applies the symmetry operation to a fractional vector (With trim to origin cell)
        /// </summary>
        /// <param name="original"></param>
        /// <returns></returns>
        Fractional3D TrimTransform(IFractional3D original);

        /// <summary>
        ///     Applies the symmetry operation to a fractional vector (No trim to origin cell)
        /// </summary>
        /// <param name="original"></param>
        /// <returns></returns>
        Fractional3D Transform(IFractional3D original);

        /// <summary>
        ///     Get the linearized 12 entry operations array, the entries are ordered as row_1_column_1, row_1_column_2,...
        /// </summary>
        /// <returns></returns>
        double[] GetOperationsArray();
    }
}