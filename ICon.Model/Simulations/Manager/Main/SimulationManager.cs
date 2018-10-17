using System;
using System.Collections.Generic;
using System.Text;
using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Simulations
{
    /// <summary>
    /// Implementation of the simulation manager that handles the creation and packaging of simulation sets for simulation encoding
    /// </summary>
    internal class SimulationManager : ModelManager<SimulationModelData, SimulationDataCache, SimulationDataManager, SimulationCacheManager, SimulationInputManager, SimulationQueryManager, SimulationEventManager, SimulationUpdateManager>, ISimulationManager
    {
        /// <summary>
        /// Get access to the simulation manager query port
        /// </summary>
        public ISimulationQueryPort QueryPort => QueryManager;

        /// <summary>
        /// Get access to the simulation manager input port
        /// </summary>
        public new ISimulationInputPort InputPort => InputManager;

        /// <summary>
        /// Get access to the simulatuion manager event port
        /// </summary>
        public new ISimulationEventPort EventPort => EventManager;

        /// <summary>
        /// Create new simulation manager with the provided project services and manages the provided simulation data object
        /// </summary>
        /// <param name="modelProject"></param>
        /// <param name="data"></param>
        public SimulationManager(IModelProject modelProject, SimulationModelData data) : base(modelProject, data)
        {
        }

        /// <summary>
        /// Get the type of the manager interface
        /// </summary>
        /// <returns></returns>
        public override Type GetManagerInterfaceType()
        {
            return typeof(ISimulationManager);
        }

        /// <summary>
        /// Get the validation service for simulation manager related parameters and objects
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        public override IValidationService CreateValidationService(ProjectSettings settings)
        {
            if (!settings.TryGetModuleSettings(out MocassinSimulationSettings moduleSettings))
                throw new InvalidOperationException("Settings object for the simulation module is missing");

            return new SimulationValidationService(moduleSettings, ModelProject);
        }
    }
}
