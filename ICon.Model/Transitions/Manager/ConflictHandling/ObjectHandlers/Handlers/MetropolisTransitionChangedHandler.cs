using System;
using System.Collections.Generic;
using System.Text;
using Mocassin.Framework.Operations;
using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Transitions.ConflictHandling
{
    /// <summary>
    /// Object handler that handles internal data changes of the transition manager system required after a metropolis transition data change
    /// </summary>
    public class MetropolisTransitionChangedHandler : MetropolisTransitionHandlerBase
    {
        /// <summary>
        /// Create new metropolis transition changed handler with the provided data accessor and project services
        /// </summary>
        /// <param name="dataAccess"></param>
        /// <param name="modelProject"></param>
        public MetropolisTransitionChangedHandler(IDataAccessor<TransitionModelData> dataAccess, IModelProject modelProject)
            : base(dataAccess, modelProject)
        {
        }

        /// <summary>
        /// Determine the conflicts induced by the changed metropolis transition and update the transition model data structure with the changes
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override ConflictReport HandleConflicts(MetropolisTransition obj)
        {
            return new ConflictReport();
        }
    }
}
