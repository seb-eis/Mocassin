using System;
using System.Collections.Generic;
using System.Text;
using Mocassin.Mathematics.ValueTypes;

namespace Mocassin.Model.Structures
{
    /// <summary>
    /// Represents a cell position that carries spherical coordinate information and occupation information trough an index
    /// </summary>
    public readonly struct SphericalPosition : ISphericalPosition, ISphericalMassPoint3D<SphericalPosition>
    {
        /// <summary>
        /// Spherical coordinate vector of the position
        /// </summary>
        public Spherical3D Vector { get; }

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
        public double A => Vector.Radius;

        /// <summary>
        /// The coordinate value in B direction
        /// </summary>
        public double B => Vector.Theta;

        /// <summary>
        /// The coordinate value in C direction
        /// </summary>
        public double C => Vector.Phi;

        /// <summary>
        /// The radius coordinate value
        /// </summary>
        public double Radius => Vector.Radius;

        /// <summary>
        /// The polar angle coordinate
        /// </summary>
        public double Theta => Vector.Theta;

        /// <summary>
        /// The azimuthal angle coordinate
        /// </summary>
        public double Phi => Vector.Phi;

        /// <summary>
        /// Creates new cell position from cartesian 3D vector and particle set index
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="occupationIndex"></param>
        /// <param name="particleIndex"></param>
        public SphericalPosition(Spherical3D vector, int occupationIndex, PositionStatus status) : this()
        {
            Status = status;
            Vector = vector;
            OccupationIndex = occupationIndex;
        }

        /// <summary>
        /// Creates new cell position with the specfied mass and vector info
        /// </summary>
        /// <param name="radius"></param>
        /// <param name="theta"></param>
        /// <param name="phi"></param>
        /// <param name="mass"></param>
        /// <returns></returns>
        public SphericalPosition CreateNew(double radius, double theta, double phi, double mass)
        {
            return new SphericalPosition(new Spherical3D(radius, theta, phi), Convert.ToInt32(mass), Status);
        }

        /// <summary>
        /// Creates new cell position with the specfied mass and vector info
        /// </summary>
        /// <param name="coordinates"></param>
        /// <param name="mass"></param>
        /// <returns></returns>
        public SphericalPosition CreateNew(in Coordinates<double, double, double> coordinates, double mass)
        {
            return new SphericalPosition(new Spherical3D(coordinates), Convert.ToInt32(mass), Status);
        }

        /// <summary>
        /// Cretes new position from vector and mass
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="mass"></param>
        /// <returns></returns>
        public SphericalPosition CreateNew(in Spherical3D vector, double mass)
        {
            return new SphericalPosition(vector, Convert.ToInt32(mass), Status);
        }

        /// <summary>
        /// Creates new cell position with the vector info without changing the particle set index
        /// </summary>
        /// <param name="radius"></param>
        /// <param name="theta"></param>
        /// <param name="phi"></param>
        /// <returns></returns>
        public SphericalPosition CreateNew(double radius, double theta, double phi)
        {
            return new SphericalPosition(new Spherical3D(radius, theta, phi), OccupationIndex, Status);
        }

        /// <summary>
        /// Creates new cell position with the vector info without changing the particle set index
        /// </summary>
        /// <param name="coordinates"></param>
        /// <returns></returns>
        public SphericalPosition CreateNew(Coordinates<double, double, double> coordinates)
        {
            return new SphericalPosition(new Spherical3D(coordinates), OccupationIndex, Status);
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
