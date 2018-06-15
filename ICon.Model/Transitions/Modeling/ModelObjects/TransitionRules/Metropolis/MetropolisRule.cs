using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using ICon.Model.Basic;

namespace ICon.Model.Transitions
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
        [IndexResolvable]
        public IMetropolisTransition Transition { get; set; }

        /// <summary>
        /// Get model object name
        /// </summary>
        /// <returns></returns>
        public override string GetModelObjectName()
        {
            return "'Metropolis Rule'";
        }

        /// <summary>
        /// Populates from the an model object interface and returns this object (Returns null if the population failed)
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override ModelObject PopulateObject(IModelObject obj)
        {
            if (CastWithDepricatedCheck<IMetropolisRule>(obj) is var rule)
            {
                base.PopulateObject(obj);
                Transition = rule.Transition;
                return this;
            }
            return null;
        }
    }
}
