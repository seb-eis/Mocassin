using System;
using Mocassin.Mathematics.ValueTypes;

namespace Mocassin.Model.Structures
{
    /// <inheritdoc cref="Mocassin.Model.Structures.ICartesianPosition"/>
    public readonly struct CartesianPosition : ICartesianPosition, ICartesianMassPoint3D
    {
        /// <inheritdoc />
        public Cartesian3D Vector { get; }

        /// <inheritdoc />
        public int OccupationIndex { get; }

        /// <inheritdoc />
        public PositionStatus Status { get; }

        /// <inheritdoc />
        public Coordinates3D Coordinates => Vector.Coordinates;

        /// <inheritdoc />
        public double Mass => OccupationIndex;

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
        public CartesianPosition(in Cartesian3D vector, int particleSetIndex, PositionStatus status)
            : this()
        {
            Status = status;
            Vector = vector;
            OccupationIndex = particleSetIndex;
        }
    }
}