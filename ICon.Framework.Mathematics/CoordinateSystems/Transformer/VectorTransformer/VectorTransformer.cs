using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using ICon.Mathematics.ValueTypes;

namespace ICon.Mathematics.Coordinates
{
    /// <inheritdoc />
    public class VectorTransformer : IVectorTransformer
    {
        /// <inheritdoc />
        public FractionalCoordinateSystem3D FractionalSystem { get; protected set; }

        /// <inheritdoc />
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

        /// <inheritdoc />
        public Cartesian3D ToCartesian(IVector3D vector)
        {
            return TransformAny(vector, value => new Cartesian3D(value.Coordinates), ToCartesian, ToCartesian);
        }

        /// <inheritdoc />
        public Fractional3D ToFractional(IVector3D vector)
        {
            return TransformAny(vector, ToFractional, value => new Fractional3D(value.Coordinates), ToFractional);
        }

        /// <inheritdoc />
        public Spherical3D ToSpherical(IVector3D vector)
        {
            return TransformAny(vector, ToSpherical, ToSpherical, value => new Spherical3D(value.Coordinates));
        }

        /// <inheritdoc />
        public IEnumerable<Cartesian3D> ToCartesian(IEnumerable<IVector3D> vectors)
        {
            return vectors.Select(ToCartesian);
        }

        /// <inheritdoc />
        public IEnumerable<Fractional3D> ToFractional(IEnumerable<IVector3D> vectors)
        {
            return vectors.Select(ToFractional);
        }

        /// <inheritdoc />
        public IEnumerable<Spherical3D> ToSpherical(IEnumerable<IVector3D> vectors)
        {
            return vectors.Select(ToSpherical);
        }

        /// <inheritdoc />
        public Cartesian3D ToCartesian(ISpherical3D vector)
        {
            return new Cartesian3D(SphericalSystem.TransformToReference(vector.Coordinates));
        }

        /// <inheritdoc />
        public IEnumerable<Cartesian3D> ToCartesian(IEnumerable<ISpherical3D> vectors)
        {
            return vectors.Select(ToCartesian);
        }

        /// <inheritdoc />
        public Cartesian3D ToCartesian(IFractional3D vector)
        {
            return new Cartesian3D(FractionalSystem.TransformToReference(vector.Coordinates));
        }

        /// <inheritdoc />
        public IEnumerable<Cartesian3D> ToCartesian(IEnumerable<IFractional3D> vectors)
        {
            return vectors.Select(ToCartesian);
        }

        /// <inheritdoc />
        public Fractional3D ToFractional(ICartesian3D vector)
        {
            return new Fractional3D(FractionalSystem.TransformToSystem(vector.Coordinates));
        }

        /// <inheritdoc />
        public IEnumerable<Fractional3D> ToFractional(IEnumerable<ICartesian3D> vectors)
        {
            return vectors.Select(ToFractional);
        }

        /// <inheritdoc />
        public Fractional3D ToFractional(ISpherical3D vector)
        {
            return new Fractional3D(FractionalSystem.TransformToSystem(ToCartesian(vector).Coordinates));
        }

        /// <inheritdoc />
        public IEnumerable<Fractional3D> ToFractional(IEnumerable<ISpherical3D> vectors)
        {
            return vectors.Select(ToFractional);
        }

        /// <inheritdoc />
        public Spherical3D ToSpherical(ICartesian3D vector)
        {
            return new Spherical3D(SphericalSystem.TransformToSystem(vector.Coordinates));
        }

        /// <inheritdoc />
        public IEnumerable<Spherical3D> ToSpherical(IEnumerable<ICartesian3D> vectors)
        {
            return vectors.Select(ToSpherical);
        }

        /// <inheritdoc />
        public Spherical3D ToSpherical(IFractional3D vector)
        {
            return new Spherical3D(SphericalSystem.TransformToSystem(ToCartesian(vector).Coordinates));
        }

        /// <inheritdoc />
        public IEnumerable<Spherical3D> ToSpherical(IEnumerable<IFractional3D> vectors)
        {
            return vectors.Select(ToSpherical);
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
            switch (vector)
            {
                case IFractional3D fractional:
                    return onFractional(fractional);

                case ISpherical3D spherical:
                    return onSpherical(spherical);

                case ICartesian3D cartesian:
                    return onCartesian(cartesian);

                default:
                    throw new ArgumentException("Argument implements none of the supported interfaces", nameof(vector));
            }
        }
    }
}
