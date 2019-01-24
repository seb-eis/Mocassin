using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Mocassin.Model.Basic;

namespace Mocassin.Model.Transitions
{
    /// <inheritdoc cref="IKineticRule" />
    [DataContract]
    public class KineticRule : TransitionRule, IKineticRule
    {
        /// <inheritdoc />
        [DataMember]
        [UseTrackedReferences]
        public IKineticTransition Transition { get; set; }

        /// <inheritdoc />
        [DataMember]
        public double AttemptFrequency { get; set; }

        /// <summary>
        ///     The list of dependent rules that are a direct result of this rule
        /// </summary>
        [DataMember]
        public List<KineticRule> DependentRules { get; set; }

        /// <inheritdoc />
        public KineticRule()
        {
            DependentRules = new List<KineticRule>();
        }

        /// <inheritdoc />
        public override string GetObjectName()
        {
            return "Kinetic Rule";
        }

        /// <inheritdoc />
        public override ModelObject PopulateFrom(IModelObject obj)
        {
            if (!(CastIfNotDeprecated<IKineticRule>(obj) is IKineticRule rule))
                return null;

            base.PopulateFrom(obj);
            Transition = rule.Transition;
            AttemptFrequency = rule.AttemptFrequency;

            return null;
        }

        /// <inheritdoc />
        public IEnumerable<IKineticRule> GetDependentRules()
        {
            return DependentRules.AsEnumerable();
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