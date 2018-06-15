using System;
using System.Collections.Generic;
using System.Text;

using ICon.Mathematics.ValueTypes;

namespace ICon.Model.Structures
{
    /// <summary>
    /// Represents a position with specified occupation that carries cartesian coordinates and a particle index
    /// </summary>
    public readonly struct CartesianAtom : ICartesianAtom, ICartesianMassPoint3D<CartesianAtom>
    {
        /// <summary>
        /// The cartesian position vector
        /// </summary>
        public Cartesian3D Vector { get; }

        /// <summary>
        /// The particle index of the actual occupation
        /// </summary>
        public Int32 ParticleIndex { get; }

        /// <summary>
        /// The X coordinate value
        /// </summary>
        public Double X => Vector.X;

        /// <summary>
        /// The Y coordinate value
        /// </summary>
        public Double Y => Vector.Y;

        /// <summary>
        /// The Z coordinaet value
        /// </summary>
        public Double Z => Vector.Z;

        /// <summary>
        /// The unspecified coordinate tuple of the internal vector
        /// </summary>
        public Coordinates<Double, Double, Double> Coordinates => Vector.Coordinates;

        /// <summary>
        /// Creates new cartesian atom from vector and particle index
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="particleIndex"></param>
        public CartesianAtom(Cartesian3D vector, Int32 particleIndex) : this()
        {
            Vector = vector;
            ParticleIndex = particleIndex;
        }

        /// <summary>
        /// Factory to create new cartesian atom from coordinate values and a mass
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="mass"></param>
        /// <returns></returns>
        public CartesianAtom CreateNew(Double a, Double b, Double c, Double mass)
        {
            return new CartesianAtom(new Cartesian3D(a, b, c), Convert.ToInt32(mass));
        }

        /// <summary>
        /// Factory to create new cartesian atom from coordinaet tuple and mass
        /// </summary>
        /// <param name="coordinates"></param>
        /// <param name="mass"></param>
        /// <returns></returns>
        public CartesianAtom CreateNew(in Coordinates<Double, Double, Double> coordinates, Double mass)
        {
            return new CartesianAtom(new Cartesian3D(coordinates), Convert.ToInt32(mass));
        }

        /// <summary>
        /// Factory to create new cartesian atom from vector and mass
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="mass"></param>
        /// <returns></returns>
        public CartesianAtom CreateNew(in Cartesian3D vector, Double mass)
        {
            return new CartesianAtom(vector, Convert.ToInt32(mass));
        }

        /// <summary>
        /// Factory to create new cartesian atom from coordinates while keeping the current particle index
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public CartesianAtom CreateNew(Double a, Double b, Double c)
        {
            return new CartesianAtom(new Cartesian3D(a, b, c), ParticleIndex);
        }

        /// <summary>
        /// Factory to create new cartesian atom from coordinates while keeping the current particle index
        /// </summary>
        /// <param name="coordinates"></param>
        /// <returns></returns>
        public CartesianAtom CreateNew(Coordinates<Double, Double, Double> coordinates)
        {
            return new CartesianAtom(new Cartesian3D(coordinates), ParticleIndex);
        }

        /// <summary>
        /// Get the particle index as a mass value
        /// </summary>
        /// <returns></returns>
        public Double GetMass()
        {
            return ParticleIndex;
        }
    }
}
