﻿using System;
using System.Collections.Generic;
using System.Linq;
using Mocassin.Framework.Collections;
using Mocassin.Framework.Extensions;
using Mocassin.Mathematics.Permutation;
using Mocassin.Mathematics.ValueTypes;
using Mocassin.Model.Particles;
using Mocassin.Model.Structures;
using Mocassin.Symmetry.Analysis;
using Mocassin.Symmetry.SpaceGroups;

namespace Mocassin.Model.Energies
{
    /// <summary>
    ///     Grouping analyzer that handles calculations and permutation generation for grouping definitions
    /// </summary>
    public class GeometryGroupAnalyzer
    {
        /// <summary>
        ///     The unit cell provider that supplies the information about the super-cell the group is located in
        /// </summary>
        public IUnitCellProvider<IUnitCellPosition> UnitCellProvider { get; }

        /// <summary>
        ///     The space group service that supplies the symmetry information about the unit cell
        /// </summary>
        public ISpaceGroupService SpaceGroupService { get; }

        /// <summary>
        ///     Generates new grouping analyzer that uses the passed unit cell provider and space group service for geoemtric
        ///     analysis
        /// </summary>
        /// <param name="unitCellProvider"></param>
        /// <param name="spaceGroupService"></param>
        public GeometryGroupAnalyzer(IUnitCellProvider<IUnitCellPosition> unitCellProvider, ISpaceGroupService spaceGroupService)
        {
            UnitCellProvider = unitCellProvider ?? throw new ArgumentNullException(nameof(unitCellProvider));
            SpaceGroupService = spaceGroupService ?? throw new ArgumentNullException(nameof(spaceGroupService));
        }

        /// <summary>
        ///     Creates the extended position group that results from the provided group interaction. If the interaction is
        ///     deprecated an incomplete object is returned
        /// </summary>
        /// <param name="groupInteraction"></param>
        /// <returns></returns>
        public ExtendedPositionGroup CreateExtendedPositionGroup(IGroupInteraction groupInteraction)
        {
            var extGroup = new ExtendedPositionGroup
                {GroupInteraction = groupInteraction, CenterPosition = groupInteraction.CenterUnitCellPosition};

            if (groupInteraction.IsDeprecated)
                return extGroup;

            extGroup.SurroundingUnitCellPositions = GetGroupUnitCellPositions(groupInteraction).ToList();
            extGroup.PointOperationGroup =
                SpaceGroupService.GetPointOperationGroup(extGroup.CenterPosition.Vector, groupInteraction.GetBaseGeometry());

            extGroup.UniqueOccupationStates =
                GetUniqueGroupOccupationStates(extGroup.PointOperationGroup, GetGroupStatePermutationSource(groupInteraction)).ToList();

            extGroup.UniqueEnergyDictionary = MakeFullEnergyDictionary(extGroup.UniqueOccupationStates, extGroup.CenterPosition);    
            return extGroup;
        }

        /// <summary>
        ///     Creates the extended position groups for all passed group interactions. Each calculation is done in a single task
        /// </summary>
        /// <param name="groupInteractions"></param>
        /// <returns></returns>
        public IEnumerable<ExtendedPositionGroup> CreateExtendedPositionGroups(IEnumerable<IGroupInteraction> groupInteractions)
        {
            Func<ExtendedPositionGroup> MakeCall(IGroupInteraction groupInteraction)
            {
                return () => CreateExtendedPositionGroup(groupInteraction);
            }

            return MocassinTaskingExtensions.RunAndGetResults(groupInteractions.Select(MakeCall));
        }

        /// <summary>
        ///     Get the enumerable sequence off all possible symmetrical pair interactions that can be found within an interaction
        ///     group
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        public IEnumerable<SymmetricParticlePair> GetAllGroupPairs(IGroupInteraction group)
        {
            var particles =
                new HashSet<IParticle>(GetGroupUnitCellPositions(group).SelectMany(value => value.OccupationSet.GetParticles()));
            var permutationSource =
                new PermutationSlotMachine<IParticle>(group.CenterUnitCellPosition.OccupationSet.GetParticles(), particles);
            return new HashSet<SymmetricParticlePair>(permutationSource.Select(perm => new SymmetricParticlePair
                {Particle0 = perm[0], Particle1 = perm[1]})).AsEnumerable();
        }

