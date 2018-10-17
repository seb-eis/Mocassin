using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Mocassin.Model.Particles;
using Mocassin.Model.Basic;

namespace Mocassin.Model.Transitions
{
    /// <summary>
    /// Metropolis rule implementation that extends the basic transition rule by the metropolis properties and functionalities
    /// </summary>
    [DataContract]
    public class MetropolisRule : TransitionRule, IMetropolisRule
    {
        /// <summary>
        /// The metropolis transition parent instance
        /// </summary>
        [DataMember]
        [IndexResolved]
        public IMetropolisTransition Transition { get; set; }

        /// <summary>
        /// The list of dependent rules that are a direct result of this rule
        /// </summary>
        [DataMember]
        public List<MetropolisRule> DependentRules { get; set; }


        /// <summary>
        /// Creates new kinetic rule with empty dependent rule list
        /// </summary>
        public MetropolisRule()
        {
            DependentRules = new List<MetropolisRule>();
        }

        /// <summary>
        /// Get all dependent rules that are a direct result of this one
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IMetropolisRule> GetDependentRules()
        {
            return DependentRules.AsEnumerable();
        }

        /// <summary>
        /// Get model object name
        /// </summary>
        /// <returns></returns>
        public override string GetObjectName()
        {
            return "'Metropolis Rule'";
        }

        /// <summary>
        /// Populates from the an model object interface and returns this object (Returns null if the population failed)
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override ModelObject PopulateFrom(IModelObject obj)
        {
            if (CastIfNotDeprecated<IMetropolisRule>(obj) is var rule)
            {
                base.PopulateFrom(obj);
                Transition = rule.Transition;
                return this;
            }
            return null;
        }

        /// <summary>
        /// Adds a dependent kinetic transition rule. Throw if the passed rule is not a kinetic rule
        /// </summary>
        /// <param name="rule"></param>
        public override void AddDependentRule(TransitionRule rule)
        {
            if (rule is MetropolisRule metropolisRule)
            {
                DependentRules.Add(metropolisRule);
                return;
            }
            throw new ArgumentException("Passed rule does not inherit from a metropolis rule", nameof(rule));
        }
    }
}
