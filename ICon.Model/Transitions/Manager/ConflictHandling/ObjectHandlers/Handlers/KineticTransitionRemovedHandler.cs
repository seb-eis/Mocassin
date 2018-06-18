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
    public class KineticTransitionRemovedHandler : ObjectConflictHandler<KineticTransition, TransitionModelData>
    {

        /// <summary>
        /// Create new kinetic transition removed handler with the provided data access and project services
        /// </summary>
        /// <param name="dataAccess"></param>
        /// <param name="projectServices"></param>
        public KineticTransitionRemovedHandler(IDataAccessor<TransitionModelData> dataAccess, IProjectServices projectServices)
            : base(dataAccess, projectServices)
        {
        }

        /// <summary>
        /// Determine the conflicts induced by the removde/deprecated kinetic transition and update the transition model data structure with the changes
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override ConflictReport HandleConflicts(KineticTransition obj)
        {
            throw new NotImplementedException();
        }
    }
}
