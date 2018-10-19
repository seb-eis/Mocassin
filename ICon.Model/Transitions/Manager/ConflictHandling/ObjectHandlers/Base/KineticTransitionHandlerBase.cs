using System.Collections.Generic;
using System.Linq;
using Mocassin.Framework.Extensions;
using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;
using Mocassin.Model.Particles;

namespace Mocassin.Model.Transitions.ConflictHandling
{
    /// <summary>
    ///     Base class for shared functionality of all conflict handlers that deal with the kinetic transition objects
    /// </summary>
    public abstract class KineticTransitionHandlerBase : ObjectConflictHandler<KineticTransition, TransitionModelData>
    {
        /// <inheritdoc />
        protected KineticTransitionHandlerBase(IDataAccessor<TransitionModelData> dataAccessor, IModelProject modelProject)
            : base(dataAccessor, modelProject)
        {
        }

        /// <summary>
        ///     Create all transition rules for the kinetic transition
        /// </summary>
        /// <param name="transition"></param>
        /// <returns></returns>
        protected IEnumerable<KineticRule> CreateTransitionRules(KineticTransition transition)
        {
            var particles = ModelProject.GetManager<IParticleManager>().QueryPort.Query(port => port.GetParticles());
            var creator = new QuickRuleGenerator<KineticRule>(particles);
            return creator.MakeUniqueRules(transition.AbstractTransition.AsSingleton(), true)
                .SingleOrDefault()
                .Action(rule => rule.Transition = transition);
        }

        /// <summary>
        ///     Adds new kinetic rules to the model data at old deprecated places or the end of the list. Indexes objects
        ///     accordingly
        /// </summary>
        /// <param name="rules"></param>
        protected void IndexAndAddToModelData(IEnumerable<KineticRule> rules)
        {
            new IndexedDataManager<KineticRule>().IndexAndAddUseDeprecated(DataAccess.Query(data => data.KineticRules), rules);
        }
    }
}