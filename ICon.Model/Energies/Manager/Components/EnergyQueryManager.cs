using Mocassin.Model.Basic;

namespace Mocassin.Model.Energies
{
    /// <summary>
    ///     Basic implementation of the energy query manager that handles safe data queries and service requests to the energy
    ///     manager from outside sources
    /// </summary>
    internal class EnergyQueryManager : ModelQueryManager<EnergyModelData, IEnergyDataPort, EnergyModelCache, IEnergyCachePort>,
        IEnergyQueryPort
    {
        /// <inheritdoc />
        public EnergyQueryManager(EnergyModelData baseData, EnergyModelCache extendedModel, AccessLockSource lockSource)
            : base(baseData, extendedModel, lockSource)
        {
        }
    }
}