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
        /// Registers a new kinetic simulation to the manager if it passes validation (Awaits distribution of affiliated events in case of operation success)
        /// </summary>
        /// <param name="simulation"></param>
        /// <returns></returns>
        [OperationMethod(DataOperationType.NewObject)]
        protected IOperationReport TryRegisterNewObject(IKineticSimulation simulation)
        {
            return DefaultRegisterModelObject(simulation, accessor => accessor.Query(data => data.KineticSimulations));
        }

        /// <summary>
        /// Removes a kinetic simulation from the manager by deprecation if possible (Awaits distribution of affiliated events in case of operation success)
        /// </summary>
        /// <param name="simulation"></param>
        /// <returns></returns>
        [OperationMethod(DataOperationType.ObjectRemoval)]
        protected IOperationReport TryRemoveObject(IKineticSimulation simulation)
        {
            return DefaultRemoveModelObject(simulation, accessor => accessor.Query(data => data.KineticSimulations));
        }

        /// <summary>
        /// Replaces a kinetic simulation in the manager by another if the new one passes validation (Awaits distribution of affiliated events in case of operation success)
        /// </summary>
        /// <param name="orgSimulation"></param>
        /// <param name="newSimulation"></param>
        /// <returns></returns>
        [OperationMethod(DataOperationType.ObjectChange)]
        protected IOperationReport TryReplaceParticle(IKineticSimulation orgSimulation, IKineticSimulation newSimulation)
        {
            return DefaultReplaceModelObject(orgSimulation, newSimulation, accessor => accessor.Query(data => data.KineticSimulations));
        }

        /// <summary>
        /// Registers a new metropolis simulation to the manager if it passes validation (Awaits distribution of affiliated events in case of operation success)
        /// </summary>
        /// <param name="simulation"></param>
        /// <returns></returns>
        [OperationMethod(DataOperationType.NewObject)]
        protected IOperationReport TryRegisterNewObject(IMetropolisSimulation simulation)
        {
            return DefaultRegisterModelObject(simulation, accessor => accessor.Query(data => data.MetropolisSimulations));
        }

        /// <summary>
        /// Removes a metropolis simulation from the manager by deprecation if possible (Awaits distribution of affiliated events in case of operation success)
        /// </summary>
        /// <param name="simulation"></param>
        /// <returns></returns>
        [OperationMethod(DataOperationType.ObjectRemoval)]
        protected IOperationReport TryRemoveObject(IMetropolisSimulation simulation)
        {
            return DefaultRemoveModelObject(simulation, accessor => accessor.Query(data => data.MetropolisSimulations));
        }

        /// <summary>
        /// Replaces a metropolis simulation in the manager by another if the new one passes validation (Awaits distribution of affiliated events in case of operation success)
        /// </summary>
        /// <param name="orgSimulation"></param>
        /// <param name="newSimulation"></param>
        /// <returns></returns>
        [OperationMethod(DataOperationType.ObjectChange)]
        protected IOperationReport TryReplaceParticle(IMetropolisSimulation orgSimulation, IMetropolisSimulation newSimulation)
        {
            return DefaultReplaceModelObject(orgSimulation, newSimulation, accessor => accessor.Query(data => data.MetropolisSimulations));
        }

        /// <summary>
        /// Registers a new kinetic simulation series to the manager if it passes validation (Awaits distribution of affiliated events in case of operation success)
        /// </summary>
        /// <param name="series"></param>
        /// <returns></returns>
        [OperationMethod(DataOperationType.NewObject)]
        protected IOperationReport TryRegisterNewObject(IKineticSimulationSeries series)
        {
            return DefaultRegisterModelObject(series, accessor => accessor.Query(data => data.KineticSeries));
        }

        /// <summary>
        /// Removes a kinetic simulation series from the manager by deprecation if possible (Awaits distribution of affiliated events in case of operation success)
        /// </summary>
        /// <param name="series"></param>
        /// <returns></returns>
        [OperationMethod(DataOperationType.ObjectRemoval)]
        protected IOperationReport TryRemoveObject(IKineticSimulationSeries series)
        {
            return DefaultRemoveModelObject(series, accessor => accessor.Query(data => data.KineticSeries));
        }

        /// <summary>
        /// Replaces a kinetic simulation series in the manager by another if the new one passes validation (Awaits distribution of affiliated events in case of operation success)
        /// </summary>
        /// <param name="orgSeries"></param>
        /// <param name="newSeries"></param>
        /// <returns></returns>
        [OperationMethod(DataOperationType.ObjectChange)]
        protected IOperationReport TryReplaceParticle(IKineticSimulationSeries orgSeries, IKineticSimulationSeries newSeries)
        {
            return DefaultReplaceModelObject(orgSeries, newSeries, accessor => accessor.Query(data => data.KineticSeries));
        }

        /// <summary>
        /// Registers a new metropolis simulation series to the manager if it passes validation (Awaits distribution of affiliated events in case of operation success)
        /// </summary>
        /// <param name="series"></param>
        /// <returns></returns>
        [OperationMethod(DataOperationType.NewObject)]
        protected IOperationReport TryRegisterNewObject(IMetropolisSimulationSeries series)
        {
            return DefaultRegisterModelObject(series, accessor => accessor.Query(data => data.MetropolisSeries));
        }

        /// <summary>
        /// Removes a metropolis simulation series from the manager by deprecation if possible (Awaits distribution of affiliated events in case of operation success)
        /// </summary>
        /// <param name="series"></param>
        /// <returns></returns>
        [OperationMethod(DataOperationType.ObjectRemoval)]
        protected IOperationReport TryRemoveObject(IMetropolisSimulationSeries series)
        {
            return DefaultRemoveModelObject(series, accessor => accessor.Query(data => data.MetropolisSeries));
        }

        /// <summary>
        /// Replaces a metropolis simulation series in the manager by another if the new one passes validation (Awaits distribution of affiliated events in case of operation success)
        /// </summary>
        /// <param name="orgSeries"></param>
        /// <param name="newSeries"></param>
        /// <returns></returns>
        [OperationMethod(DataOperationType.ObjectChange)]
        protected IOperationReport TryReplaceParticle(IMetropolisSimulationSeries orgSeries, IMetropolisSimulationSeries newSeries)
        {
            return DefaultReplaceModelObject(orgSeries, newSeries, accessor => accessor.Query(data => data.MetropolisSeries));
        }

        /// <summary>
        /// Get the conflict handler provider that supplies handlers for internal data conflicts due to user induced changes
        /// </summary>
        /// <returns></returns>
        protected override IDataConflictHandlerProvider<SimulationModelData> MakeConflictHandlerProvider()
        {
            return new ConflictHandling.SimulationDataConflictHandlerProvider(ProjectServices);
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
