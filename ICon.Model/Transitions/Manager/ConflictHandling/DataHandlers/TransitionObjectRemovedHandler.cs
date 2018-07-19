using System;
using System.Collections.Generic;
using System.Text;

using ICon.Framework.Operations;
using ICon.Model.Basic;
using ICon.Model.ProjectServices;

namespace ICon.Model.Transitions.ConflictHandling
{
    /// <summary>
    /// Internal conflict handler for the removal of model objects within the transition managment system
    /// </summary>
    public class TransitionObjectRemovedHandler : DataConflictHandler<TransitionModelData, ModelObject>
    {
        /// <summary>
        /// Create new transition object removed handler using the provided project services
        /// </summary>
        /// <param name="projectServices"></param>
        public TransitionObjectRemovedHandler(IProjectServices projectServices) : base(projectServices)
        {
        }

        /// <summary>
        /// Handles the internal changes within the transition model data required due to removed kinetic transition
        /// </summary>
        /// <param name="transition"></param>
        /// <param name="dataAccess"></param>
        /// <returns></returns>
        [ConflictHandlingMethod]
        protected IConflictReport HandleRemovedKineticTransition(KineticTransition transition, IDataAccessor<TransitionModelData> dataAccess)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Handles the internal changes within the transition model data required due to a removed metropolis transition
        /// </summary>
        /// <param name="transition"></param>
        /// <param name="dataAccess"></param>
        /// <returns></returns>
        [ConflictHandlingMethod]
        protected IConflictReport HandleRemovedMetropolisTransition(MetropolisTransition transition, IDataAccessor<TransitionModelData> dataAccess)
        {
            throw new NotImplementedException();
        }
    }
}
