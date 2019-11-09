using System;
using Mocassin.Mathematics.ValueTypes;

namespace Mocassin.Model.Structures
{
    /// <inheritdoc cref="Mocassin.Model.Structures.ICartesianAtom"/>
    public readonly struct CartesianAtom : ICartesianAtom, ICartesianMassPoint3D
    {
        /// <inheritdoc />
        public Cartesian3D Vector { get; }

        /// <inheritdoc />
        public int ParticleIndex { get; }

        /// <inheritdoc />
        public double Mass => ParticleIndex;

        /// <inheritdoc />
        public double X => Vector.X;

        /// <inheritdoc />
        public double Y => Vector.Y;

        /// <inheritdoc />
        public double Z => Vector.Z;

        /// <inheritdoc />
        public Coordinates3D Coordinates => Vector.Coordinates;

        /// <summary>
        ///     Creates new cartesian atom from vector and particle index
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="particleIndex"></param>
        public CartesianAtom(in Cartesian3D vector, int particleIndex)
            : this()
        {
            Vector = vector;
            ParticleIndex = particleIndex;
        }
    }
}