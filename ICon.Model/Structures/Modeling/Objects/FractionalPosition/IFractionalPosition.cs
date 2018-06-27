using System;
using System.Collections.Generic;
using System.Text;

using ICon.Mathematics.ValueTypes;

namespace ICon.Model.Structures
{
    /// <summary>
    /// Represents a non unique occupied position which carries fractional coordinate information and an occupation index
    /// </summary>
    public interface IFractionalPosition : IFractional3D
    {
        /// <summary>
        /// The particle set index that describes possible occupations
        /// </summary>
        int OccupationIndex { get; }

        /// <summary>
        /// The status of the position
        /// </summary>
        PositionStatus Status { get; }
    }
}
