using System;
using System.Collections.Generic;
using System.Text;

using ICon.Mathematics.ValueTypes;

namespace ICon.Model.Structures
{
    /// <summary>
    /// Represents a cell position that carries spherical coordinate information and occupation information trough an index
    /// </summary>
    public readonly struct FractionalPosition : IFractionalPosition, IFractionalMassPoint3D<FractionalPosition>
    {
        /// <summary>
        /// Fractional coordinate vector of the position
        /// </summary>
        public Fractional3D Vector { get; }

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
        public double A => Vector.A;

        /// <summary>
        /// The coordinate value in B direction
        /// </summary>
        public double B => Vector.B;

        /// <summary>
        /// The coordinate value in C direction
        /// </summary>
        public double C => Vector.C;

        /// <summary>
        /// Creates new cell position from fractional 3D vector and particle set index
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="occupationIndex"></param>
        /// <param name="status"></param>
        public FractionalPosition(Fractional3D vector, int occupationIndex, PositionStatus status) : this()
        {
            Status = status;
            Vector = vector;
            OccupationIndex = occupationIndex;
        }

        /// <summary>
        /// Creates new cell position with the specfied mass and vector info
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="mass"></param>
        /// <returns></returns>
        public FractionalPosition CreateNew(double a, double b, double c, double mass)
        {
            return new FractionalPosition(new Fractional3D(a, b, c), Convert.ToInt32(mass), Status);
        }

        /// <summary>
        /// Creates new cell position with the specfied mass and vector info
        /// </summary>
        /// <param name="coordinates"></param>
        /// <param name="mass"></param>
        /// <returns></returns>
        public FractionalPosition CreateNew(in Coordinates<double, double, double> coordinates, double mass)
        {
            return new FractionalPosition(new Fractional3D(coordinates), Convert.ToInt32(mass), Status);
        }

        /// <summary>
        /// Cretes new position from vector and mass
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="mass"></param>
        /// <returns></returns>
        public FractionalPosition CreateNew(in Fractional3D vector, double mass)
        {
            return new FractionalPosition(vector, Convert.ToInt32(mass), Status);
        }

        /// <summary>
        /// Creates new cell position with the vector info without changing the particle set index
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public FractionalPosition CreateNew(double a, double b, double c)
        {
            return new FractionalPosition(new Fractional3D(a, b, c), OccupationIndex, Status);
        }

        /// <summary>
        /// Creates new cell position with the vector info without changing the particle set index
        /// </summary>
        /// <param name="coordinates"></param>
        /// <returns></returns>
        public FractionalPosition CreateNew(Coordinates<double, double, double> coordinates)
        {
            return new FractionalPosition(new Fractional3D(coordinates), OccupationIndex, Status);
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
