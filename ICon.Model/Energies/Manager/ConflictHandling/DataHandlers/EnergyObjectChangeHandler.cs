using Mocassin.Framework.Operations;
using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Energies.ConflictHandling
{
    /// <summary>
    ///     Energy object change handler that handles data changes and conflict resolving for changed energy model objects
    /// </summary>
    public class EnergyObjectChangeHandler : DataConflictHandler<EnergyModelData, ModelObject>
    {
        /// <inheritdoc />
        public EnergyObjectChangeHandler(IModelProject modelProject)
            : base(modelProject)
        {
        }

        /// <summary>
        ///     Resolves conflicts and data changes if an unstable environment changes. The passed object represents the changed
        ///     model object
        /// </summary>
        /// <param name="envInfo"></param>
        /// <param name="dataAccess"></param>
        /// <returns></returns>
        [ConflictHandlingMethod]
        protected IConflictReport HandleObjectChange(UnstableEnvironment envInfo, IDataAccessor<EnergyModelData> dataAccess)
        {
            return new UnstableEnvironmentChangeHandler(dataAccess, ModelProject).HandleConflicts(envInfo);
        }

        /// <summary>
        ///     Resolves conflicts and data changes if a group interaction is added. The passed object has to be the changed model
        ///     object.
        /// </summary>
        /// <param name="groupInteraction"></param>
        /// <param name="dataAccess"></param>
        /// <returns></returns>
        [ConflictHandlingMethod]
        protected IConflictReport HandleObjectChange(GroupInteraction groupInteraction, IDataAccessor<EnergyModelData> dataAccess)
        {
            return new GroupInteractionChangeHandler(dataAccess, ModelProject).HandleConflicts(groupInteraction);
        }
    }
}