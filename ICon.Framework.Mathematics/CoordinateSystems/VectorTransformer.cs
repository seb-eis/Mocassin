using System;
using System.Collections.Generic;
using System.Linq;
using Mocassin.Mathematics.ValueTypes;

namespace Mocassin.Mathematics.Coordinates
{
    /// <inheritdoc />
    public class VectorTransformer : IVectorTransformer
    {
        /// <inheritdoc />
        public FractionalCoordinateSystem3D FractionalSystem { get; }

        /// <inheritdoc />
        public SphericalCoordinateSystem3D SphericalSystem { get; }

        /// <summary>
        ///     Creates new vector transformer with the provided coordinate systems
        /// </summary>
        /// <param name="fractionalSystem"></param>
        /// <param name="sphericalSystem"></param>
        public VectorTransformer(FractionalCoordinateSystem3D fractionalSystem, SphericalCoordinateSystem3D sphericalSystem)
        {
            FractionalSystem = fractionalSystem ?? throw new ArgumentNullException(nameof(fractionalSystem));
            SphericalSystem = sphericalSystem ?? throw new ArgumentNullException(nameof(sphericalSystem));
        }

        /// <inheritdoc />
        public Cartesian3D ToCartesian(in Fractional3D vector) => FractionalSystem.ToReferenceSystem(vector);

        /// <inheritdoc />
        public Spherical3D ToSpherical(in Fractional3D vector) => SphericalSystem.ToSystem(ToCartesian(vector));

        /// <inheritdoc />
        public IEnumerable<Cartesian3D> ToCartesian(IEnumerable<Fractional3D> vectors)
        {
            return vectors.Select(x => ToCartesian(x));
        }

        /// <inheritdoc />
        public IEnumerable<Spherical3D> ToSpherical(IEnumerable<Fractional3D> vectors)
        {
            return vectors.Select(x => ToSpherical(x));
        }

        /// <inheritdoc />
        public Fractional3D ToFractional(in Cartesian3D vector) => FractionalSystem.ToSystem(vector);

        /// <inheritdoc />
        public Spherical3D ToSpherical(in Cartesian3D vector) => SphericalSystem.ToSystem(vector);

        /// <inheritdoc />
        public IEnumerable<Fractional3D> ToFractional(IEnumerable<Cartesian3D> vectors)
        {
            return vectors.Select(x => ToFractional(x));
        }

        /// <inheritdoc />
        public IEnumerable<Spherical3D> ToSpherical(IEnumerable<Cartesian3D> vectors)
        {
            return vectors.Select(x => ToSpherical(in x));
        }

        /// <inheritdoc />
        public Fractional3D ToFractional(in Spherical3D vector) => FractionalSystem.ToSystem(ToCartesian(vector));

        /// <inheritdoc />
        public Cartesian3D ToCartesian(in Spherical3D vector) => SphericalSystem.ToReferenceSystem(vector);

        /// <inheritdoc />
        public IEnumerable<Fractional3D> ToFractional(IEnumerable<Spherical3D> vectors)
        {
            return vectors.Select(x => ToFractional(x));
        }

        /// <inheritdoc />
        public IEnumerable<Cartesian3D> ToCartesian(IEnumerable<Spherical3D> vectors)
        {
            return vectors.Select(x => ToCartesian(x));
        }
    }
}