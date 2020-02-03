using System.Collections.Generic;
using Mocassin.Model.Particles;
using Mocassin.Model.Structures;
using Mocassin.Symmetry.SpaceGroups;

namespace Mocassin.Model.Energies
{
    /// <summary>
    ///     Represents a position group info that carries extended data and information about a defined group interaction
    /// </summary>
    public interface IPositionGroupInfo
    {
        /// <summary>
        ///     The group interaction the position group info belongs to
        /// </summary>
        IGroupInteraction GroupInteraction { get; }

        /// <summary>
        ///     The center position of the group
        /// </summary>
        ICellReferencePosition CenterPosition { get; }

        /// <summary>
        ///     The unit cell position entries of all surrounding positions
        /// </summary>
        IReadOnlyList<ICellReferencePosition> SurroundingPositions { get; }

        /// <summary>
        ///     The point operation group that describes the full geometry and symmetry information of the position group
        /// </summary>
        IPointOperationGroup PointOperationGroup { get; }

        /// <summary>
        ///     The list of unique occupation states described by the group
        /// </summary>
        IReadOnlyList<IOccupationState> UniqueOccupationStates { get; }

        /// <summary>
        ///     Get the full energy collection that assign each unique combination of center and surrounding occupation an energy
        ///     value
        /// </summary>
        IReadOnlyList<GroupEnergyEntry> GetEnergyEntryList();

        /// <summary>
        ///     Synchronizes the energy dictionary with the currently set values on the interaction
        /// </summary>
        void SynchronizeEnergyDictionary();
    }
}