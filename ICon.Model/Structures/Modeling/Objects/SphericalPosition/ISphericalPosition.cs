using Mocassin.Mathematics.ValueTypes;

namespace Mocassin.Model.Structures
{
    /// <summary>
    ///     Represents a non unique occupied position which carries spherical coordinate information and an occupation index
    /// </summary>
    public interface ISphericalPosition : ISpherical3D
    {
        /// <summary>
        ///     Index of the particle set that describes possible occupations
        /// </summary>
        int OccupationIndex { get; }

        /// <summary>
        ///     The status of the position (Stable, unstable)
        /// </summary>
        PositionStatus Status { get; }
    }
}