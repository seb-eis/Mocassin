using System;
using System.Collections.Generic;
using System.Text;
using ICon.Framework.Operations;
using ICon.Model.Basic;
using ICon.Model.ProjectServices;

namespace ICon.Model.Transitions.ConflictHandling
{
    /// <summary>
    /// Object handler that handles internal data changes of the transition manager system required after a new metropolis transition input
    /// </summary>
    public class MetropolisTransitionAddedHandler : MetropolisTransitionHandlerBase
    {

        /// <summary>
        /// Create new metropolis transition added handler with the provided data accessor and project services
        /// </summary>
        /// <param name="dataAccess"></param>
        /// <param name="projectServices"></param>
        public MetropolisTransitionAddedHandler(IDataAccessor<TransitionModelData> dataAccess, IProjectServices projectServices)
            : base(dataAccess, projectServices)
        {
        }

        /// <summary>
        /// Determine the conflicts induced by the new metropolis transition and update the transition model data structure with the changes
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="dataAccess"></param>
        /// <param name="projectServices"></param>
        /// <returns></returns>
        public override ConflictReport HandleConflicts(MetropolisTransition obj)
        {
            return new ConflictReport();
        }
    }
}
