using System.Collections.Generic;
using ICon.Mathematics.ValueTypes;

namespace ICon.Mathematics.Coordinates
{
    /// <summary>
    ///     Transformer between the fractional, cartesian and spherical vector system that supports coordinate systems
    /// </summary>
    public interface IVectorTransformer
    {
        /// <summary>
        ///     The fractional coordinate system (Supports Fractional/Cartesian transformations)
        /// </summary>
        FractionalCoordinateSystem3D FractionalSystem { get; }

        /// <summary>
        ///     The spherical coordinate system (Supports Cartesian/Spherical transformations)
        /// </summary>
        SphericalCoordinateSystem3D SphericalSystem { get; }

        /// <summary>
        ///     Determines the coordinate type of a 3D vector by implemented interface and transform the coordinates into a basic
        ///     cartesian vector
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        Cartesian3D ToCartesian(IVector3D vector);

        /// <summary>
        ///     Determines the coordinate type of a 3D vector by implemented interface and transform the coordinates into a basic
        ///     fractional vector
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        Fractional3D ToFractional(IVector3D vector);

        /// <summary>
        ///     Determines the coordinate type of a 3D vector by implemented interface and transform the coordinates into a basic
        ///     spherical vector
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        Spherical3D ToSpherical(IVector3D vector);

        /// <summary>
        ///     Determines the coordinate type of a sequence of 3D vectors by implemented interface and transform the coordinates
        ///     into a basic cartesian vector sequence
        /// </summary>
        /// <param name="vectors"></param>
        /// <returns></returns>
        IEnumerable<Cartesian3D> ToCartesian(IEnumerable<IVector3D> vectors);

        /// <summary>
        ///     Determines the coordinate type of a sequence of 3D vectors by implemented interface and transform the coordinates
        ///     into a basic fractional vector sequence
        /// </summary>
        /// <param name="vectors"></param>
        /// <returns></returns>
        IEnumerable<Fractional3D> ToFractional(IEnumerable<IVector3D> vectors);

        /// <summary>
        ///     Determines the coordinate type of a sequence of 3D vectors by implemented interface and transform the coordinates
        ///     into a basic cartesian vector sequence
        /// </summary>
        /// <param name="vectors"></param>
        /// <returns></returns>
        IEnumerable<Spherical3D> ToSpherical(IEnumerable<IVector3D> vectors);

        /// <summary>
        ///     Transforms the spherical coordinate info of a spherical vector to a basic cartesian vector
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        Cartesian3D ToCartesian(ISpherical3D vector);

        /// <summary>
        ///     Transforms the spherical coordinate info of a spherical vector sequence to a sequence of basic cartesian vector
        /// </summary>
        /// <param name="vectors"></param>
        /// <returns></returns>
        IEnumerable<Cartesian3D> ToCartesian(IEnumerable<ISpherical3D> vectors);

        /// <summary>
        ///     Transforms the fractional coordinate info of fractional vector to a basic cartesian vector
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        Cartesian3D ToCartesian(IFractional3D vector);

        /// <summary>
        ///     Transforms the fractional coordinate info of fractional vector sequence to a basic cartesian vector sequence
        /// </summary>
        /// <param name="vectors"></param>
        /// <returns></returns>
        IEnumerable<Cartesian3D> ToCartesian(IEnumerable<IFractional3D> vectors);

        /// <summary>
        ///     Transforms the cartesian coordinate info of a cartesian vector to a basic fractional vector
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        Fractional3D ToFractional(ICartesian3D vector);

        /// <summary>
        ///     Transforms the cartesian coordinate info of a cartesian vector sequence to a basic fractional vector sequence
        /// </summary>
        /// <param name="vectors"></param>
        /// <returns></returns>
        IEnumerable<Fractional3D> ToFractional(IEnumerable<ICartesian3D> vectors);

        /// <summary>
        ///     Transforms the spherical coordinate info of a spherical vector to a basic fractional vector
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        Fractional3D ToFractional(ISpherical3D vector);

        /// <summary>
        ///     Transforms the spherical coordinate info of a spherical vector sequence to a basic fractional vector sequence
        /// </summary>
        /// <param name="vectors"></param>
        /// <returns></returns>
        IEnumerable<Fractional3D> ToFractional(IEnumerable<ISpherical3D> vectors);


        /// <summary>
        ///     Transforms the cartesian coordinate info of a cartesian vector to a basic spherical vector
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        Spherical3D ToSpherical(ICartesian3D vector);

        /// <summary>
        ///     Transforms the cartesian coordinate info of a cartesian vector sequence to a basic spherical vector sequence
        /// </summary>
        /// <param name="vectors"></param>
        /// <returns></returns>
        IEnumerable<Spherical3D> ToSpherical(IEnumerable<ICartesian3D> vectors);

        /// <summary>
        ///     Transforms the fractional coordinate info of a fractional vector to a basic spherical vector
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        Spherical3D ToSpherical(IFractional3D vector);

        /// <summary>
        ///     Transforms the fractional coordinate info of a fractional vector sequence to a basic spherical vector sequence
        /// </summary>
        /// <param name="vectors"></param>
        /// <returns></returns>
        IEnumerable<Spherical3D> ToSpherical(IEnumerable<IFractional3D> vectors);
    }
}