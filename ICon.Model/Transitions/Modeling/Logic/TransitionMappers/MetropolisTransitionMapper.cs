﻿using System;
using System.Collections.Generic;
using System.Linq;

using ICon.Mathematics.Coordinates;
using ICon.Mathematics.Permutation;
using ICon.Mathematics.ValueTypes;
using ICon.Framework.Collections;

namespace ICon.Model.Transitions
{
    /// <summary>
    /// Mapper for metropolis transitions that searches and creates all possible 4D encoded exchanges for reference transitions
    /// </summary>
    public class MetropolisTransitionMapper
    {
        /// <summary>
        /// Creates the sequence of metropolis transitions mappings that result from a refernce transition and the 4D encoded unit cell positions for each symmetry index
        /// </summary>
        /// <param name="transition"></param>
        /// <param name="encodedPositions"></param>
        /// <returns></returns>
        public IEnumerable<MetropolisMapping> MapTransition(IMetropolisTransition transition, IList<SetList<CrystalVector4D>> encodedPositions)
        {
            if (!MappingIsPossible(transition, encodedPositions))
            {
                throw new InvalidOperationException("The passed combination of transition and encoded position lists does not yield any valid mapping");
            }
            return MakeMappings(transition, encodedPositions);
        }

        /// <summary>
        /// Creates the sequence of metropolis mappings for a set of metropolis transitions with the provided 4D encoded position lists
        /// </summary>
        /// <param name="transitions"></param>
        /// <param name="encodedPositions"></param>
        /// <returns></returns>
        public IEnumerable<IEnumerable<MetropolisMapping>> MapTransitions(IMetropolisTransition[] transitions, IList<SetList<CrystalVector4D>> encodedPositions)
        {
            return transitions.Select(transition => MapTransition(transition, encodedPositions));
        }

        /// <summary>
        /// Creates all transition mappings through a mapping permutation provider
        /// </summary>
        /// <param name="transition"></param>
        /// <param name="encodedPositions"></param>
        /// <returns></returns>
        protected IEnumerable<MetropolisMapping> MakeMappings(IMetropolisTransition transition, IList<SetList<CrystalVector4D>> encodedPositions)
        {
            return GetMappingPermuter(transition, encodedPositions).Select(permutation => new MetropolisMapping(transition, permutation[0], permutation[1]));
        }

        /// <summary>
        /// Creates a permutation provider for the possible combinations of position indices
        /// </summary>
        /// <param name="transition"></param>
        /// <param name="encodedPositions"></param>
        /// <returns></returns>
        protected IPermutationProvider<int> GetMappingPermuter(IMetropolisTransition transition, IList<SetList<CrystalVector4D>> encodedPositions)
        {
            var first = encodedPositions[transition.FirstUnitCellPosition.Index].Select(position => position.P).ToList();
            var second = encodedPositions[transition.SecondUnitCellPosition.Index].Select(position => position.P).ToList();
            return new SlotMachinePermuter<int>(first, second);
        }

        /// <summary>
        /// Checks if a reference transition and the encoded position list result in at least 1 valid mapping
        /// </summary>
        /// <param name="transition"></param>
        /// <param name="encodedPositions"></param>
        /// <returns></returns>
        protected bool MappingIsPossible(IMetropolisTransition transition, IList<SetList<CrystalVector4D>> encodedPositions)
        {
            if (transition == null)
            {
                throw new ArgumentNullException(nameof(transition));
            }
            if (encodedPositions == null)
            {
                throw new ArgumentNullException(nameof(encodedPositions));
            }
            if (encodedPositions.Count <= Math.Max(transition.FirstUnitCellPosition.Index, transition.SecondUnitCellPosition.Index))
            {
                return false;
            }
            if (encodedPositions[transition.FirstUnitCellPosition.Index].Count == 0 || encodedPositions[transition.SecondUnitCellPosition.Index].Count == 0)
            {
                return false;
            }
            return true;
        }
    }
}
