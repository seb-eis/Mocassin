using System;
using Mocassin.Mathematics.ValueTypes;

namespace Mocassin.Model.Structures
{
    /// <inheritdoc cref="Mocassin.Model.Structures.ICartesianPosition"/>
    public readonly struct CartesianPosition : ICartesianPosition, ICartesianMassPoint3D<CartesianPosition>
    {
        /// <inheritdoc />
        public Cartesian3D Vector { get; }

        /// <inheritdoc />
        public int OccupationIndex { get; }

        /// <inheritdoc />
        public PositionStatus Status { get; }

        /// <inheritdoc />
        public Coordinates<double, double, double> Coordinates => Vector.Coordinates;

        /// <summary>
        ///     The coordinate value in A direction
        /// </summary>
        public double A => Vector.X;

        /// <summary>
        ///     The coordinate value in B direction
        /// </summary>
        public double B => Vector.Y;

        /// <summary>
        ///     The coordinate value in C direction
        /// </summary>
        public double C => Vector.Z;

        /// <inheritdoc />
        public double X => Vector.X;

        /// <inheritdoc />
        public double Y => Vector.Y;

        /// <inheritdoc />
        public double Z => Vector.Z;

        /// <summary>
        ///     Creates new cell position from cartesian 3D vector and particle set index
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="particleSetIndex"></param>
        /// <param name="status"></param>
        public CartesianPosition(Cartesian3D vector, int particleSetIndex, PositionStatus status)
            : this()
        {
            Status = status;
            Vector = vector;
            OccupationIndex = particleSetIndex;
        }

        /// <inheritdoc />
        public CartesianPosition CreateNew(double x, double y, double z, double mass)
        {
            return new CartesianPosition(new Cartesian3D(x, y, z), Convert.ToInt32(mass), Status);
        }

        /// <inheritdoc />
        public CartesianPosition CreateNew(in Coordinates<double, double, double> coordinates, double mass)
        {
            return new CartesianPosition(new Cartesian3D(coordinates), Convert.ToInt32(mass), Status);
        }

        /// <inheritdoc />
        public CartesianPosition CreateNew(in Cartesian3D vector, double mass)
        {
            return new CartesianPosition(vector, Convert.ToInt32(mass), Status);
        }

        /// <inheritdoc />
        public CartesianPosition CreateNew(double x, double y, double z)
        {
            return new CartesianPosition(new Cartesian3D(x, y, z), OccupationIndex, Status);
        }

        /// <inheritdoc />
        public CartesianPosition CreateNew(Coordinates<double, double, double> coordinates)
        {
            return new CartesianPosition(new Cartesian3D(coordinates), OccupationIndex, Status);
        }

        /// <inheritdoc />
        public double GetMass()
        {
            return OccupationIndex;
        }
    }
}