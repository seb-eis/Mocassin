using System.Collections.Generic;
using Mocassin.Mathematics.ValueTypes;

namespace Mocassin.Mathematics.Coordinates
{
    /// <summary>
    ///     Provides vector transformations between the cartesian system, a spherical and a fractional system
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
        ///     Transforms from a <see cref="Fractional3D" /> to a <see cref="Cartesian3D" />
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        Cartesian3D ToCartesian(in Fractional3D vector);

        /// <summary>
        ///     Transforms from a <see cref="Fractional3D" /> to a <see cref="Spherical3D" />
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        Spherical3D ToSpherical(in Fractional3D vector);

        /// <summary>
        ///     Transforms from a <see cref="Fractional3D" /> sequence to a <see cref="Cartesian3D" /> sequence
        /// </summary>
        /// <param name="vectors"></param>
        /// <returns></returns>
        IEnumerable<Cartesian3D> ToCartesian(IEnumerable<Fractional3D> vectors);

        /// <summary>
        ///     Transforms from a <see cref="Fractional3D" /> sequence to a <see cref="Spherical3D" /> sequence
        /// </summary>
        /// <param name="vectors"></param>
        /// <returns></returns>
        IEnumerable<Spherical3D> ToSpherical(IEnumerable<Fractional3D> vectors);

        /// <summary>
        ///     Transforms from a <see cref="Cartesian3D" /> to a <see cref="Fractional3D" />
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        Fractional3D ToFractional(in Cartesian3D vector);

        /// <summary>
        ///     Transforms from a <see cref="Cartesian3D" /> to a <see cref="Spherical3D" />
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        Spherical3D ToSpherical(in Cartesian3D vector);

        /// <summary>
        ///     Transforms from a <see cref="Cartesian3D" /> sequence to a <see cref="Fractional3D" /> sequence
        /// </summary>
        /// <param name="vectors"></param>
        /// <returns></returns>
        IEnumerable<Fractional3D> ToFractional(IEnumerable<Cartesian3D> vectors);

        /// <summary>
        ///     Transforms from a <see cref="Cartesian3D" /> sequence to a <see cref="Spherical3D" /> sequence
        /// </summary>
        /// <param name="vectors"></param>
        /// <returns></returns>
        IEnumerable<Spherical3D> ToSpherical(IEnumerable<Cartesian3D> vectors);

        /// <summary>
        ///     Transforms from a <see cref="Spherical3D" /> to a <see cref="Fractional3D" />
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        Fractional3D ToFractional(in Spherical3D vector);

        /// <summary>
        ///     Transforms from a <see cref="Spherical3D" /> to a <see cref="Cartesian3D" />
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        Cartesian3D ToCartesian(in Spherical3D vector);

        /// <summary>
        ///     Transforms from a <see cref="Spherical3D" /> sequence to a <see cref="Fractional3D" /> sequence
        /// </summary>
        /// <param name="vectors"></param>
        /// <returns></returns>
        IEnumerable<Fractional3D> ToFractional(IEnumerable<Spherical3D> vectors);

        /// <summary>
        ///     Transforms from a <see cref="Spherical3D" /> sequence to a <see cref="Cartesian3D" /> sequence
        /// </summary>
        /// <param name="vectors"></param>
        /// <returns></returns>
        IEnumerable<Cartesian3D> ToCartesian(IEnumerable<Spherical3D> vectors);
    }
}