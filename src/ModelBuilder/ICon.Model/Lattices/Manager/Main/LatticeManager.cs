using System;
using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Lattices
{
    /// <summary>
    ///     Basic implementation of the lattice manager that manages and supplies objects and parameters related to the lattice
    ///     modelling process
    /// </summary>
    internal class LatticeManager : ModelManager<LatticeModelData, LatticeDataCache, LatticeDataManager, LatticeCacheManager, LatticeInputManager,
        LatticeQueryManager, LatticeEventManager, LatticeUpdateManager>, ILatticeManager
    {
        /// <summary>
        ///     Access to input port for model requests
        /// </summary>
        public new ILatticeInputPort InputAccess => InputManager;

        /// <summary>
        ///     Access to query port for model data and cache queries
        /// </summary>
        public ILatticeQueryPort DataAccess => QueryManager;

        /// <summary>
        ///     Access to the event port for change subscriptions
        /// </summary>
        public new ILatticeEventPort EventAccess => EventManager;

        /// <summary>
        ///     Creates new lattice manager with the provided project services and base data object
        /// </summary>
        /// <param name="modelProject"></param>
        /// <param name="data"></param>
        public LatticeManager(IModelProject modelProject, LatticeModelData data)
            : base(modelProject, data)
        {
        }

        /// <summary>
        ///     Get the  type of the manager interface this class supports
        /// </summary>
        /// <returns></returns>
        public override Type GetManagerInterfaceType() => typeof(ILatticeManager);

        /// <summary>
        ///     Get a new validation service for this manager that uses the settings from the provided project settings and handles
        ///     Lattice object validations
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        public override IValidationService CreateValidationService(ProjectSettings settings)
        {
            if (!settings.TryGetModuleSettings(out MocassinLatticeSettings moduleSettings))
                throw new InvalidOperationException("Settings object for the lattice module is missing");

            return new LatticeValidationService(ModelProject, moduleSettings);
        }
    }
}