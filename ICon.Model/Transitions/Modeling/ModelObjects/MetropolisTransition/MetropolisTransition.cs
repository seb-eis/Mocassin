using System.Collections.Generic;
using System.Linq;
using Mocassin.Model.Basic;
using Mocassin.Model.Structures;

namespace Mocassin.Model.Transitions
{
    /// <inheritdoc cref="IMetropolisTransition" />
    public class MetropolisTransition : ModelObject, IMetropolisTransition
    {
        /// <inheritdoc />
        [UseTrackedData]
        public ICellSite FirstCellSite { get; set; }

        /// <inheritdoc />
        [UseTrackedData]
        public ICellSite SecondCellSite { get; set; }

        /// <inheritdoc />
        [UseTrackedData]
        public IAbstractTransition AbstractTransition { get; set; }

        /// <summary>
        ///     The list of affiliated metropolis transition rules (Auto-managed by model system)
        /// </summary>
        [UseTrackedData]
        public List<MetropolisRule> TransitionRules { get; set; }


        /// <inheritdoc />
        public override string ObjectName => "Metropolis Transition";

        /// <inheritdoc />
        public IEnumerable<IMetropolisRule> GetTransitionRules() => (TransitionRules ?? new List<MetropolisRule>()).AsEnumerable();

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
                && FirstCellSite.Index == other.FirstCellSite.Index
                && SecondCellSite.Index == other.SecondCellSite.Index)
                return AbstractTransition == other.AbstractTransition;

            return other != null
                   && FirstCellSite.Index == other.SecondCellSite.Index
                   && SecondCellSite.Index == other.FirstCellSite.Index
                   && AbstractTransition == other.AbstractTransition;
        }

        /// <inheritdoc />
        public bool MappingsContainInversion() => FirstCellSite == SecondCellSite;

        /// <inheritdoc />
        public override ModelObject PopulateFrom(IModelObject obj)
        {
            if (!(CastIfNotDeprecated<IMetropolisTransition>(obj) is { } transition))
                return null;

            FirstCellSite = transition.FirstCellSite;
            SecondCellSite = transition.SecondCellSite;
            AbstractTransition = transition.AbstractTransition;
            return this;
        }
    }
}