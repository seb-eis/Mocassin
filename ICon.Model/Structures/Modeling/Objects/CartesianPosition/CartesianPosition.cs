using System;
using System.Collections.Generic;
using System.Text;

using ICon.Mathematics.ValueTypes;

namespace ICon.Model.Structures
{
    /// <summary>
    /// Represents a cell position that carries cartesian coordinate information and occupation information trough an index
    /// </summary>
    public readonly struct CartesianPosition : ICartesianPosition, ICartesianMassPoint3D<CartesianPosition>
    {
        /// <summary>
        /// Cartesian coordinate vector of the position
        /// </summary>
        public Cartesian3D Vector { get; }

        /// <summary>
        /// The particle set index that describes possible occupations
        /// </summary>
        public int OccupationIndex { get; }

        /// <summary>
        /// The status of the position
        /// </summary>
        public PositionStatus Status { get; }

        /// <summary>
        /// The coordinate tuple of the internal vector
        /// </summary>
        public Coordinates<double, double, double> Coordinates => Vector.Coordinates;

        /// <summary>
        /// The coordinate value in A direction
        /// </summary>
        public double A => Vector.X;

        /// <summary>
        /// The coordinate value in B direction
        /// </summary>
        public double B => Vector.Y;

        /// <summary>
        /// The coordinate value in C direction
        /// </summary>
        public double C => Vector.Z;

        /// <summary>
        /// The X coordinate
        /// </summary>
        public double X => Vector.X;

        /// <summary>
        /// The Y coordinate
        /// </summary>
        public double Y => Vector.Y;

        /// <summary>
        /// The Z coordinate
        /// </summary>
        public double Z => Vector.Z;

        /// <summary>
        /// Creates new cell position from cartesian 3D vector and particle set index
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="particleSetIndex"></param>
        /// <param name="status"></param>
        public CartesianPosition(Cartesian3D vector, int particleSetIndex, PositionStatus status) : this()
        {
            Status = status;
            Vector = vector;
            OccupationIndex = particleSetIndex;
        }

        /// <summary>
        /// Creates new cell position with the specfied mass and vector info
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="mass"></param>
        /// <returns></returns>
        public CartesianPosition CreateNew(double x, double y, double z, double mass)
        {
            return new CartesianPosition(new Cartesian3D(x, y, z), Convert.ToInt32(mass), Status);
        }

        /// <summary>
        /// Creates new cell position with the specfied mass and vector info
        /// </summary>
        /// <param name="coordinates"></param>
        /// <param name="mass"></param>
        /// <returns></returns>
        public CartesianPosition CreateNew(in Coordinates<double, double, double> coordinates, double mass)
        {
            return new CartesianPosition(new Cartesian3D(coordinates), Convert.ToInt32(mass), Status);
        }

        /// <summary>
        /// Cretes new cell position from vector and mass
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="mass"></param>
        /// <returns></returns>
        public CartesianPosition CreateNew(in Cartesian3D vector, double mass)
        {
            return new CartesianPosition(vector, Convert.ToInt32(mass), Status);
        }

        /// <summary>
        /// Creates new cell position with the vector info without changing the particle set index
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public CartesianPosition CreateNew(double x, double y, double z)
        {
            return new CartesianPosition(new Cartesian3D(x, y, z), OccupationIndex, Status);
        }

        /// <summary>
        /// Creates new cell position with the vector info without changing the particle set index
        /// </summary>
        /// <param name="coordinates"></param>
        /// <returns></returns>
        public CartesianPosition CreateNew(Coordinates<double, double, double> coordinates)
        {
            return new CartesianPosition(new Cartesian3D(coordinates), OccupationIndex, Status);
        }

        /// <summary>
        /// Get the particle index as a mass value, if particle index is -1 than the particle set index is returned
        /// </summary>
        /// <returns></returns>
        public double GetMass()
        {
            return OccupationIndex;
        }
    }
}
