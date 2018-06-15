using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using ICon.Mathematics.ValueTypes;

namespace ICon.Mathematics.Coordinates
{
    /// <summary>
    /// Basic vector transformer class that supports coordinate transformations between the basic cartesian, fraction and spherical vector
    /// </summary>
    public class VectorTransformer
    {
        /// <summary>
        /// The fractional coordinate system (Supports Fractional/Cartesian transformations)
        /// </summary>
        public FractionalCoordinateSystem3D FractionalSystem { get; protected set; }

        /// <summary>
        /// The spherical coordinate system (Supports Cartesian/Spherical transformations)
        /// </summary>
        public SphericalCoordinateSystem3D SphericalSystem { get; protected set; }

        /// <summary>
        /// Creates new vector transformer with the provided coordinate systems
        /// </summary>
        /// <param name="fractionalSystem"></param>
        /// <param name="sphericalSystem"></param>
        public VectorTransformer(FractionalCoordinateSystem3D fractionalSystem, SphericalCoordinateSystem3D sphericalSystem)
        {
            FractionalSystem = fractionalSystem ?? throw new ArgumentNullException(nameof(fractionalSystem));
            SphericalSystem = sphericalSystem ?? throw new ArgumentNullException(nameof(sphericalSystem));
        }

        /// <summary>
        /// Determines the coordinate type of a generic 3D vector by implemented interface and transform the coordinates into a basic cartesian vector
        /// </summary>
        /// <typeparam name="TVector"></typeparam>
        /// <param name="vector"></param>
        /// <returns></returns>
        public Cartesian3D ToCartesian<TVector>(TVector vector) where TVector : IVector3D
        {
            return TransformAny(vector, (value) => new Cartesian3D(value.Coordinates), CartesianFromFractional, CartesianFromSpherical);
        }

        /// <summary>
        /// Determines the coordinate type of a generic 3D vector by implemented interface and transform the coordinates into a basic fractional vector
        /// </summary>
        /// <typeparam name="TVector"></typeparam>
        /// <param name="vector"></param>
        /// <returns></returns>
        public Fractional3D ToFractional<TVector>(TVector vector) where TVector : IVector3D
        {
            return TransformAny(vector, FractionalFromCartesian, (value) => new Fractional3D(value.Coordinates), FractionalFromSpherical);
        }

        /// <summary>
        /// Determines the coordinate type of a generic 3D vector by implemented interface and transform the coordinates into a basic spherical vector
        /// </summary>
        /// <typeparam name="TVector"></typeparam>
        /// <param name="vector"></param>
        /// <returns></returns>
        public Spherical3D ToSpherical<TVector>(TVector vector) where TVector : IVector3D
        {
            return TransformAny(vector, SphericalFromCartesian, SphericalFromFractional, (value) => new Spherical3D(value.Coordinates));
        }

        /// <summary>
        /// Determines the coordinate type of a generic sequence of 3D vectors by implemented interface and transform the coordinates into a basic cartesian vector sequence
        /// </summary>
        /// <typeparam name="TVector"></typeparam>
        /// <param name="vectors"></param>
        /// <returns></returns>
        public IEnumerable<Cartesian3D> ToCartesian<TVector>(IEnumerable<TVector> vectors) where TVector : IVector3D
        {
            return vectors.Select(vector => ToCartesian(vector));
        }

        /// <summary>
        /// Determines the coordinate type of a generic sequence of 3D vectors by implemented interface and transform the coordinates into a basic fractional vector sequence
        /// </summary>
        /// <typeparam name="TVector"></typeparam>
        /// <param name="vectors"></param>
        /// <returns></returns>
        public IEnumerable<Fractional3D> ToFractional<TVector>(IEnumerable<TVector> vectors) where TVector : IVector3D
        {
            return vectors.Select(vector => ToFractional(vector));
        }

        /// <summary>
        /// Determines the coordinate type of a generic sequence of 3D vectors by implemented interface and transform the coordinates into a basic cartesian vector sequence
        /// </summary>
        /// <typeparam name="TVector"></typeparam>
        /// <param name="vectors"></param>
        /// <returns></returns>
        public IEnumerable<Spherical3D> ToSpherical<TVector>(IEnumerable<TVector> vectors) where TVector : IVector3D
        {
            return vectors.Select(vector => ToSpherical(vector));
        }

        /// <summary>
        /// Transforms the spherical coordinate info of a generic spherical vector to a basic cartesian vector
        /// </summary>
        /// <typeparam name="TVector"></typeparam>
        /// <param name="vector"></param>
        /// <returns></returns>
        public Cartesian3D CartesianFromSpherical<TVector>(TVector vector) where TVector : ISpherical3D
        {
            return new Cartesian3D(SphericalSystem.TransformToReference(vector.Coordinates));
        }

        /// <summary>
        /// Transforms the spherical coordinate info of a generic spherical vector sequence to a sequence of basic cartesian vector
        /// </summary>
        /// <typeparam name="TVector"></typeparam>
        /// <param name="vector"></param>
        /// <returns></returns>
        public IEnumerable<Cartesian3D> CartesianFromSpherical<TVector>(IEnumerable<TVector> vectors) where TVector : ISpherical3D
        {
            return vectors.Select(vector => CartesianFromSpherical(vector));
        }

        /// <summary>
        /// Transforms the fractional coordinate info of generic fractional vector to a basic cartesian vector
        /// </summary>
        /// <typeparam name="TVector"></typeparam>
        /// <param name="vector"></param>
        /// <returns></returns>
        public Cartesian3D CartesianFromFractional<TVector>(TVector vector) where TVector : IFractional3D
        {
            return new Cartesian3D(FractionalSystem.TransformToReference(vector.Coordinates));
        }

