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

        /// <summary>
        /// Marked factory method to provide a custom model object add handler to the automated handling system of the transition input system
        /// </summary>
        /// <returns></returns>
        [HandlerFactoryMethod(DataOperationType.NewObject)]
        protected object CreateObjectAddedHandler()
        {
            return new TransitionObjectAddedHandler(ProjectServices);
        }

        /// <summary>
        /// Marked factory method to provide a custom model object change handler to the automated handling system of the transition input system
        /// </summary>
        /// <returns></returns>
        [HandlerFactoryMethod(DataOperationType.ObjectChange)]
        protected object CreateObjectChangedHandler()
        {
            return new TransitionObjectChangedHandler(ProjectServices);
        }

        /// <summary>
        /// Marked factory method to provide a custom model object removal handler to the automated handling system of the transition input system
        /// </summary>
        /// <returns></returns>
        [HandlerFactoryMethod(DataOperationType.ObjectRemoval)]
        protected object CreateObjectRemovedHandler()
        {
            return new TransitionObjectRemovedHandler(ProjectServices);
        }
    }
}
