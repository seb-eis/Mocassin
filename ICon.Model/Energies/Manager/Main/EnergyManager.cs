using System;
using System.Collections.Generic;
using System.Text;

using ICon.Model.Basic;
using ICon.Model.ProjectServices;

namespace ICon.Model.Energies
{
    /// <summary>
    /// Basic implementation of the energy manager that manages and supplies objects and parameters related to the energetic modelling process
    /// </summary>
    internal class EnergyManager : ModelManager<EnergyModelData, EnergyDataCache, EnergyDataManager, EnergyCacheManager, EnergyInputManager, EnergyQueryManager, EnergyEventManager, EnergyUpdateManager>, IEnergyManager
    {
        /// <summary>
        /// Access to input port for model requests
        /// </summary>
        public new IEnergyInputPort InputPort => InputManager;

        /// <summary>
        /// Access to query port for model data and cache queries
        /// </summary>
        public IEnergyQueryPort QueryPort => QueryManager;

        /// <summary>
        /// Access to the event port for change subscriptions
        /// </summary>
        public new IEnergyEventPort EventPort => EventManager;

        /// <summary>
        /// Creates new energy manager with provided project service and base data object
        /// </summary>
        /// <param name="projectServices"></param>
        /// <param name="data"></param>
        public EnergyManager(IProjectServices projectServices, EnergyModelData data) : base(projectServices, data)
        {

        }

        /// <summary>
        /// Get the  type of the manager interface this class supports
        /// </summary>
        /// <returns></returns>
        public override Type GetManagerInterfaceType()
        {
            return typeof(IEnergyManager);
        }

        /// <summary>
        /// Get a new validation service for this manager that uses the settings from the provided project settings and handles energy object validations
        /// </summary>
        /// <param name="settingsData"></param>
        /// <returns></returns>
        public override IValidationService MakeValidationService(ProjectSettingsData settingsData)
        {
            return new EnergyValidationService(ProjectServices, settingsData.EnergySettings);
        }
    }
}
