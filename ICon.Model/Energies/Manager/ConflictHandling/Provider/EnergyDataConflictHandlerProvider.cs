using ICon.Model.Basic;
using ICon.Model.ProjectServices;

namespace ICon.Model.Energies.ConflictHandling
{
    /// <summary>
    ///     Resolver provider for all energy conflict resolvers that handle internal data conflicts of the particle manager.
    /// </summary>
    public class EnergyDataConflictHandlerProvider : DataConflictHandlerProvider<EnergyModelData>
    {
        /// <inheritdoc />
        public EnergyDataConflictHandlerProvider(IProjectServices projectServices)
            : base(projectServices)
        {
        }

        /// <summary>
        ///     Get the custom handler for energy parameter change data conflict resolving (Overwrites default handler in pipeline)
        /// </summary>
        /// <returns></returns>
        [HandlerFactoryMethod(DataOperationType.ParameterChange)]
        protected object CreateParameterHandler()
        {
            return new EnergyParameterChangeHandler(ProjectServices);
        }

        /// <summary>
        ///     Get the custom handler for energy object change data conflict resolving (Overwrites default handler in pipeline)
        /// </summary>
        /// <returns></returns>
        [HandlerFactoryMethod(DataOperationType.ObjectChange)]
        protected object CreateObjectChangeHandler()
        {
            return new EnergyObjectChangeHandler(ProjectServices);
        }

        /// <summary>
        ///     Get the custom handler for energy object change data conflict resolving (Overwrites default handler in pipeline)
        /// </summary>
        /// <returns></returns>
        [HandlerFactoryMethod(DataOperationType.NewObject)]
        protected object CreateInputChangeHandler()
        {
            return new EnergyObjectAddedHandler(ProjectServices);
        }
    }
}