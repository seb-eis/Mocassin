using Mocassin.Mathematics.ValueTypes;

namespace Mocassin.Model.Structures
{
    /// <inheritdoc cref="Mocassin.Model.Structures.ISphericalPosition" />
    public readonly struct SphericalPosition : ISphericalPosition, ISphericalMassPoint3D
    {
        /// <inheritdoc />
        public Spherical3D Vector { get; }

        /// <inheritdoc />
        public int OccupationIndex { get; }

        /// <inheritdoc />
        public PositionStability Stability { get; }

        /// <inheritdoc />
        public Coordinates3D Coordinates => Vector.Coordinates;

        /// <inheritdoc />
        public double Mass => OccupationIndex;

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
        /// <param name="stability"></param>
        public SphericalPosition(Spherical3D vector, int occupationIndex, PositionStability stability)
            : this()
        {
            Stability = stability;
            Vector = vector;
            OccupationIndex = occupationIndex;
        }
    }
}