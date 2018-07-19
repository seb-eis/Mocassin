using System;
using System.Collections.Generic;
using System.Text;
using ICon.Framework.Operations;
using ICon.Model.Basic;
using ICon.Model.ProjectServices;

namespace ICon.Model.Energies.ConflictHandling
{
    /// <summary>
    /// Energy object change handler that hanldes data changes and conflict resolving for changed energy model objects
    /// </summary>
    public class EnergyObjectChangeHandler : DataConflictHandler<EnergyModelData, ModelObject>
    {
        /// <summary>
        /// Create new energy object change handler that uses the provided project service
        /// </summary>
        /// <param name="projectServices"></param>
        public EnergyObjectChangeHandler(IProjectServices projectServices) : base(projectServices)
        {

        }

        /// <summary>
        /// Resolves conflicts and data changes if an unstable environment changes. The passed object represents the changed model object
        /// </summary>
        /// <param name="envInfo"></param>
        /// <param name="dataAccess"></param>
        /// <returns></returns>
        [ConflictHandlingMethod]
        protected IConflictReport HandleObjectChange(UnstableEnvironment envInfo, IDataAccessor<EnergyModelData> dataAccess)
        {
            Console.WriteLine($"Resolver {typeof(UnstableEnvironmentChangeHandler)} was called for {envInfo.GetType().ToString()}");
            return new UnstableEnvironmentChangeHandler(dataAccess, ProjectServices).HandleConflicts(envInfo);
        }

        /// <summary>
        /// Resolves conflicts and data changes if a group interaction is added. The passed object has to be the changed model object.
        /// </summary>
        /// <param name="groupInteraction"></param>
        /// <param name="dataAccess"></param>
        /// <returns></returns>
        [ConflictHandlingMethod]
        protected IConflictReport HandleObjectChange(GroupInteraction groupInteraction, IDataAccessor<EnergyModelData> dataAccess)
        {
            Console.WriteLine($"Resolver {typeof(GroupInteractionChangeHandler)} was called for {groupInteraction.GetType().ToString()}");
            return new GroupInteractionChangeHandler(dataAccess, ProjectServices).HandleConflicts(groupInteraction);
        }
    }
}
