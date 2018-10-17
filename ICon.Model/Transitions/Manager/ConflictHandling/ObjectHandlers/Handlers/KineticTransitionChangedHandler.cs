using System;
using System.Collections.Generic;
using System.Text;
using Mocassin.Framework.Operations;
using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Transitions.ConflictHandling
{
    /// <summary>
    /// Object handler that handles internal data changes of the transition manager system required after a kinetic transition data change
    /// </summary>
    public class KineticTransitionChangedHandler : KineticTransitionHandlerBase
    {
        /// <summary>
        /// Create new kinetic transition change handler with the provided data acessor and project services
        /// </summary>
        /// <param name="dataAccess"></param>
        /// <param name="modelProject"></param>
        public KineticTransitionChangedHandler(IDataAccessor<TransitionModelData> dataAccess, IModelProject modelProject)
            : base(dataAccess, modelProject)
        {
        }

        /// <summary>
        /// Determine the conflicts induced by the changed kinetic transition and update the transition model data structure with the changes
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="dataAccess"></param>
        /// <param name="projectServices"></param>
        /// <returns></returns>
        public override ConflictReport HandleConflicts(KineticTransition obj)
        {
            return new ConflictReport();
        }
    }
}
