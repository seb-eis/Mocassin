using System;
using System.Collections.Generic;
using System.Linq;
using ICon.Framework.Extensions;
using ICon.Framework.Operations;
using ICon.Model.Particles;
using ICon.Model.Basic;
using ICon.Model.ProjectServices;

namespace ICon.Model.Transitions.ConflictHandling
{
    /// <summary>
    /// Base class for shared functionality of all conflict handlers that deal with the kinetic transition objects
    /// </summary>
    public abstract class KineticTransitionHandlerBase : ObjectConflictHandler<KineticTransition, TransitionModelData>
    {
        /// <summary>
        /// Create new kinetic transition handler base with the provided project services and data accessor
        /// </summary>
        /// <param name="dataAccess"></param>
        /// <param name="projectServices"></param>
        protected KineticTransitionHandlerBase(IDataAccessor<TransitionModelData> dataAccess, IProjectServices projectServices)
            : base(dataAccess, projectServices)
        {
        }

        /// <summary>
        /// Create all transition rules for the kinetic transition
        /// </summary>
        /// <param name="transition"></param>
        /// <returns></returns>
        protected IEnumerable<KineticRule> CreateTransitionRules(KineticTransition transition)
        {
            var particles = ProjectServices.GetManager<IParticleManager>().QueryPort.Query(port => port.GetParticles());
            var creator = new QuickRuleGenerator<KineticRule>(particles);
            return creator.MakeUniqueRules(transition.AbstractTransition.AsSingleton(), true)
                .SingleOrDefault()
                .Change(rule => rule.Transition = transition);
        }
    }
}
