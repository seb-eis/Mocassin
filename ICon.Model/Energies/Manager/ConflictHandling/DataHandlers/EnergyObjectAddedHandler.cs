using System;
using Mocassin.Framework.Operations;
using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Energies.ConflictHandling
{
    /// <summary>
    ///     Energy object added handler that handles data updates and conflict resolving due to newly added model objects
    /// </summary>
    public class EnergyObjectAddedHandler : DataConflictHandler<EnergyModelData, ModelObject>
    {
        /// <inheritdoc />
        public EnergyObjectAddedHandler(IModelProject modelProject)
            : base(modelProject)
        {
        }

        /// <summary>
        ///     Resolves conflicts and data changes if an unstable environment is added. The passed object has to be the changed
        ///     model object.
        /// </summary>
        /// <param name="envInfo"></param>
        /// <param name="dataAccess"></param>
        /// <returns></returns>
        /// <remarks> This method uses the same handler as the analog object change handler as the action is identical </remarks>
        [ConflictHandlingMethod]
        protected IConflictReport HandleObjectChange(UnstableEnvironment envInfo, IDataAccessor<EnergyModelData> dataAccess)
        {
            Console.WriteLine($"Resolver {typeof(UnstableEnvironmentChangeHandler)} was called for {envInfo.GetType()}");
            return new UnstableEnvironmentChangeHandler(dataAccess, ModelProject).HandleConflicts(envInfo);
        }

        /// <summary>
        ///     Resolves conflicts and data changes if a group interaction is added. The passed object has to be the changed model
        ///     object.
        /// </summary>
        /// <param name="groupInteraction"></param>
        /// <param name="dataAccess"></param>
        /// <returns></returns>
        /// <remarks> This method uses the same handler as the analog object change handler as the action is identical </remarks>
        [ConflictHandlingMethod]
        protected IConflictReport HandleObjectChange(GroupInteraction groupInteraction, IDataAccessor<EnergyModelData> dataAccess)
        {
            Console.WriteLine($"Resolver {typeof(GroupInteractionChangeHandler)} was called for {groupInteraction.GetType()}");
            return new GroupInteractionChangeHandler(dataAccess, ModelProject).HandleConflicts(groupInteraction);
        }
    }
}