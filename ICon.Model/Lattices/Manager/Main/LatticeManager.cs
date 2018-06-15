using System;
using System.Collections.Generic;
using System.Text;

using ICon.Model.Basic;
using ICon.Model.ProjectServices;

namespace ICon.Model.Lattices
{
    /// <summary>
    /// Basic implementation of the lattice manager that manages and supplies objects and parameters related to the lattice modelling process
    /// </summary>
    internal class LatticeManager : ModelManager<LatticeModelData, LatticeDataCache, LatticeDataManager, LatticeCacheManager, LatticeInputManager, LatticeQueryManager, LatticeEventManager, LatticeUpdateManager>, ILatticeManager
    {
        /// <summary>
        /// Access to input port for model requests
        /// </summary>
        public new ILatticeInputPort InputPort => InputManager;

        /// <summary>
        /// Access to query port for model data and cache queries
        /// </summary>
        public ILatticeQueryPort QueryPort => QueryManager;

        /// <summary>
        /// Access to the event port for change subscriptions
        /// </summary>
        public new ILatticeEventPort EventPort => EventManager;

        /// <summary>
        /// Creates new lattice manager with the provided project services and base data object
        /// </summary>
        /// <param name="projectServices"></param>
        /// <param name=""></param>
        public LatticeManager(IProjectServices projectServices, LatticeModelData data) : base(projectServices, data)
        {

        }

        /// <summary>
        /// Get the  type of the manager interface this class supports
        /// </summary>
        /// <returns></returns>
        public override Type GetManagerInterfaceType()
        {
            return typeof(ILatticeManager);
        }

        /// <summary>
        /// Get a new validation service for this manager that uses the settings from the provided project settings and handles Lattice object validations
        /// </summary>
        /// <param name="settingsData"></param>
        /// <returns></returns>
        public override IValidationService MakeValidationService(ProjectSettingsData settingsData)
        {
            return new LatticeValidationService(ProjectServices, settingsData.LatticeSettings);
        }
    }
}
