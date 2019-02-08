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
        public IStableEnvironmentInfo GetStableEnvironmentInfo()
        {
            return Data.StableEnvironmentInfo;
        }

        /// <inheritdoc />
        public IGroupInteraction GetGroupInteraction(int index)
        {
            return Data.GroupInteractions[index];
        }

        /// <inheritdoc />
        public ReadOnlyListAdapter<IGroupInteraction> GetGroupInteractions()
        {
            return ReadOnlyListAdapter<IGroupInteraction>.FromEnumerable(Data.GroupInteractions);
        }

        /// <inheritdoc />
        public ISymmetricPairInteraction GetStablePairInteraction(int index)
        {
            return Data.StablePairInteractions[index];
        }

        /// <inheritdoc />
        public ReadOnlyListAdapter<ISymmetricPairInteraction> GetStablePairInteractions()
        {
            return ReadOnlyListAdapter<ISymmetricPairInteraction>.FromEnumerable(Data.StablePairInteractions);
        }

        /// <inheritdoc />
        public IUnstableEnvironment GetUnstableEnvironment(int index)
        {
            return Data.UnstableEnvironments[index];
        }


        /// <inheritdoc />
        public IUnstableEnvironment GetUnstableEnvironment(IUnitCellPosition unitCellPosition)
        {
            return Data.UnstableEnvironments.SingleOrDefault(x => x.UnitCellPosition == unitCellPosition);
        }

        /// <inheritdoc />
        public ReadOnlyListAdapter<IUnstableEnvironment> GetUnstableEnvironments()
        {
            return ReadOnlyListAdapter<IUnstableEnvironment>.FromEnumerable(Data.UnstableEnvironments);
        }

        /// <inheritdoc />
        public IAsymmetricPairInteraction GetUnstablePairInteractions(int index)
        {
            return Data.UnstablePairInteractions[index];
        }

        /// <inheritdoc />
        public ReadOnlyListAdapter<IAsymmetricPairInteraction> GetUnstablePairInteractions()
        {
            return ReadOnlyListAdapter<IAsymmetricPairInteraction>.FromEnumerable(Data.UnstablePairInteractions);
        }

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