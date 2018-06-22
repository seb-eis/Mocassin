using System;
using System.Collections.Generic;
using System.Linq;

using ICon.Symmetry.Analysis;
using ICon.Symmetry.SpaceGroups;
using ICon.Mathematics.Permutation;
using ICon.Mathematics.ValueTypes;
using ICon.Mathematics.Coordinates;
using ICon.Model.Particles;
using ICon.Model.Structures;

namespace ICon.Model.Energies
{
    /// <summary>
    /// Grouping analyzer that handles calculations and premutation generation for grouping definitions
    /// </summary>
    public class GeometryGroupAnalyzer
    {
        /// <summary>
        /// The unit cell provider that supplies the information about the supercell the group is loacted in
        /// </summary>
        public IUnitCellProvider<IUnitCellPosition> UnitCellProvider { get; }

        /// <summary>
        /// The space group service that supplies the symmetry information about the unit cell
        /// </summary>
        public ISpaceGroupService SpaceGroupService { get; }

        /// <summary>
        /// Generates new grouping analyzer that uses the passed unit cell provider and space group service for geoemtric analysis
        /// </summary>
        /// <param name="unitCellProvider"></param>
        /// <param name="spaceGroupService"></param>
        public GeometryGroupAnalyzer(IUnitCellProvider<IUnitCellPosition> unitCellProvider, ISpaceGroupService spaceGroupService)
        {
            UnitCellProvider = unitCellProvider ?? throw new ArgumentNullException(nameof(unitCellProvider));
            SpaceGroupService = spaceGroupService ?? throw new ArgumentNullException(nameof(spaceGroupService));
        }

        /// <summary>
        /// Creates a permutation provider for the positions of a interaction group
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        public IPermutationProvider<IParticle> GetGroupStatePermuter(IGroupInteraction group)
        {
            return GetGroupStatePermuter(group.GetBaseGeometry());
        }

        /// <summary>
        /// Creates a permutation provider for a sequence of fractional position vectors (Without center position) describing the group geometry
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        public IPermutationProvider<IParticle> GetGroupStatePermuter(IEnumerable<Fractional3D> group)
        {
            return new SlotMachinePermuter<IParticle>(group.Select(vector => UnitCellProvider.GetEntryValueAt(vector).OccupationSet.GetParticles()));
        }
        
        /// <summary>
        /// Creates the extended position group that results from the provided group interaction
        /// </summary>
        /// <param name="groupInteraction"></param>
        /// <returns></returns>
        public ExtendedPositionGroup CreateExtendedPositionGroup(IGroupInteraction groupInteraction)
        {
            var extendedGroup = new ExtendedPositionGroup() { CenterPosition = groupInteraction.UnitCellPosition };

            extendedGroup.GroupSequenceUcps = GetGroupInteractionUcps(groupInteraction).ToList();

            return extendedGroup;
        }

        /// <summary>
        /// Get the unit cell position sequence described by a group interaction. Will contain null values if any of the fractional vector does not point to
        /// a valid position
        /// </summary>
        /// <param name="groupInteraction"></param>
        /// <returns></returns>
        protected IEnumerable<IUnitCellPosition> GetGroupInteractionUcps(IGroupInteraction groupInteraction)
        {
            return groupInteraction.GetBaseGeometry().Select(vector => UnitCellProvider.GetEntryValueAt(vector));
        }

        /// <summary>
        /// Extends the group vector sequence based upon the point symmetry operations of the center position. Filters out any duplicates
        /// </summary>
        /// <param name="groupInteraction"></param>
        /// <returns></returns>
        protected List<IList<Fractional3D>> GetExtendedGroupSequences(IGroupInteraction groupInteraction)
        {
            return null;
        }
    }
}
