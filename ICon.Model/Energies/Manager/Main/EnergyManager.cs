using System;
using ICon.Model.Basic;
using ICon.Model.ProjectServices;

namespace ICon.Model.Energies
{
    /// <summary>
    ///     Basic implementation of the energy manager that manages and supplies objects and parameters related to the
    ///     energetic modeling process
    /// </summary>
    internal class EnergyManager : ModelManager<EnergyModelData, EnergyModelCache, EnergyDataManager, EnergyCacheManager, EnergyInputManager
        , EnergyQueryManager, EnergyEventManager, EnergyUpdateManager>, IEnergyManager
    {
        /// <inheritdoc />
        public new IEnergyInputPort InputPort => InputManager;

        /// <inheritdoc />
        public IEnergyQueryPort QueryPort => QueryManager;

        /// <inheritdoc />
        public new IEnergyEventPort EventPort => EventManager;

        /// <inheritdoc />
        public EnergyManager(IProjectServices projectServices, EnergyModelData data)
            : base(projectServices, data)
        {
        }

        /// <inheritdoc />
        public override Type GetManagerInterfaceType()
        {
            return typeof(IEnergyManager);
        }

        /// <inheritdoc />
        public override IValidationService CreateValidationService(ProjectSettingsData settingsData)
        {
            return new EnergyValidationService(ProjectServices, settingsData.EnergySettings);
        }
    }
}