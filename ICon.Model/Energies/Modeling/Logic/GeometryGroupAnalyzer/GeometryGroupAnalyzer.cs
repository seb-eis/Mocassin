using System;
using System.Collections.Generic;
using System.Linq;

using ICon.Symmetry.Analysis;
using ICon.Symmetry.SpaceGroups;
using ICon.Mathematics.Permutation;
using ICon.Mathematics.ValueTypes;
using ICon.Framework.Collections;
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
            var extGroup = new ExtendedPositionGroup() { CenterPosition = groupInteraction.CenterUnitCellPosition };

            extGroup.GroupUnitCellPositions = GetGroupUnitCellPositions(groupInteraction).ToList();
            extGroup.PointOperationGroup = SpaceGroupService.GetPointOperationGroup(extGroup.CenterPosition.Vector, groupInteraction.GetBaseGeometry());
            extGroup.UniqueOccupationStates = GetUniqueGroupOccupationStates(extGroup.PointOperationGroup, GetGroupStatePermuter(groupInteraction)).ToList();
            extGroup.FullEnergyDictionary = MakeUniqueEnergyDictionary(extGroup.UniqueOccupationStates, extGroup.CenterPosition);
            return extGroup;
        }
        /// <summary>
        /// Get the enumerable sequence off all possible symmetrical pair interactions that can be found within an interaction group
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        public IEnumerable<SymParticlePair> GetAllGroupPairs(IGroupInteraction group)
        {
            var particles = new HashSet<IParticle>(GetGroupUnitCellPositions(group).SelectMany(value => value.OccupationSet.GetParticles()));
            var permuter = new SlotMachinePermuter<IParticle>(group.CenterUnitCellPosition.OccupationSet.GetParticles(), particles);
            return new HashSet<SymParticlePair>(permuter.Select(perm => new SymParticlePair() { Particle0 = perm[0], Particle1 = perm[1] })).AsEnumerable();
        }


        /// <summary>
        /// Get the unit cell position sequence described by a group interaction. Will contain null values if any of the fractional vector does not point to
        /// a valid position
        /// </summary>
        /// <param name="groupInteraction"></param>
        /// <returns></returns>
        public IEnumerable<IUnitCellPosition> GetGroupUnitCellPositions(IGroupInteraction groupInteraction)
        {
            return groupInteraction.GetBaseGeometry().Select(vector => UnitCellProvider.GetEntryValueAt(vector));
        }

        /// <summary>
        /// Generates all unique occupation states for the surrounding atoms of a group without permuting the center position
        /// </summary>
        /// <param name="operationGroup"></param>
        /// <param name="permProvider"></param>
        /// <returns></returns>
        protected IEnumerable<OccupationState> GetUniqueGroupOccupationStates(IPointOperationGroup operationGroup, IPermutationProvider<IParticle> permProvider)
        {
            var comparer = new EqualityCompareAdapter<IParticle>((a, b) => a.Index == b.Index, null);
            var rawPermutations = operationGroup.GetGeometryUniquePermutations(permProvider, comparer, a => 1 << a.Index);
            return rawPermutations.Select(value => new OccupationState() { Particles = value.ToList() });
        }

        /// <summary>
        /// Takes a center position and a set of possible unique occupation states for the surroundings an creates the full energy dictinary for
        /// all existing occupation combinations of an interaction group
        /// </summary>
        /// <param name="occStates"></param>
        /// <param name="centerPosition"></param>
        /// <returns></returns>
        protected Dictionary<IParticle, Dictionary<OccupationState, double>> MakeUniqueEnergyDictionary(IEnumerable<OccupationState> occStates, IUnitCellPosition centerPosition)
        {
            var result = new Dictionary<IParticle, Dictionary<OccupationState, double>>(centerPosition.OccupationSet.ParticleCount);
            foreach (var centerParticle in centerPosition.OccupationSet.GetParticles())
            {
                var innerDictionary = new Dictionary<OccupationState, double>(10);
                foreach (var state in occStates)
                {
                    innerDictionary.Add(state, 0.0);
                }
                result.Add(centerParticle, innerDictionary);
            }
            return result;
        }
    }
}
