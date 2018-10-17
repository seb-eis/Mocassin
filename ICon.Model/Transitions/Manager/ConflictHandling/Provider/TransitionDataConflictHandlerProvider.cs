using System;
using System.Collections.Generic;
using System.Text;

using Mocassin.Framework.Operations;
using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Transitions.ConflictHandling
{
    /// <summary>
    /// Resolver provider for all transition conflict resolvers that handle internal data conflicts of the particle manager
    /// </summary>
    public class TransitionDataConflictHandlerProvider : DataConflictHandlerProvider<TransitionModelData>
    {
        /// <summary>
        /// Creates new transition data conflict resolver provider with access to the provided project services
        /// </summary>
        /// <param name="modelProject"></param>
        public TransitionDataConflictHandlerProvider(IModelProject modelProject) : base(modelProject)
        {

        }

        /// <summary>
        /// Marked factory method to provide a custom model object add handler to the automated handling system of the transition input system
        /// </summary>
        /// <returns></returns>
        [HandlerFactoryMethod(DataOperationType.NewObject)]
        protected object CreateObjectAddedHandler()
        {
            return new TransitionObjectAddedHandler(ModelProject);
        }

        /// <summary>
        /// Marked factory method to provide a custom model object change handler to the automated handling system of the transition input system
        /// </summary>
        /// <returns></returns>
        [HandlerFactoryMethod(DataOperationType.ObjectChange)]
        protected object CreateObjectChangedHandler()
        {
            return new TransitionObjectChangedHandler(ModelProject);
        }

        /// <summary>
        /// Marked factory method to provide a custom model object removal handler to the automated handling system of the transition input system
        /// </summary>
        /// <returns></returns>
        [HandlerFactoryMethod(DataOperationType.ObjectRemoval)]
        protected object CreateObjectRemovedHandler()
        {
            return new TransitionObjectRemovedHandler(ModelProject);
        }
    }
}
