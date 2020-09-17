using System;
using Mocassin.Framework.Operations;
using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Transitions.ConflictHandling
{
    /// <summary>
    ///     Internal conflict handler for the removal of model objects within the transition managment system
    /// </summary>
    public class TransitionObjectRemovedHandler : DataConflictHandler<TransitionModelData, ModelObject>
    {
        /// <inheritdoc />
        public TransitionObjectRemovedHandler(IModelProject modelProject)
            : base(modelProject)
        {
        }

        /// <summary>
        ///     Handles the internal changes within the transition model data required due to removed kinetic transition
        /// </summary>
        /// <param name="transition"></param>
        /// <param name="dataAccess"></param>
        /// <returns></returns>
        [ConflictHandlingMethod]
        protected IConflictReport HandleRemovedKineticTransition(KineticTransition transition,
            IDataAccessor<TransitionModelData> dataAccess) =>
            throw new NotImplementedException();

        /// <summary>
        ///     Handles the internal changes within the transition model data required due to a removed metropolis transition
        /// </summary>
        /// <param name="transition"></param>
        /// <param name="dataAccess"></param>
        /// <returns></returns>
        [ConflictHandlingMethod]
        protected IConflictReport HandleRemovedMetropolisTransition(MetropolisTransition transition,
            IDataAccessor<TransitionModelData> dataAccess) =>
            throw new NotImplementedException();
    }
}