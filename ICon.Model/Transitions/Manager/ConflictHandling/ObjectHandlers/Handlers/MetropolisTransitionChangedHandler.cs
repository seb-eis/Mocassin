using System;
using System.Collections.Generic;
using System.Text;
using ICon.Framework.Operations;
using ICon.Model.Basic;
using ICon.Model.ProjectServices;

namespace ICon.Model.Transitions.ConflictHandling
{
    /// <summary>
    /// Object handler that handles internal data changes of the transition manager system required after a metropolis transition data change
    /// </summary>
    public class MetropolisTransitionChangedHandler : ObjectConflictHandler<MetropolisTransition, TransitionModelData>
    {
        /// <summary>
        /// Create new metropolis transition changed handler with the provided data accessor and project services
        /// </summary>
        /// <param name="dataAccess"></param>
        /// <param name="projectServices"></param>
        public MetropolisTransitionChangedHandler(IDataAccessor<TransitionModelData> dataAccess, IProjectServices projectServices)
            : base(dataAccess, projectServices)
        {
        }

        /// <summary>
        /// Determine the conflicts induced by the changed metropolis transition and update the transition model data structure with the changes
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override ConflictReport HandleConflicts(MetropolisTransition obj)
        {
            throw new NotImplementedException();
        }
    }
}
