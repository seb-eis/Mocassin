using System;
using System.Collections.Generic;
using System.Linq;
using Mocassin.Model.Basic;

namespace Mocassin.Model.Transitions
{
    /// <inheritdoc cref="IKineticRule" />
    public class KineticRule : TransitionRule, IKineticRule
    {
        /// <inheritdoc />
        [UseTrackedData]
        public IKineticTransition Transition { get; set; }

        /// <inheritdoc />
        public double AttemptFrequency { get; set; }

        /// <summary>
        ///     The list of dependent rules that are a direct result of this rule
        /// </summary>
        public List<KineticRule> DependentRules { get; set; }

        /// <inheritdoc />
        public override string ObjectName => "Kinetic Rule";

        /// <inheritdoc />
        public KineticRule()
        {
            DependentRules = new List<KineticRule>();
        }

        /// <inheritdoc />
        public IEnumerable<IKineticRule> GetDependentRules() => DependentRules.AsEnumerable();

        /// <inheritdoc />
        public override ModelObject PopulateFrom(IModelObject obj)
        {
            if (!(CastIfNotDeprecated<IKineticRule>(obj) is { } rule))
                return null;

            base.PopulateFrom(obj);
            Transition = rule.Transition;
            AttemptFrequency = rule.AttemptFrequency;

            return null;
        }

        /// <inheritdoc />
        public override void AddDependentRule(TransitionRule rule)
        {
            if (!(rule is KineticRule kineticRule))
                throw new ArgumentException("Passed rule does not inherit from a kinetic rule", nameof(rule));

            kineticRule.Transition = Transition;
            DependentRules.Add(kineticRule);
        }
    }
}