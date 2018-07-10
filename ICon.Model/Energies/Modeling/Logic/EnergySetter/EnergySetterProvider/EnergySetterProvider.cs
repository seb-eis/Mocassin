using System;
using System.Collections.Generic;
using System.Linq;

using ICon.Framework.Constraints;
using ICon.Model.Basic;
using ICon.Model.ProjectServices;

namespace ICon.Model.Energies
{
    /// <summary>
    /// Energy setter provider that dynamically creates setter adapter to manipulate pair and group interaction energy entries
    /// </summary>
    public class EnergySetterProvider : IEnergySetterProvider
    {
        /// <summary>
        /// The energy model data object to supply access to the interaction model objects
        /// </summary>
        protected EnergyModelData EnergyModelData { get; set; }

        /// <summary>
        /// The data accessor provider used to lock the data object by the setter adapters
        /// </summary>
        protected IDataAccessorProvider<EnergyModelData> DataAccessorProvider { get; set; }

        /// <summary>
        /// Value constraint for the pair energies
        /// </summary>
        public IValueConstraint<double, double> PairEnergyConstraint { get; set; }

        /// <summary>
        /// Value constraint for the group energies
        /// </summary>
        public IValueConstraint<double, double> GroupEnergyConstraint { get; set; }

        /// <summary>
        /// Creates new energy setter provider that uses the passed mode data object
        /// </summary>
        /// <param name="energyModelData"></param>
        public EnergySetterProvider(EnergyModelData energyModelData)
        {
            EnergyModelData = energyModelData ?? throw new ArgumentNullException(nameof(energyModelData));
            DataAccessorProvider = DataAccessProvider.Create(EnergyModelData, new DataAccessLocker(10, TimeSpan.FromMilliseconds(100)));
        }

        /// <summary>
        /// Get a read only list of all stable pair interaction energy setters
        /// </summary>
        /// <returns></returns>
        public IReadOnlyList<IPairEnergySetter> GetStablePairEnergySetters()
        {
            return EnergyModelData.StablePairInteractions.Select(value => GetStablePairEnergySetter(value.Index)).ToList();
        }

        /// <summary>
        /// Get the stable pair interaction energy setter for the interaction at the provided index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public IPairEnergySetter GetStablePairEnergySetter(int index)
        {
            return new PairEnergySetter(EnergyModelData.StablePairInteractions[index])
            {
                DataAccessorProvider = DataAccessorProvider,
                EnergyConstraint = PairEnergyConstraint
            };
        }

        /// <summary>
        /// Get a read only list of all unstable pair interaction energy setters
        /// </summary>
        /// <returns></returns>
        public IReadOnlyList<IPairEnergySetter> GetUnstablePairEnergySetters()
        {
            return EnergyModelData.UnstablePairInteractions.Select(value => GetUnstablePairEnergySetter(value.Index)).ToList();
        }

        /// <summary>
        /// Get the unstable pair interaction energy setter for the interaction at the provided index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public IPairEnergySetter GetUnstablePairEnergySetter(int index)
        {
            return new PairEnergySetter(EnergyModelData.UnstablePairInteractions[index])
            {
                DataAccessorProvider = DataAccessorProvider,
                EnergyConstraint = PairEnergyConstraint
            };
        }

        /// <summary>
        /// Get a read only list of all energy setters for group interactions
        /// </summary>
        /// <returns></returns>
        public IReadOnlyList<IGroupEnergySetter> GetGroupEnergySetters()
        {
            return EnergyModelData.GroupInteractions.Select(value => GetGroupEnergySetter(value.Index)).ToList();
        }

        /// <summary>
        /// Get the group interaction energy setter for the group interaction at the provided index. Retruns an empty setter if the group is deprecated
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public IGroupEnergySetter GetGroupEnergySetter(int index)
        {
            var groupInteraction = EnergyModelData.GroupInteractions[index];
            if (groupInteraction.IsDeprecated)
            {
                return GroupEnergySetter.CreateEmpty();
            }

            return new GroupEnergySetter(EnergyModelData.GroupInteractions[index])
            {
                DataAccessorProvider = DataAccessorProvider,
                EnergyConstraint = GroupEnergyConstraint
            };
        }
    }
}
