using System;
using Mocassin.Framework.Extensions;
using Mocassin.Framework.Exceptions;

namespace Mocassin.Mathematics.ValueTypes
{
    /// <summary>
    ///     Generic basic spherical mass point that specifies a 3D spherical vector with additional information
    /// </summary>
    /// <typeparam name="TMass"></typeparam>
    public readonly struct SphericalMassPoint3D<TMass> : ISphericalMassPoint3D<SphericalMassPoint3D<TMass>>
        where TMass : struct, IConvertible
    {
        /// <summary>
        ///     The raw mass value of the mass point
        /// </summary>
        public TMass Mass { get; }

        /// <inheritdoc />
        public Spherical3D Vector { get; }

        /// <inheritdoc />
        public double Radius => Vector.Radius;

        /// <inheritdoc />
        public double Theta => Vector.Theta;

        /// <inheritdoc />
        public double Phi => Vector.Phi;

        /// <inheritdoc />
        public Coordinates<double, double, double> Coordinates => Vector.Coordinates;

        /// <summary>
        ///     Static constructor, checks if the used mass is of primitive type
        /// </summary>
        static SphericalMassPoint3D()
        {
            if (!typeof(TMass).IsPrimitive)
                throw new InvalidGenericTypeException("Basic spherical mass points only support mass values of primitive type",
                    typeof(TMass));
        }

        /// <summary>
        ///     Creates new mass point from mass and spherical vector
        /// </summary>
        /// <param name="mass"></param>
        /// <param name="vector"></param>
        public SphericalMassPoint3D(TMass mass, in Spherical3D vector)
            : this()
        {
            Mass = mass;
            Vector = vector;
        }

        /// <summary>
        ///     Creates new mass point from mass and coordinate infos
        /// </summary>
        /// <param name="mass"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public SphericalMassPoint3D(TMass mass, double x, double y, double z)
            : this(mass, new Spherical3D(x, y, z))
        {
        }

        /// <inheritdoc />
        public SphericalMassPoint3D<TMass> CreateNew(double a, double b, double c, double mass)
        {
            return new SphericalMassPoint3D<TMass>(ToMassType(mass), new Spherical3D(a, b, c));
        }

        /// <inheritdoc />
        public SphericalMassPoint3D<TMass> CreateNew(in Coordinates<double, double, double> coordinates, double mass)
        {
            return new SphericalMassPoint3D<TMass>(ToMassType(mass), new Spherical3D(coordinates));
        }

        /// <inheritdoc />
        public SphericalMassPoint3D<TMass> CreateNew(in Spherical3D vector, double mass)
        {
            return new SphericalMassPoint3D<TMass>(ToMassType(mass), vector);
        }

        /// <inheritdoc />
        public SphericalMassPoint3D<TMass> CreateNew(double a, double b, double c)
        {
            return new SphericalMassPoint3D<TMass>(Mass, new Spherical3D(a, b, c));
        }

        /// <inheritdoc />
        public SphericalMassPoint3D<TMass> CreateNew(Coordinates<double, double, double> coordinates)
        {
            return new SphericalMassPoint3D<TMass>(Mass, new Spherical3D(coordinates));
        }

        /// <inheritdoc />
        public double GetMass()
        {
            return Mass.ToPrimitive<TMass, double>();
        }

        /// <summary>
        ///     Culture invariant conversion of the double value to the internal mass type
        /// </summary>
        /// <param name="mass"></param>
        /// <returns></returns>
        public TMass ToMassType(double mass)
        {
            return mass.ToPrimitive<double, TMass>();
        }
    }
}