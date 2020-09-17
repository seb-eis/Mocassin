using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Simulations.ConflictHandling
{
    /// <summary>
    ///     Conflict handler provider for the simulation manager related model objects and parameters
    /// </summary>
    public class SimulationDataConflictHandlerProvider : DataConflictHandlerProvider<SimulationModelData>
    {
        /// <inheritdoc />
        public SimulationDataConflictHandlerProvider(IModelProject modelProject)
            : base(modelProject)
        {
        }

        /// <summary>
        ///     Creation method for the simulation added conflict handler
        /// </summary>
        /// <returns></returns>
        [HandlerFactoryMethod(DataOperationType.NewObject)]
        protected object CreateObjectHandler() => new SimulationObjectChangedHandler(ModelProject);
    }
}