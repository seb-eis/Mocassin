using System;
using System.Collections.Generic;
using System.Text;

using ICon.Symmetry.SpaceGroups;
using ICon.Mathematics.ValueTypes;
using ICon.Model.Structures;
using ICon.Model.Particles;

namespace ICon.Model.Energies
{
    /// <summary>
    /// Represents an extended position group that carries all information to fully describe the properties of a group interaction
    /// </summary>
    public class ExtendedPositionGroup
    {
        /// <summary>
        /// The center position
        /// </summary>
        public IUnitCellPosition CenterPosition { get; set; }

        /// <summary>
        /// The unit cell positions of the group sequences
        /// </summary>
        public IList<IUnitCellPosition> GroupUnitCellPositions { get; set; }

        /// <summary>
        /// The point operation group describing the symmetry operation information of the group
        /// </summary>
        public IPointOperationGroup PointOperationGroup { get; set; }

        /// <summary>
        /// List of all unique occupation states without the center position
        /// </summary>
        public IList<OccupationState> UniqueOccupationStates { get; set; }

        /// <summary>
        /// The full unique energy dictionary for each unqiue occupation state arround each unique center particle
        /// </summary>
        public Dictionary<IParticle, Dictionary<OccupationState, double>> FullEnergyDictionary { get; set; }
    }
}
