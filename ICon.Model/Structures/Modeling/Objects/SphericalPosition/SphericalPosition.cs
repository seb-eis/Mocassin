using System;
using Mocassin.Mathematics.ValueTypes;

namespace Mocassin.Model.Structures
{
    /// <inheritdoc cref="Mocassin.Model.Structures.ISphericalPosition"/>
    public readonly struct SphericalPosition : ISphericalPosition, ISphericalMassPoint3D<SphericalPosition>
    {
        /// <inheritdoc />
        public Spherical3D Vector { get; }

        /// <inheritdoc />
        public int OccupationIndex { get; }

        /// <inheritdoc />
        public PositionStatus Status { get; }

        /// <inheritdoc />
        public Coordinates<double, double, double> Coordinates => Vector.Coordinates;

        /// <summary>
        ///     The coordinate value in A direction
        /// </summary>
        public double A => Vector.Radius;

        /// <summary>
        ///     The coordinate value in B direction
        /// </summary>
        public double B => Vector.Theta;

        /// <summary>
        ///     The coordinate value in C direction
        /// </summary>
        public double C => Vector.Phi;

        /// <inheritdoc />
        public double Radius => Vector.Radius;

        /// <inheritdoc />
        public double Theta => Vector.Theta;

        /// <inheritdoc />
        public double Phi => Vector.Phi;

        /// <summary>
        ///     Creates new cell position from cartesian 3D vector and particle set index
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="occupationIndex"></param>
        /// <param name="status"></param>
        public SphericalPosition(Spherical3D vector, int occupationIndex, PositionStatus status)
            : this()
        {
            Status = status;
            Vector = vector;
            OccupationIndex = occupationIndex;
        }

        /// <inheritdoc />
        public SphericalPosition CreateNew(double radius, double theta, double phi, double mass)
        {
            return new SphericalPosition(new Spherical3D(radius, theta, phi), Convert.ToInt32(mass), Status);
        }

        /// <inheritdoc />
        public SphericalPosition CreateNew(in Coordinates<double, double, double> coordinates, double mass)
        {
            return new SphericalPosition(new Spherical3D(coordinates), Convert.ToInt32(mass), Status);
        }

        /// <inheritdoc />
        public SphericalPosition CreateNew(in Spherical3D vector, double mass)
        {
            return new SphericalPosition(vector, Convert.ToInt32(mass), Status);
        }

        /// <inheritdoc />
        public SphericalPosition CreateNew(double radius, double theta, double phi)
        {
            return new SphericalPosition(new Spherical3D(radius, theta, phi), OccupationIndex, Status);
        }

        /// <inheritdoc />
        public SphericalPosition CreateNew(Coordinates<double, double, double> coordinates)
        {
            return new SphericalPosition(new Spherical3D(coordinates), OccupationIndex, Status);
        }

        /// <inheritdoc />
        public double GetMass()
        {
            return OccupationIndex;
        }
    }
}