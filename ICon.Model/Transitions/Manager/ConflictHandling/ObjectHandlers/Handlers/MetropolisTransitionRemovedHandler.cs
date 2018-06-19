using System;
using System.Collections.Generic;
using System.Text;
using ICon.Framework.Operations;
using ICon.Model.Basic;
using ICon.Model.ProjectServices;

namespace ICon.Model.Transitions.ConflictHandling
{
    /// <summary>
    /// Object handler that handles internal data changes of the transition manager system required after a kinetic transition is removed/deprecated
    /// </summary>
    public class MetropolisTransitionRemovedHandler : MetropolisTransitionHandlerBase
    {
        /// <summary>
        /// Create new metropolis transition removed handler with the provided data accessor and project services
        /// </summary>
        /// <param name="dataAccess"></param>
        /// <param name="projectServices"></param>
        public MetropolisTransitionRemovedHandler(IDataAccessor<TransitionModelData> dataAccess, IProjectServices projectServices) 
            : base(dataAccess, projectServices)
        {
        }

        /// <summary>
        /// Determine the conflicts induced by the removde/deprecated kinetic transition and update the transition model data structure with the changes
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override ConflictReport HandleConflicts(MetropolisTransition obj)
        {
            return new ConflictReport();
        }
    }
}
