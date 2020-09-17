using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Energies.ConflictHandling
{
    /// <summary>
    ///     Resolver provider for all energy conflict resolvers that handle internal data conflicts of the particle manager.
    /// </summary>
    public class EnergyDataConflictHandlerProvider : DataConflictHandlerProvider<EnergyModelData>
    {
        /// <inheritdoc />
        public EnergyDataConflictHandlerProvider(IModelProject modelProject)
            : base(modelProject)
        {
        }

        /// <summary>
        ///     Get the custom handler for energy parameter change data conflict resolving (Overwrites default handler in pipeline)
        /// </summary>
        /// <returns></returns>
        [HandlerFactoryMethod(DataOperationType.ParameterChange)]
        protected object CreateParameterHandler() => new EnergyParameterChangeHandler(ModelProject);

        /// <summary>
        ///     Get the custom handler for energy object change data conflict resolving (Overwrites default handler in pipeline)
        /// </summary>
        /// <returns></returns>
        [HandlerFactoryMethod(DataOperationType.ObjectChange)]
        protected object CreateObjectChangeHandler() => new EnergyObjectChangeHandler(ModelProject);

        /// <summary>
        ///     Get the custom handler for energy object change data conflict resolving (Overwrites default handler in pipeline)
        /// </summary>
        /// <returns></returns>
        [HandlerFactoryMethod(DataOperationType.NewObject)]
        protected object CreateInputChangeHandler() => new EnergyObjectAddedHandler(ModelProject);
    }
}