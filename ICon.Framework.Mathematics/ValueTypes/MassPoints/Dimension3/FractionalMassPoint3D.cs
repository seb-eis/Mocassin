using System;
using Mocassin.Framework.Extensions;
using Mocassin.Framework.Exceptions;

namespace Mocassin.Mathematics.ValueTypes
{
    /// <summary>
    ///     Generic basic fractional mass point that specifies a 3D fractional vector with additional information
    /// </summary>
    /// <typeparam name="TMass"></typeparam>
    public readonly struct FractionalMassPoint3D<TMass> : IFractionalMassPoint3D<FractionalMassPoint3D<TMass>>
        where TMass : struct, IConvertible
    {
        /// <summary>
        ///     The raw mass value of the mass point
        /// </summary>
        public TMass Mass { get; }

        public Fractional3D Vector { get; }

        /// <inheritdoc />
        public double A => Vector.A;

        /// <inheritdoc />
        public double B => Vector.B;

        /// <inheritdoc />
        public double C => Vector.C;

        /// <inheritdoc />
        public Coordinates<double, double, double> Coordinates => Vector.Coordinates;

        /// <summary>
        ///     Static constructor, checks if the used mass is of primitive type
        /// </summary>
        static FractionalMassPoint3D()
        {
            if (!typeof(TMass).IsPrimitive)
                throw new InvalidGenericTypeException("Basic fractional mass points only suppport mass values of primitive type",
                    typeof(TMass));
        }

        /// <summary>
        ///     Creates new mass point from mass and fractional vector
        /// </summary>
        /// <param name="mass"></param>
        /// <param name="vector"></param>
        public FractionalMassPoint3D(TMass mass, in Fractional3D vector)
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
        public FractionalMassPoint3D(TMass mass, double x, double y, double z)
            : this(mass, new Fractional3D(x, y, z))
        {
        }

        /// <inheritdoc />
        public FractionalMassPoint3D<TMass> CreateNew(double a, double b, double c, double mass)
        {
            return new FractionalMassPoint3D<TMass>(ToMassType(mass), new Fractional3D(a, b, c));
        }

        /// <inheritdoc />
        public FractionalMassPoint3D<TMass> CreateNew(in Coordinates<double, double, double> coordinates, double mass)
        {
            return new FractionalMassPoint3D<TMass>(ToMassType(mass), new Fractional3D(coordinates));
        }

        /// <inheritdoc />
        public FractionalMassPoint3D<TMass> CreateNew(in Fractional3D vector, double mass)
        {
            return new FractionalMassPoint3D<TMass>(ToMassType(mass), vector);
        }

        /// <inheritdoc />
        public FractionalMassPoint3D<TMass> CreateNew(double a, double b, double c)
        {
            return new FractionalMassPoint3D<TMass>(Mass, new Fractional3D(a, b, c));
        }

        /// <inheritdoc />
        public FractionalMassPoint3D<TMass> CreateNew(Coordinates<double, double, double> coordinates)
        {
            return new FractionalMassPoint3D<TMass>(Mass, new Fractional3D(coordinates));
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