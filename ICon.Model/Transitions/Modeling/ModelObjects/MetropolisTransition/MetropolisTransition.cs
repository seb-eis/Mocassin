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
        [UseTrackedReferences]
        public ICellReferencePosition FirstCellReferencePosition { get; set; }

        /// <inheritdoc />
        [DataMember]
        [UseTrackedReferences]
        public ICellReferencePosition SecondCellReferencePosition { get; set; }

        /// <inheritdoc />
        [DataMember]
        [UseTrackedReferences]
        public IAbstractTransition AbstractTransition { get; set; }

        /// <summary>
        ///     The list of affiliated metropolis transition rules (Auto-managed by model system)
        /// </summary>
        [DataMember]
        [UseTrackedReferences]
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
                && FirstCellReferencePosition.Index == other.FirstCellReferencePosition.Index 
                && SecondCellReferencePosition.Index == other.SecondCellReferencePosition.Index)
                return AbstractTransition == other.AbstractTransition;

            return other != null 
                   && FirstCellReferencePosition.Index == other.SecondCellReferencePosition.Index 
                   && SecondCellReferencePosition.Index == other.FirstCellReferencePosition.Index 
                   && AbstractTransition == other.AbstractTransition;
        }


		/// <inheritdoc />
		public override string ObjectName => "Metropolis Transition";

		/// <inheritdoc />
		public override ModelObject PopulateFrom(IModelObject obj)
        {
            if (!(CastIfNotDeprecated<IMetropolisTransition>(obj) is IMetropolisTransition transition)) 
                return null;

            FirstCellReferencePosition = transition.FirstCellReferencePosition;
            SecondCellReferencePosition = transition.SecondCellReferencePosition;
            AbstractTransition = transition.AbstractTransition;
            return this;

        }

        /// <inheritdoc />
        public bool MappingsContainInversion()
        {
            return FirstCellReferencePosition == SecondCellReferencePosition;
        }
    }
}