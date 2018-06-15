using System;
using System.Collections.Generic;
using System.Text;

using ICon.Framework.Operations;
using ICon.Model.Basic;
using ICon.Model.ProjectServices;

namespace ICon.Model.Transitions.ConflictHandling
{
    /// <summary>
    /// Resolver provider for all transition conflict resolvers that handle internal data conflicts of the particle manager
    /// </summary>
    public class TransitionDataConflictHandlerProvider : DataConflictHandlerProvider<TransitionModelData>
    {
        /// <summary>
        /// Creates new transition data conflict resolver provider with access to the provided project services
        /// </summary>
        /// <param name="projectServices"></param>
        public TransitionDataConflictHandlerProvider(IProjectServices projectServices) : base(projectServices)
        {

        }
    }
}
