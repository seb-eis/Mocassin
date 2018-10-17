using System;
using Mocassin.Framework.Extensions;
using Mocassin.Framework.Exceptions;

namespace Mocassin.Mathematics.ValueTypes
{
    /// <summary>
    ///     Generic cartesian 3D mass point (Only supports primitive type masses that support System.Convert)
    /// </summary>
    /// <typeparam name="TMass"></typeparam>
    public readonly struct CartesianMassPoint3D<TMass> : ICartesianMassPoint3D<CartesianMassPoint3D<TMass>>
        where TMass : struct, IConvertible
    {
        /// <summary>
        ///     The raw mass value of the mass point
        /// </summary>
        public TMass Mass { get; }

        /// <inheritdoc />
        public Cartesian3D Vector { get; }

        /// <inheritdoc />
        public double X => Vector.X;

        /// <inheritdoc />
        public double Y => Vector.Y;

        /// <inheritdoc />
        public double Z => Vector.Z;

        /// <inheritdoc />
        public Coordinates<double, double, double> Coordinates => Vector.Coordinates;

        /// <summary>
        ///     Static constructor, checks if the used mass is of primitive type
        /// </summary>
        static CartesianMassPoint3D()
        {
            if (!typeof(TMass).IsPrimitive)
                throw new InvalidGenericTypeException("Basic cartesian mass points only support mass values of primitive type",
                    typeof(TMass));
        }

        /// <summary>
        ///     Creates new mass point from mass and cartesian vector
        /// </summary>
        /// <param name="mass"></param>
        /// <param name="vector"></param>
        public CartesianMassPoint3D(TMass mass, in Cartesian3D vector)
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
        public CartesianMassPoint3D(TMass mass, double x, double y, double z)
            : this(mass, new Cartesian3D(x, y, z))
        {
        }

        /// <inheritdoc />
        public CartesianMassPoint3D<TMass> CreateNew(double a, double b, double c, double mass)
        {
            return new CartesianMassPoint3D<TMass>(ToMassType(mass), new Cartesian3D(a, b, c));
        }

        /// <inheritdoc />
        public CartesianMassPoint3D<TMass> CreateNew(in Coordinates<double, double, double> coordinates, double mass)
        {
            return new CartesianMassPoint3D<TMass>(ToMassType(mass), new Cartesian3D(coordinates));
        }

        /// <inheritdoc />
        public CartesianMassPoint3D<TMass> CreateNew(in Cartesian3D vector, double mass)
        {
            return new CartesianMassPoint3D<TMass>(ToMassType(mass), vector);
        }

        /// <inheritdoc />
        public CartesianMassPoint3D<TMass> CreateNew(double a, double b, double c)
        {
            return new CartesianMassPoint3D<TMass>(Mass, new Cartesian3D(a, b, c));
        }

        /// <inheritdoc />
        public CartesianMassPoint3D<TMass> CreateNew(Coordinates<double, double, double> coordinates)
        {
            return new CartesianMassPoint3D<TMass>(Mass, new Cartesian3D(coordinates));
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