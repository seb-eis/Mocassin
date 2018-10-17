using System;
using System.Collections.Generic;
using System.Linq;
using Mocassin.Framework.Extensions;
using Mocassin.Framework.Operations;
using Mocassin.Model.Basic;
using Mocassin.Model.Particles;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Transitions.ConflictHandling
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
        /// <param name="modelProject"></param>
        protected MetropolisTransitionHandlerBase(IDataAccessor<TransitionModelData> dataAccess, IModelProject modelProject)
            : base(dataAccess, modelProject)
        {
        }

        /// <summary>
        /// Create all transition rules for the provided metropolis transition
        /// </summary>
        /// <param name="transition"></param>
        /// <returns></returns>
        protected IEnumerable<MetropolisRule> CreateTransitionRules(MetropolisTransition transition)
        {
            var particles = ModelProject.GetManager<IParticleManager>().QueryPort.Query(port => port.GetParticles());
            var creator = new QuickRuleGenerator<MetropolisRule>(particles);
            return creator.MakeUniqueRules(transition.AbstractTransition.AsSingleton(), true)
                .SingleOrDefault()
                .Action(rule => rule.Transition = transition);
        }

        /// <summary>
        /// Adds new kinetic rules to the model data at old deprecated places or the end of the list. Indexes objects accordingly
        /// </summary>
        /// <param name="rules"></param>
        protected void IndexAndAddToModelData(IEnumerable<MetropolisRule> rules)
        {
            new IndexedDataManager<MetropolisRule>().IndexAndAddUseDeprecated(DataAccess.Query(data => data.MetropolisRules), rules);
        }
    }
}
