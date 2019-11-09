using Mocassin.Mathematics.ValueTypes;

namespace Mocassin.Model.Structures
{
    /// <inheritdoc cref="Mocassin.Model.Structures.IFractionalPosition" />
    public readonly struct FractionalPosition : IFractionalPosition, IFractionalMassPoint3D
    {
        /// <inheritdoc />
        public Fractional3D Vector { get; }

        /// <inheritdoc />
        public int OccupationIndex { get; }

        /// <inheritdoc />
        public PositionStatus Status { get; }

        /// <inheritdoc />
        public Coordinates3D Coordinates => Vector.Coordinates;

        /// <inheritdoc />
        public double A => Vector.A;

        /// <inheritdoc />
        public double B => Vector.B;

        /// <inheritdoc />
        public double C => Vector.C;

        /// <inheritdoc />
        public double Mass => OccupationIndex;

        /// <summary>
        ///     Creates new cell position from fractional 3D vector and particle set index
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="occupationIndex"></param>
        /// <param name="status"></param>
        public FractionalPosition(Fractional3D vector, int occupationIndex, PositionStatus status)
            : this()
        {
            Status = status;
            Vector = vector;
            OccupationIndex = occupationIndex;
        }
    }
}