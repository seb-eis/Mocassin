using System;
using System.Collections.Generic;

using ICon.Framework.Constraints;
using ICon.Model.Basic;

namespace ICon.Model.Energies
{
    /// <summary>
    /// Setter adapter for savely manipulating group energy values and pushing the changes into the model data
    /// </summary>
    public class GroupEnergySetter : ValueSetter, IGroupEnergySetter
    {
        /// <summary>
        /// Read only collection of the group energy entries
        /// </summary>
        IReadOnlyCollection<GroupEnergyEntry> IGroupEnergySetter.EnergyEntries => throw new NotImplementedException();

        /// <summary>
        /// Interafce access to the group interaction the setter can manipulate
        /// </summary>
        IGroupInteraction IGroupEnergySetter.GroupInteraction => GroupInteraction;

        /// <summary>
        /// Constraint for the energy values
        /// </summary>
        public IValueConstraint<double, double> EnergyConstraint { get; set; }

        /// <summary>
        /// The data accessor provider for the energy model data to create data locks
        /// </summary>
        public IDataAccessorProvider<EnergyModelData> DataAccessorProvider { get; set; }

        /// <summary>
        /// The wrapped group interaction that is manipulated by the setter
        /// </summary>
        protected GroupInteraction GroupInteraction { get; set; }

        /// <summary>
        /// The set of known energy group entries that can be manipulated
        /// </summary>
        protected HashSet<GroupEnergyEntry> EnergyEntries { get; set; }

        /// <summary>
        /// Create new group energy setter from the passed group interaction
        /// </summary>
        /// <param name="positionGroupInfo"></param>
        /// <param name="groupInteraction"></param>
        public GroupEnergySetter(GroupInteraction groupInteraction)
        {
            GroupInteraction = groupInteraction ?? throw new ArgumentNullException(nameof(groupInteraction));
            EnergyEntries = CreateEnergyEntries(groupInteraction);
        }

        /// <summary>
        /// Protected creation of new group energy setter without a group interaction
        /// </summary>
        protected GroupEnergySetter()
        {
        }

        /// <summary>
        /// Pushes the local energy information into the group energy dictionary
        /// </summary>
        public override void PushData()
        {
            foreach (var item in EnergyEntries)
            {
                if (!GroupInteraction.TrySetEnergyEntry(item))
                {
                    OnValuesPushed.OnError(new InvalidOperationException("The state of the group energy setter contains invalid entries"));
                    return;
                }
            }
            OnValuesPushed.OnNext();
        }

        /// <summary>
        /// Sets multiple energy entries in the local energy set
        /// </summary>
        /// <param name="energyEntries"></param>
        public void SetEnergyEntries(IEnumerable<GroupEnergyEntry> energyEntries)
        {
            foreach (var item in energyEntries)
            {
                SetEnergyEntry(item);
            }
        }

        /// <summary>
        /// Sets a single energy value in the local energy set. Distributes an error through affiliated observable if setting fails
        /// </summary>
        /// <param name="energyEntry"></param>
        public void SetEnergyEntry(in GroupEnergyEntry energyEntry)
        {
            if (!EnergyConstraint.IsValid(energyEntry.Energy))
            {
                OnValueChanged.OnError(new ArgumentException("Passed energy entry violates the energy value constraints", nameof(energyEntry)));
                return;
            }
            if (!EnergyEntries.Contains(energyEntry))
            {
                OnValueChanged.OnError(new ArgumentException("Passed energy entry does not exist in the group interaction", nameof(energyEntry)));
                return;
            }
            EnergyEntries.Remove(energyEntry);
            EnergyEntries.Add(energyEntry);
            OnValueChanged.OnNext();
        }

        /// <summary>
        /// Creates an empty group energy setter with an invalid group interaction and an empty energy entry set
        /// </summary>
        /// <returns></returns>
        public static GroupEnergySetter CreateEmpty()
        {
            return new GroupEnergySetter()
            {
                GroupInteraction = new GroupInteraction() { Index = -1, IsDeprecated = true },
                EnergyEntries = new HashSet<GroupEnergyEntry>()
            };
        }

        /// <summary>
        /// Creates the hash set of energy entries for local manipulation from the passed group interaction
        /// </summary>
        /// <param name="groupInteraction"></param>
        /// <returns></returns>
        protected HashSet<GroupEnergyEntry> CreateEnergyEntries(GroupInteraction groupInteraction)
        {
            return new HashSet<GroupEnergyEntry>(groupInteraction.GetEnergyEntries());
        }
    }
}
