using System;
using System.Collections.Generic;
using System.Text;
using Mocassin.Framework.Operations;
using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Transitions.ConflictHandling
{
    /// <summary>
    /// Internal conflict handler for the addition of model objects to the transition managment system
    /// </summary>
    public class TransitionObjectAddedHandler : DataConflictHandler<TransitionModelData, ModelObject>
    {
        /// <summary>
        /// Create new transition object added handler using the provided project services
        /// </summary>
        /// <param name="modelProject"></param>
        public TransitionObjectAddedHandler(IModelProject modelProject) : base(modelProject)
        {
        }
        
        /// <summary>
        /// Handles the internal changes within the transition model data required due to a new kinetic transition added by the controller
        /// </summary>
        /// <param name="transition"></param>
        /// <param name="dataAccess"></param>
        /// <returns></returns>
        [ConflictHandlingMethod]
        protected IConflictReport HandleNewKineticTransition(KineticTransition transition, IDataAccessor<TransitionModelData> dataAccess)
        {
            return new KineticTransitionAddedHandler(dataAccess, ModelProject).HandleConflicts(transition);
        }

        /// <summary>
        /// Handles the internal changes within the transition model data required due to a new metropolis transition added by the controller
        /// </summary>
        /// <param name="transition"></param>
        /// <param name="dataAccess"></param>
        /// <returns></returns>
        [ConflictHandlingMethod]
        protected IConflictReport HandleNewMetropolisTransition(MetropolisTransition transition, IDataAccessor<TransitionModelData> dataAccess)
        {
            return new MetropolisTransitionAddedHandler(dataAccess, ModelProject).HandleConflicts(transition);
        }
    }
}
