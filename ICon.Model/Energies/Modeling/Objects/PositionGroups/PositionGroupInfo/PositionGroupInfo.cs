using System;
using System.Collections.Generic;
using System.Linq;
using Mocassin.Model.Particles;
using Mocassin.Model.Structures;
using Mocassin.Symmetry.SpaceGroups;

namespace Mocassin.Model.Energies
{
    /// <inheritdoc />
    public class PositionGroupInfo : IPositionGroupInfo
    {
        /// <summary>
        ///     The wrapped extended position group
        /// </summary>
        protected ExtendedPositionGroup ExtendedPositionGroup { get; set; }

        /// <inheritdoc />
        public IGroupInteraction GroupInteraction => ExtendedPositionGroup.GroupInteraction;

        /// <inheritdoc />
        public IUnitCellPosition CenterPosition => ExtendedPositionGroup.CenterPosition;

        /// <inheritdoc />
        public IReadOnlyList<IUnitCellPosition> SurroundingPositions => ExtendedPositionGroup.SurroundingUnitCellPositions;

        /// <inheritdoc />
        public IReadOnlyList<IOccupationState> UniqueOccupationStates => ExtendedPositionGroup.UniqueOccupationStates;

        /// <inheritdoc />
        public IPointOperationGroup PointOperationGroup => ExtendedPositionGroup.PointOperationGroup;

        /// <summary>
        ///     Creates new position group info wrapper for the provided extended position group
        /// </summary>
        /// <param name="extendedPositionGroup"></param>
        public PositionGroupInfo(ExtendedPositionGroup extendedPositionGroup)
        {
            ExtendedPositionGroup = extendedPositionGroup ?? throw new ArgumentNullException(nameof(extendedPositionGroup));
        }

        /// <inheritdoc />
        public IReadOnlyList<GroupEnergyEntry> GetEnergyEntryList()
        {
            return ExtendedPositionGroup
                .UniqueEnergyDictionary
                .SelectMany(outer => outer.Value.Select(inner => new GroupEnergyEntry(outer.Key, inner.Key, inner.Value)))
                .ToList();
        }
    }
}