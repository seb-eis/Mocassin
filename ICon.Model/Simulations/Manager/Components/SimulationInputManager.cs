using System;
using System.Collections.Generic;
using System.Text;
using ICon.Framework.Operations;
using ICon.Model.Basic;
using ICon.Model.ProjectServices;

namespace ICon.Model.Simulations
{
    /// <summary>
    /// Implementation of the simulation manager input manager that handles user induces change requests to the simulation model data
    /// </summary>
    internal class SimulationInputManager : ModelInputManager<SimulationModelData, ISimulationDataPort, SimulationEventManager>, ISimulationInputPort
    {
        /// <summary>
        /// Create new simulation input manager using the provided data object, event manager and project services
        /// </summary>
        /// <param name="data"></param>
        /// <param name="manager"></param>
        /// <param name="services"></param>
        public SimulationInputManager(SimulationModelData data, SimulationEventManager manager, IProjectServices services) : base(data, manager, services)
        {

        }

        /// <summary>
        /// Get the conflict handler provider that supplies handlers for internal data conflicts due to user induced changes
        /// </summary>
        /// <returns></returns>
        protected override IDataConflictHandlerProvider<SimulationModelData> MakeConflictHandlerProvider()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Cleans all deprecated simulation model objects and distributes the chaganges throug the event manager
        /// </summary>
        /// <returns></returns>
        [OperationMethod(DataOperationType.ObjectCleaning)]
        protected override IOperationReport TryCleanDeprecatedData()
        {
            return DefaultCleanDeprecatedData();
        }
    }
}
