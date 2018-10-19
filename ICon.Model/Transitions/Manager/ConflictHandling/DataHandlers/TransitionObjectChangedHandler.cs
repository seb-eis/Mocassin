using System;
using Mocassin.Framework.Operations;
using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Transitions.ConflictHandling
{
    /// <summary>
    ///     Internal conflict handler for the change of model objects witin the transition managment system
    /// </summary>
    public class TransitionObjectChangedHandler : DataConflictHandler<TransitionModelData, ModelObject>
    {
        /// <inheritdoc />
        public TransitionObjectChangedHandler(IModelProject modelProject)
            : base(modelProject)
        {
        }

        /// <summary>
        ///     Handles the internal changes within the transition model data required due to a changed kinetic transition
        /// </summary>
        /// <param name="transition"></param>
        /// <param name="dataAccess"></param>
        /// <returns></returns>
        [ConflictHandlingMethod]
        protected IConflictReport HandleChangedKineticTransition(KineticTransition transition,
            IDataAccessor<TransitionModelData> dataAccess)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Handles the internal changes within the transition model data required due to a changed metropolis transition
        /// </summary>
        /// <param name="transition"></param>
        /// <param name="dataAccess"></param>
        /// <returns></returns>
        [ConflictHandlingMethod]
        protected IConflictReport HandleChangedMetropolisTransition(MetropolisTransition transition,
            IDataAccessor<TransitionModelData> dataAccess)
        {
            throw new NotImplementedException();
        }
    }
}