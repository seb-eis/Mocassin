using Mocassin.Framework.Operations;
using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;
using Mocassin.Model.Simulations.ConflictHandling;

namespace Mocassin.Model.Simulations
{
    /// <inheritdoc cref="ISimulationInputPort" />
    internal class SimulationInputManager : ModelInputManager<SimulationModelData, ISimulationDataPort, SimulationEventManager>,
        ISimulationInputPort
    {
        /// <inheritdoc />
        public SimulationInputManager(SimulationModelData modelData, SimulationEventManager eventManager, IModelProject project)
            : base(modelData, eventManager, project)
        {
        }

        /// <summary>
        ///     Registers a new kinetic simulation to the manager if it passes validation (Awaits distribution of affiliated events
        ///     in case of operation success)
        /// </summary>
        /// <param name="simulation"></param>
        /// <returns></returns>
        [DataOperation(DataOperationType.NewObject)]
        protected IOperationReport TryRegisterNewObject(IKineticSimulation simulation)
        {
            return DefaultRegisterModelObject(simulation, accessor => accessor.Query(data => data.KineticSimulations));
        }

        /// <summary>
        ///     Removes a kinetic simulation from the manager by deprecation if possible (Awaits distribution of affiliated events
        ///     in case of operation success)
        /// </summary>
        /// <param name="simulation"></param>
        /// <returns></returns>
        [DataOperation(DataOperationType.ObjectRemoval)]
        protected IOperationReport TryRemoveObject(IKineticSimulation simulation)
        {
            return DefaultRemoveModelObject(simulation, accessor => accessor.Query(data => data.KineticSimulations));
        }

        /// <summary>
        ///     Replaces a kinetic simulation in the manager by another if the new one passes validation (Awaits distribution of
        ///     affiliated events in case of operation success)
        /// </summary>
        /// <param name="orgSimulation"></param>
        /// <param name="newSimulation"></param>
        /// <returns></returns>
        [DataOperation(DataOperationType.ObjectChange)]
        protected IOperationReport TryReplaceParticle(IKineticSimulation orgSimulation, IKineticSimulation newSimulation)
        {
            return DefaultReplaceModelObject(orgSimulation, newSimulation, accessor => accessor.Query(data => data.KineticSimulations));
        }

        /// <summary>
        ///     Registers a new metropolis simulation to the manager if it passes validation (Awaits distribution of affiliated
        ///     events in case of operation success)
        /// </summary>
        /// <param name="simulation"></param>
        /// <returns></returns>
        [DataOperation(DataOperationType.NewObject)]
        protected IOperationReport TryRegisterNewObject(IMetropolisSimulation simulation)
        {
            return DefaultRegisterModelObject(simulation, accessor => accessor.Query(data => data.MetropolisSimulations));
        }

        /// <summary>
        ///     Removes a metropolis simulation from the manager by deprecation if possible (Awaits distribution of affiliated
        ///     events in case of operation success)
        /// </summary>
        /// <param name="simulation"></param>
        /// <returns></returns>
        [DataOperation(DataOperationType.ObjectRemoval)]
        protected IOperationReport TryRemoveObject(IMetropolisSimulation simulation)
        {
            return DefaultRemoveModelObject(simulation, accessor => accessor.Query(data => data.MetropolisSimulations));
        }

        /// <summary>
        ///     Replaces a metropolis simulation in the manager by another if the new one passes validation (Awaits distribution of
        ///     affiliated events in case of operation success)
        /// </summary>
        /// <param name="orgSimulation"></param>
        /// <param name="newSimulation"></param>
        /// <returns></returns>
        [DataOperation(DataOperationType.ObjectChange)]
        protected IOperationReport TryReplaceParticle(IMetropolisSimulation orgSimulation, IMetropolisSimulation newSimulation)
        {
            return DefaultReplaceModelObject(orgSimulation, newSimulation, accessor => accessor.Query(data => data.MetropolisSimulations));
        }

        /// <summary>
        ///     Get the conflict handler provider that supplies handlers for internal data conflicts due to user induced changes
        /// </summary>
        /// <returns></returns>
        protected override IDataConflictHandlerProvider<SimulationModelData> CreateDataConflictHandlerProvider()
        {
            return new SimulationDataConflictHandlerProvider(ModelProject);
        }

        /// <inheritdoc />
        [DataOperation(DataOperationType.ObjectCleaning)]
        protected override IOperationReport TryCleanDeprecatedData()
        {
            return DefaultCleanDeprecatedData();
        }
    }
}