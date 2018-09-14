using System;
using System.Collections.Generic;
using System.Linq;
using ICon.Model.Particles;
using ICon.Model.Structures;
using ICon.Symmetry.SpaceGroups;

namespace ICon.Model.Energies
{
    /// <summary>
    /// Adapter class to provide read only restricted access to an extended position group
    /// </summary>
    public class PositionGroupInfo : IPositionGroupInfo
    {
        /// <summary>
        /// The wrapped extended position group
        /// </summary>
        protected ExtendedPositionGroup ExtendedPositionGroup { get; set; }

        /// <summary>
        /// The group interaction the position group info belongs to
        /// </summary>
        public IGroupInteraction GroupInteraction => ExtendedPositionGroup.GroupInteraction;

        /// <summary>
        /// The center position of the group
        /// </summary>
        public IUnitCellPosition CenterPosition => ExtendedPositionGroup.CenterPosition;

        /// <summary>
        /// Read only access to the surrounding unit cell position entries
        /// </summary>
        public IReadOnlyList<IUnitCellPosition> SurroundingPositions => ExtendedPositionGroup.SurroundingUnitCellPositions;

        /// <summary>
        /// The list of unique occupation states described by the group
        /// </summary>
        public IReadOnlyList<IOccupationState> UniqueOccupationStates => ExtendedPositionGroup.UniqueOccupationStates;

        /// <summary>
        /// The point operation group that fully describes geometry and symmetry information of the group
        /// </summary>
        public IPointOperationGroup PointOperationGroup => ExtendedPositionGroup.PointOperationGroup;

        /// <summary>
        /// Creates new position group info wrapper for the provided extended position group
        /// </summary>
        /// <param name="extendedPositionGroup"></param>
        public PositionGroupInfo(ExtendedPositionGroup extendedPositionGroup)
        {
            ExtendedPositionGroup = extendedPositionGroup ?? throw new ArgumentNullException(nameof(extendedPositionGroup));
        }

        /// <summary>
        /// Get a read only list of the current status of the energy group entries
        /// </summary>
        /// <returns></returns>
        public IReadOnlyList<GroupEnergyEntry> GetEnergyEntryList()
        {
            return ExtendedPositionGroup
                .UniqueEnergyDictionary
                .SelectMany(outer => outer.Value.Select(inner => new GroupEnergyEntry(outer.Key, inner.Key, inner.Value)))
                .ToList();
        }
    }
}