        /// <summary>
        ///     Get the unit cell position sequence described by a group interaction. Will contain null values if any of the fractional vector does not point to a valid position
        /// </summary>
        /// <param name="groupInteraction"></param>
        /// <returns></returns>
        public IEnumerable<IUnitCellPosition> GetGroupUnitCellPositions(IGroupInteraction groupInteraction)
        {
            return groupInteraction.GetBaseGeometry().Select(vector => UnitCellProvider.GetEntryValueAt(vector));
        }

        /// <summary>
        ///     Determines if the passed group interaction contains any pair interactions that are filtered by the passed set of
        ///     interaction filters
        /// </summary>
        /// <param name="groupInteraction"></param>
        /// <param name="filters"></param>
        /// <returns></returns>
        public bool GroupContainsFilteredPairs(IGroupInteraction groupInteraction, IEnumerable<IInteractionFilter> filters)
        {         
            var partnerPositions = GetGroupUnitCellPositions(groupInteraction).ToList();
            var distances = groupInteraction.GetBaseGeometry()
                .Select(vector => vector - groupInteraction.CenterUnitCellPosition.Vector)
                .Select(x => UnitCellProvider.VectorEncoder.Transformer.ToCartesian(x).GetLength())
                .ToList();

            return filters
                .Any(x => distances.Where((t, i) => x.IsApplicable(t, groupInteraction.CenterUnitCellPosition, partnerPositions[i])).Any());
        }

        /// <summary>
        ///     Generates all unique occupation states for the surrounding atoms of a group without permuting the center position
        /// </summary>
        /// <param name="operationGroup"></param>
        /// <param name="permProvider"></param>
        /// <returns></returns>
        protected IEnumerable<OccupationState> GetUniqueGroupOccupationStates(IPointOperationGroup operationGroup,
            IPermutationSource<IParticle> permProvider)
        {
            var comparer = new EqualityCompareAdapter<IParticle>((a, b) => a.Index == b.Index);
            var rawPermutations = operationGroup.GetUniquePermutations(permProvider, comparer, a => 1 << a.Index);
            return rawPermutations.Select(value => new OccupationState {Particles = value.ToList()});
        }

        /// <summary>
        ///     Takes a center position and a set of possible unique occupation states for the surroundings an creates the full
        ///     energy dictionary for
        ///     all existing occupation combinations of an interaction group
        /// </summary>
        /// <param name="occStates"></param>
        /// <param name="centerPosition"></param>
        /// <returns></returns>
        protected Dictionary<IParticle, Dictionary<OccupationState, double>> MakeFullEnergyDictionary(
            IEnumerable<OccupationState> occStates, IUnitCellPosition centerPosition)
        {
            if (!(occStates is ICollection<OccupationState> occStateCollection))
                occStateCollection = occStates.ToList();

            var result = new Dictionary<IParticle, Dictionary<OccupationState, double>>(centerPosition.OccupationSet.ParticleCount);
            foreach (var centerParticle in centerPosition.OccupationSet.GetParticles())
            {
                var innerDictionary = new Dictionary<OccupationState, double>(10);
                foreach (var state in occStateCollection)
                    innerDictionary.Add(state, 0.0);

                result.Add(centerParticle, innerDictionary);
            }

            return result;
        }

        /// <summary>
        ///     Creates a permutation provider for the positions of a interaction group
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        public IPermutationSource<IParticle> GetGroupStatePermutationSource(IGroupInteraction group)
        {
            return GetGroupStatePermutationSource(group.GetBaseGeometry());
        }

        /// <summary>
        ///     Creates a permutation provider for a sequence of fractional position vectors (Without center position) describing
        ///     the group geometry
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        public IPermutationSource<IParticle> GetGroupStatePermutationSource(IEnumerable<Fractional3D> group)
        {
            return new PermutationSlotMachine<IParticle>(group.Select(vector =>
                UnitCellProvider.GetEntryValueAt(vector).OccupationSet.GetParticles()));
        }
    }
}