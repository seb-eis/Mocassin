using System;
using System.Linq;
using Mocassin.Framework.Collections;
using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;
using Mocassin.Model.Structures;

namespace Mocassin.Model.Energies
{
    /// <summary>
    ///     Energy data manager that provides safe read only access to the energy base model data
    /// </summary>
    internal class EnergyDataManager : ModelDataManager<EnergyModelData>, IEnergyDataPort
    {
        /// <inheritdoc />
        public EnergyDataManager(EnergyModelData modelData)
            : base(modelData)
        {
        }

        /// <inheritdoc />
        public IStableEnvironmentInfo GetStableEnvironmentInfo() => Data.StableEnvironmentInfo;

        /// <inheritdoc />
        public IGroupInteraction GetGroupInteraction(int index) => Data.GroupInteractions[index];

        /// <inheritdoc />
        public FixedList<IGroupInteraction> GetGroupInteractions() => FixedList<IGroupInteraction>.FromEnumerable(Data.GroupInteractions);

        /// <inheritdoc />
        public IStablePairInteraction GetStablePairInteraction(int index) => Data.StablePairInteractions[index];

        /// <inheritdoc />
        public FixedList<IStablePairInteraction> GetStablePairInteractions() => FixedList<IStablePairInteraction>.FromEnumerable(Data.StablePairInteractions);

        /// <inheritdoc />
        public IUnstableEnvironment GetUnstableEnvironment(int index) => Data.UnstableEnvironments[index];


        /// <inheritdoc />
        public IUnstableEnvironment GetUnstableEnvironment(ICellSite cellSite)
        {
            return Data.UnstableEnvironments.SingleOrDefault(x => x.CellSite == cellSite);
        }

        /// <inheritdoc />
        public FixedList<IUnstableEnvironment> GetUnstableEnvironments() => FixedList<IUnstableEnvironment>.FromEnumerable(Data.UnstableEnvironments);

        /// <inheritdoc />
        public IUnstablePairInteraction GetUnstablePairInteractions(int index) => Data.UnstablePairInteractions[index];

        /// <inheritdoc />
        public FixedList<IUnstablePairInteraction> GetUnstablePairInteractions() =>
            FixedList<IUnstablePairInteraction>.FromEnumerable(Data.UnstablePairInteractions);

        /// <inheritdoc />
        public IEnergySetterProvider GetEnergySetterProvider(ProjectSettings projectSettings, IEnergyQueryPort queryPort)
        {
            if (projectSettings == null)
                throw new ArgumentNullException(nameof(projectSettings));

            var energySettings = projectSettings.GetModuleSettings<MocassinEnergySettings>();
            var accessLockSource = new AccessLockSource(projectSettings.ConcurrencySettings.MaxAttempts,
                projectSettings.ConcurrencySettings.AttemptInterval);

            return new EnergySetterProvider(new DataAccessorSource<EnergyModelData>(Data, accessLockSource), queryPort)
            {
                GroupEnergyConstraint = energySettings.GroupEnergies.ToConstraint(),
                PairEnergyConstraint = energySettings.PairEnergies.ToConstraint()
            };
        }
    }
}