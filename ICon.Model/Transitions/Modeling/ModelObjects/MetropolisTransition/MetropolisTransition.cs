using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Mocassin.Model.Basic;
using Mocassin.Model.Structures;

namespace Mocassin.Model.Transitions
{
    /// <inheritdoc cref="IMetropolisTransition" />
    [Serializable]
    [DataContract(Name = "MetropolisTransition")]
    public class MetropolisTransition : ModelObject, IMetropolisTransition
    {
        /// <inheritdoc />
        [DataMember]
        [IndexResolved]
        public IUnitCellPosition FirstUnitCellPosition { get; set; }

        /// <inheritdoc />
        [DataMember]
        [IndexResolved]
        public IUnitCellPosition SecondUnitCellPosition { get; set; }

        /// <inheritdoc />
        [DataMember]
        [IndexResolved]
        public IAbstractTransition AbstractTransition { get; set; }

        /// <summary>
        ///     The list of affiliated metropolis transition rules (Auto-managed by model system)
        /// </summary>
        [DataMember]
        [IndexResolved]
        public List<MetropolisRule> TransitionRules { get; set; }

        /// <inheritdoc />
        public IEnumerable<IMetropolisRule> GetTransitionRules()
        {
            return (TransitionRules ?? new List<MetropolisRule>()).AsEnumerable();
        }

        /// <inheritdoc />
        public IEnumerable<IMetropolisRule> GetExtendedTransitionRules()
        {
            foreach (var transitionRule in GetTransitionRules())
            {
                yield return transitionRule;

                foreach (var dependentRule in transitionRule.GetDependentRules())
                    yield return dependentRule;
            }
        }


        /// <inheritdoc />
        public bool Equals(IMetropolisTransition other)
        {
            if (other != null 
                && FirstUnitCellPosition.Index == other.FirstUnitCellPosition.Index 
                && SecondUnitCellPosition.Index == other.SecondUnitCellPosition.Index)
                return AbstractTransition == other.AbstractTransition;

            return other != null 
                   && FirstUnitCellPosition.Index == other.SecondUnitCellPosition.Index 
                   && SecondUnitCellPosition.Index == other.FirstUnitCellPosition.Index 
                   && AbstractTransition == other.AbstractTransition;
        }


        /// <inheritdoc />
        public override string GetObjectName()
        {
            return "Metropolis Transition";
        }

        /// <inheritdoc />
        public override ModelObject PopulateFrom(IModelObject obj)
        {
            if (!(CastIfNotDeprecated<IMetropolisTransition>(obj) is IMetropolisTransition transition)) 
                return null;

            FirstUnitCellPosition = transition.FirstUnitCellPosition;
            SecondUnitCellPosition = transition.SecondUnitCellPosition;
            AbstractTransition = transition.AbstractTransition;
            return this;

        }

        /// <inheritdoc />
        public bool MappingsContainInversion()
        {
            return FirstUnitCellPosition == SecondUnitCellPosition;
        }
    }
}