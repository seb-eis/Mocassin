using System;
using Mocassin.Mathematics.ValueTypes;

namespace Mocassin.Model.Structures
{
    /// <inheritdoc cref="Mocassin.Model.Structures.ICartesianAtom"/>
    public readonly struct CartesianAtom : ICartesianAtom, ICartesianMassPoint3D<CartesianAtom>
    {
        /// <inheritdoc />
        public Cartesian3D Vector { get; }

        /// <inheritdoc />
        public int ParticleIndex { get; }

        /// <inheritdoc />
        public double X => Vector.X;

        /// <inheritdoc />
        public double Y => Vector.Y;

        /// <inheritdoc />
        public double Z => Vector.Z;

        /// <inheritdoc />
        public Coordinates<double, double, double> Coordinates => Vector.Coordinates;

        /// <summary>
        ///     Creates new cartesian atom from vector and particle index
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="particleIndex"></param>
        public CartesianAtom(Cartesian3D vector, int particleIndex)
            : this()
        {
            Vector = vector;
            ParticleIndex = particleIndex;
        }

        /// <inheritdoc />
        public CartesianAtom CreateNew(double a, double b, double c, double mass)
        {
            return new CartesianAtom(new Cartesian3D(a, b, c), Convert.ToInt32(mass));
        }

        /// <inheritdoc />
        public CartesianAtom CreateNew(in Coordinates<double, double, double> coordinates, double mass)
        {
            return new CartesianAtom(new Cartesian3D(coordinates), Convert.ToInt32(mass));
        }

        /// <inheritdoc />
        public CartesianAtom CreateNew(in Cartesian3D vector, double mass)
        {
            return new CartesianAtom(vector, Convert.ToInt32(mass));
        }

        /// <inheritdoc />
        public CartesianAtom CreateNew(double a, double b, double c)
        {
            return new CartesianAtom(new Cartesian3D(a, b, c), ParticleIndex);
        }

        /// <inheritdoc />
        public CartesianAtom CreateNew(Coordinates<double, double, double> coordinates)
        {
            return new CartesianAtom(new Cartesian3D(coordinates), ParticleIndex);
        }

        /// <inheritdoc />
        public double GetMass()
        {
            return ParticleIndex;
        }
    }
}