using System;
using System.Collections.Generic;
using System.Text;
using ICon.Framework.Operations;
using ICon.Model.Basic;
using ICon.Model.ProjectServices;

namespace ICon.Model.Transitions.ConflictHandling
{
    /// <summary>
    /// Object handler that handles internal data changes of the transition manager system required after a new kinetic transition input
    /// </summary>
    public class KineticTransitionAddedHandler : ObjectConflictHandler<KineticTransition, TransitionModelData>
    {
        /// <summary>
        /// Create new kinetic transition added handler with the provided project services and data accessor
        /// </summary>
        /// <param name="dataAccess"></param>
        /// <param name="projectServices"></param>
        public KineticTransitionAddedHandler(IDataAccessor<TransitionModelData> dataAccess, IProjectServices projectServices)
            : base(dataAccess, projectServices)
        {
        }

        /// <summary>
        /// Determine the conflicts induced by the new kinetic transition and update the transition model data structure with the changes
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="dataAccess"></param>
        /// <param name="projectServices"></param>
        /// <returns></returns>
        public override ConflictReport HandleConflicts(KineticTransition obj)
        {
            throw new NotImplementedException();
        }
    }
}
