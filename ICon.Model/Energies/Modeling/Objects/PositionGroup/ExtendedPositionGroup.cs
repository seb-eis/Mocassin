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
        public IList<IUnitCellPosition> GroupSequenceUcps { get; set; }

        /// <summary>
        /// The list of unique pointgroup operations
        /// </summary>
        public IList<ISymmetryOperation> UniquePointOperations { get; set; }

        /// <summary>
        /// The two dimensional list interface of all known group sequences
        /// </summary>
        public IList<IList<Fractional3D>> GroupSequences { get; set; }

        /// <summary>
        /// The two dimensional list interface of all knwon unique group sequences
        /// </summary>
        public IList<IList<Fractional3D>> UniqueGroupSequences { get; set; }

        /// <summary>
        /// List of all unique occupation states without the center position
        /// </summary>
        public IList<OccupationState> UniqueOccupationStates { get; set; }
    }
}
