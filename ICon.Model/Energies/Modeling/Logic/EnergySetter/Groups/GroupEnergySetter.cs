using System;
using System.Collections.Generic;
using Mocassin.Framework.Constraints;
using Mocassin.Model.Basic;

namespace Mocassin.Model.Energies
{
    /// <inheritdoc cref="IGroupEnergySetter" />
    public class GroupEnergySetter : ValueSetter, IGroupEnergySetter
    {
        /// <inheritdoc />
        IReadOnlyCollection<GroupEnergyEntry> IGroupEnergySetter.EnergyEntries => EnergyEntries;

        /// <inheritdoc />
        IGroupInteraction IGroupEnergySetter.GroupInteraction => GroupInteraction;

        /// <inheritdoc />
        public IValueConstraint<double, double> EnergyConstraint { get; set; }

        /// <summary>
        ///     The data accessor provider for the energy model data to create data locks
        /// </summary>
        public IDataAccessorSource<EnergyModelData> DataAccessorSource { get; set; }

        /// <summary>
        ///     The wrapped group interaction that is manipulated by the setter
        /// </summary>
        protected GroupInteraction GroupInteraction { get; set; }

        /// <summary>
        ///     The set of known energy group entries that can be manipulated
        /// </summary>
        protected HashSet<GroupEnergyEntry> EnergyEntries { get; set; }

        /// <summary>
        ///     Get the <see cref="IEnergyQueryPort" /> to invalidate cached data
        /// </summary>
        protected IEnergyQueryPort EnergyQueryPort { get; }

        /// <inheritdoc />
        public IPositionGroupInfo PositionGroupInfo { get; }

        /// <inheritdoc />
        public GroupEnergySetter(GroupInteraction groupInteraction, IEnergyQueryPort energyQueryPort)
        {
            EnergyQueryPort = energyQueryPort ?? throw new ArgumentNullException(nameof(energyQueryPort));
            GroupInteraction = groupInteraction ?? throw new ArgumentNullException(nameof(groupInteraction));
            EnergyEntries = CreateEnergyEntries(groupInteraction);
            PositionGroupInfo = energyQueryPort.Query(x => x.GetPositionGroupInfo(groupInteraction.Index));
        }

        /// <inheritdoc />
        protected GroupEnergySetter()
        {
        }

        /// <inheritdoc />
        public override void PushData()
        {
            foreach (var item in EnergyEntries)
            {
                if (GroupInteraction.TrySetEnergyEntry(item))
                    continue;

                OnValuesPushed.OnError(new InvalidOperationException("The state of the group energy setter contains invalid entries"));
                return;
            }

            EnergyQueryPort?.Query(x => x.ClearCachedData());
            OnValuesPushed.OnNext();
        }

        /// <inheritdoc />
        public void SetEnergyEntries(IEnumerable<GroupEnergyEntry> energyEntries)
        {
            foreach (var item in energyEntries)
                SetEnergyEntry(item);
        }

        /// <inheritdoc />
        public void SetEnergyEntry(in GroupEnergyEntry energyEntry)
        {
            if (!EnergyConstraint.IsValid(energyEntry.Energy))
            {
                OnValueChanged.OnError(new ArgumentException("Passed energy entry violates the energy value constraints",
                    nameof(energyEntry)));
                return;
            }

            if (!EnergyEntries.Contains(energyEntry))
            {
                OnValueChanged.OnError(new ArgumentException("Passed energy entry does not exist in the group interaction",
                    nameof(energyEntry)));
                return;
            }

            EnergyEntries.Remove(energyEntry);
            EnergyEntries.Add(energyEntry);
            OnValueChanged.OnNext();
        }

        /// <summary>
        ///     Creates an empty group energy setter with an invalid group interaction and an empty energy entry set
        /// </summary>
        /// <returns></returns>
        public static GroupEnergySetter CreateEmpty()
        {
            return new GroupEnergySetter
            {
                GroupInteraction = new GroupInteraction {Index = -1, IsDeprecated = true},
                EnergyEntries = new HashSet<GroupEnergyEntry>()
            };
        }

        /// <summary>
        ///     Creates the hash set of energy entries for local manipulation from the passed group interaction
        /// </summary>
        /// <param name="groupInteraction"></param>
        /// <returns></returns>
        protected HashSet<GroupEnergyEntry> CreateEnergyEntries(GroupInteraction groupInteraction)
        {
            return new HashSet<GroupEnergyEntry>(groupInteraction.GetEnergyEntries());
        }
    }
}