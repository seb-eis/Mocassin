using System;
using System.Collections.Generic;
using System.Text;

using ICon.Framework.Operations;
using ICon.Model.Basic;
using ICon.Model.ProjectServices;

namespace ICon.Model.Transitions.ConflictHandling
{
    /// <summary>
    /// Internal conflict handler for the change of model objects witin the transition managment system
    /// </summary>
    public class TransitionObjectChangedHandler : DataConflictHandler<TransitionModelData, ModelObject>
    {
        /// <summary>
        /// Create new transition object change handler using the provided project services
        /// </summary>
        /// <param name="projectServices"></param>
        public TransitionObjectChangedHandler(IProjectServices projectServices) : base(projectServices)
        {
        }

        /// <summary>
        /// Handles the internal changes within the transition model data required due to a changed kinetic transition 
        /// </summary>
        /// <param name="transition"></param>
        /// <param name="dataAccess"></param>
        /// <returns></returns>
        [ConflictHandlingMethod]
        protected IConflictReport HandleChangedKineticTransition(KineticTransition transition, IDataAccessor<TransitionModelData> dataAccess)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Handles the internal changes within the transition model data required due to a changed metropolis transition
        /// </summary>
        /// <param name="transition"></param>
        /// <param name="dataAccess"></param>
        /// <returns></returns>
        [ConflictHandlingMethod]
        protected IConflictReport HandleChangedMetropolisTransition(MetropolisTransition transition, IDataAccessor<TransitionModelData> dataAccess)
        {
            throw new NotImplementedException();
        }
    }
}
