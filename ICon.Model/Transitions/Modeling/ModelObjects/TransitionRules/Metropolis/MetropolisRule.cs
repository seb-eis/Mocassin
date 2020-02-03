using System;
using System.Collections.Generic;
using System.Linq;
using Mocassin.Model.Basic;

namespace Mocassin.Model.Transitions
{
    /// <inheritdoc cref="Mocassin.Model.Transitions.IMetropolisRule" />
    public class MetropolisRule : TransitionRule, IMetropolisRule
    {
        /// <inheritdoc />
        [UseTrackedData]
        public IMetropolisTransition Transition { get; set; }

        /// <summary>
        ///     The list of dependent rules that are a direct result of this rule
        /// </summary>
        public List<MetropolisRule> DependentRules { get; set; }


        /// <summary>
        ///     Creates new kinetic rule with empty dependent rule list
        /// </summary>
        public MetropolisRule()
        {
            DependentRules = new List<MetropolisRule>();
        }

        /// <inheritdoc />
        public IEnumerable<IMetropolisRule> GetDependentRules()
        {
            return DependentRules.AsEnumerable();
        }

        /// <inheritdoc />
        public override string ObjectName => "Metropolis Rule";

        /// <inheritdoc />
        public override ModelObject PopulateFrom(IModelObject obj)
        {
            if (!(CastIfNotDeprecated<IMetropolisRule>(obj) is { } rule))
                return null;

            base.PopulateFrom(obj);
            Transition = rule.Transition;
            return this;
        }

        /// <inheritdoc />
        public override void AddDependentRule(TransitionRule rule)
        {
            if (!(rule is MetropolisRule metropolisRule))
                throw new ArgumentException("Passed rule does not inherit from a metropolis rule", nameof(rule));

            DependentRules.Add(metropolisRule);
        }
    }
}