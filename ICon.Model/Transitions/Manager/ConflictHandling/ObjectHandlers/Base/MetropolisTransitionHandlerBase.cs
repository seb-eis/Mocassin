using System;
using System.Collections.Generic;
using System.Linq;
using ICon.Framework.Extensions;
using ICon.Model.Particles;
using ICon.Model.Basic;
using ICon.Model.ProjectServices;

namespace ICon.Model.Transitions.ConflictHandling
{
    /// <summary>
    /// Base class for shared functionality of all conflict handlers that deal with the metropolis transition objects
    /// </summary>
    public abstract class MetropolisTransitionHandlerBase : ObjectConflictHandler<MetropolisTransition, TransitionModelData>
    {
        /// <summary>
        /// Create new metropolis transition handler base with the provided project services and data accessor
        /// </summary>
        /// <param name="dataAccess"></param>
        /// <param name="projectServices"></param>
        protected MetropolisTransitionHandlerBase(IDataAccessor<TransitionModelData> dataAccess, IProjectServices projectServices)
            : base(dataAccess, projectServices)
        {
        }

        /// <summary>
        /// Create all transition rules for the provided metropolis transition
        /// </summary>
        /// <param name="transition"></param>
        /// <returns></returns>
        protected IEnumerable<MetropolisRule> CreateTransitionRules(MetropolisTransition transition)
        {
            var particles = ProjectServices.GetManager<IParticleManager>().QueryPort.Query(port => port.GetParticles());
            var creator = new QuickRuleGenerator<MetropolisRule>(particles);
            return creator.MakeUniqueRules(transition.AbstractTransition.AsSingleton()).SingleOrDefault().Change(rule => rule.Transition = transition);
        }
    }
}
