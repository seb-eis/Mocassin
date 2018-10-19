using Mocassin.Mathematics.ValueTypes;

namespace Mocassin.Model.Structures
{
    /// <summary>
    ///     Represents a non unique occupied position which carries cartesian coordinate information and an occupation index
    /// </summary>
    public interface ICartesianPosition : ICartesian3D
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