        /// <summary>
        /// Transforms the fractional coordinate info of generic fractional vector sequence to a basic cartesian vector sequence
        /// </summary>
        /// <typeparam name="TVector"></typeparam>
        /// <param name="vectors"></param>
        /// <returns></returns>
        public IEnumerable<Cartesian3D> CartesianFromFractional<TVector>(IEnumerable<TVector> vectors) where TVector : IFractional3D
        {
            return vectors.Select(vector => CartesianFromFractional(vector));
        }

        /// <summary>
        /// Transforms the cartesian coordinate info of a generic cartesian vector to a basic fractional vector
        /// </summary>
        /// <typeparam name="TVector"></typeparam>
        /// <param name="vector"></param>
        /// <returns></returns>
        public Fractional3D FractionalFromCartesian<TVector>(TVector vector) where TVector : ICartesian3D
        {
            return new Fractional3D(FractionalSystem.TransformToSystem(vector.Coordinates));
        }

        /// <summary>
        /// Transforms the cartesian coordinate info of a generic cartesian vector sequence to a basic fractional vector sequence
        /// </summary>
        /// <typeparam name="TVector"></typeparam>
        /// <param name="vectors"></param>
        /// <returns></returns>
        public IEnumerable<Fractional3D> FractionalFromCartesian<TVector>(IEnumerable<TVector> vectors) where TVector : ICartesian3D
        {
            return vectors.Select(vector => FractionalFromCartesian(vector));
        }

        /// <summary>
        /// Transforms the spherical coordinate info of a generic spherical vector to a basic fractional vector
        /// </summary>
        /// <typeparam name="TVector"></typeparam>
        /// <param name="vector"></param>
        /// <returns></returns>
        public Fractional3D FractionalFromSpherical<TVector>(TVector vector) where TVector : ISpherical3D
        {
            return new Fractional3D(FractionalSystem.TransformToSystem(CartesianFromSpherical(vector).Coordinates));
        }

        /// <summary>
        /// Transforms the spherical coordinate info of a generic spherical vector sequence to a basic fractional vector sequence
        /// </summary>
        /// <typeparam name="TVector"></typeparam>
        /// <param name="vectors"></param>
        /// <returns></returns>
        public IEnumerable<Fractional3D> FractionalFromSpherical<TVector>(IEnumerable<TVector> vectors) where TVector : ISpherical3D
        {
            return vectors.Select(vector => FractionalFromSpherical(vector));
        }


        /// <summary>
        /// Transforms the cartesian coordinate info of a generic cartesian vector to a basic spherical vector
        /// </summary>
        /// <typeparam name="TVector"></typeparam>
        /// <param name="vector"></param>
        /// <returns></returns>
        public Spherical3D SphericalFromCartesian<TVector>(TVector vector) where TVector : ICartesian3D
        {
            return new Spherical3D(SphericalSystem.TransformToSystem(vector.Coordinates));
        }

        /// <summary>
        /// Transforms the cartesian coordinate info of a generic cartesian vector sequence to a basic spherical vector sequence
        /// </summary>
        /// <typeparam name="TVector"></typeparam>
        /// <param name="vectors"></param>
        /// <returns></returns>
        public IEnumerable<Spherical3D> SphericalFromCartesian<TVector>(IEnumerable<TVector> vectors) where TVector : ICartesian3D
        {
            return vectors.Select(vector => SphericalFromCartesian(vector));
        }

        /// <summary>
        /// Transforms the fractional coordinate info of a generic fractional vector to a basic spherical vector
        /// </summary>
        /// <typeparam name="TVector"></typeparam>
        /// <param name="vector"></param>
        /// <returns></returns>
        public Spherical3D SphericalFromFractional<TVector>(TVector vector) where TVector : IFractional3D
        {
            return new Spherical3D(SphericalSystem.TransformToSystem(CartesianFromFractional(vector).Coordinates));
        }

        /// <summary>
        /// Transforms the fractional coordinate info of a generic fractional vector sequence to a basic spherical vector sequence
        /// </summary>
        /// <typeparam name="TVector"></typeparam>
        /// <param name="vectors"></param>
        /// <returns></returns>
        public IEnumerable<Spherical3D> SphericalFromFractional<TVector>(IEnumerable<TVector> vectors) where TVector : IFractional3D
        {
            return vectors.Select(vector => SphericalFromFractional(vector));
        }

        /// <summary>
        /// Transforms any vector to the specified type suing the provided conversion delegates (Throws if interface is not supported)
        /// </summary>
        /// <typeparam name="TVector"></typeparam>
        /// <param name="vector"></param>
        /// <param name="onCartesian"></param>
        /// <param name="onFractional"></param>
        /// <param name="onSpherical"></param>
        /// <returns></returns>
        protected TVector TransformAny<TVector>(IVector3D vector, Func<ICartesian3D, TVector> onCartesian, Func<IFractional3D, TVector> onFractional, Func<ISpherical3D, TVector> onSpherical)
        {
            if (vector is IFractional3D fractional)
            {
                return onFractional(fractional);
            }
            if (vector is ISpherical3D spherical)
            {
                return onSpherical(spherical);
            }
            if (vector is ICartesian3D cartesian)
            {
                return onCartesian(cartesian);
            }
            throw new ArgumentException("Argument implements none of the supported interfaces", nameof(vector));
        }
    }
}
