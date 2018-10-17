using System;
using System.Collections.Generic;
using System.Linq;
using Mocassin.Framework.Constraints;
using Mocassin.Model.Basic;

namespace Mocassin.Model.Energies
{
    /// <inheritdoc />
    public class EnergySetterProvider : IEnergySetterProvider
    {
        /// <summary>
        ///     The energy model data object to supply access to the interaction model objects
        /// </summary>
        protected EnergyModelData EnergyModelData { get; set; }

        /// <summary>
        ///     The data accessor provider used to lock the data object by the setter adapters
        /// </summary>
        protected IDataAccessorSource<EnergyModelData> DataAccessorSource { get; set; }

        /// <inheritdoc />
        public IValueConstraint<double, double> PairEnergyConstraint { get; set; }

        /// <inheritdoc />
        public IValueConstraint<double, double> GroupEnergyConstraint { get; set; }

        /// <summary>
        ///     Creates new energy setter provider that uses the passed mode data object
        /// </summary>
        /// <param name="energyModelData"></param>
        public EnergySetterProvider(EnergyModelData energyModelData)
        {
            EnergyModelData = energyModelData ?? throw new ArgumentNullException(nameof(energyModelData));
            DataAccessorSource = Basic.DataAccessorSource.Create(EnergyModelData, new AccessLockSource(10, TimeSpan.FromMilliseconds(100)));
        }

        /// <inheritdoc />
        public IReadOnlyList<IPairEnergySetter> GetStablePairEnergySetters()
        {
            return EnergyModelData.StablePairInteractions.Select(value => GetStablePairEnergySetter(value.Index)).ToList();
        }

        /// <inheritdoc />
        public IPairEnergySetter GetStablePairEnergySetter(int index)
        {
            return new PairEnergySetter(EnergyModelData.StablePairInteractions[index])
            {
                DataAccessorSource = DataAccessorSource,
                EnergyConstraint = PairEnergyConstraint
            };
        }

        /// <inheritdoc />
        public IReadOnlyList<IPairEnergySetter> GetUnstablePairEnergySetters()
        {
            return EnergyModelData.UnstablePairInteractions.Select(value => GetUnstablePairEnergySetter(value.Index)).ToList();
        }

        /// <inheritdoc />
        public IPairEnergySetter GetUnstablePairEnergySetter(int index)
        {
            return new PairEnergySetter(EnergyModelData.UnstablePairInteractions[index])
            {
                DataAccessorSource = DataAccessorSource,
                EnergyConstraint = PairEnergyConstraint
            };
        }

        /// <inheritdoc />
        public IReadOnlyList<IGroupEnergySetter> GetGroupEnergySetters()
        {
            return EnergyModelData.GroupInteractions.Select(value => GetGroupEnergySetter(value.Index)).ToList();
        }

        /// <inheritdoc />
        public IGroupEnergySetter GetGroupEnergySetter(int index)
        {
            var groupInteraction = EnergyModelData.GroupInteractions[index];
            if (groupInteraction.IsDeprecated) 
                return GroupEnergySetter.CreateEmpty();

            return new GroupEnergySetter(EnergyModelData.GroupInteractions[index])
            {
                DataAccessorSource = DataAccessorSource,
                EnergyConstraint = GroupEnergyConstraint
            };
        }
    }
}