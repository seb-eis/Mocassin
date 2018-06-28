using System;
using System.Collections.Generic;

using ICon.Framework.Constraints;
using ICon.Model.Basic;

namespace ICon.Model.Energies
{
    /// <summary>
    /// Energy setter to set energy values for occupation permutatins of pair interactions
    /// </summary>
    public class PairEnergySetter : ValueSetter, IPairEnergySetter
    {
        /// <summary>
        /// Read only access to the local energy entry set
        /// </summary>
        IReadOnlyCollection<PairEnergyEntry> IPairEnergySetter.EnergyEntries => EnergyEntries;

        /// <summary>
        /// Get read access to the pair interaction the setter can manipulate
        /// </summary>
        IPairInteraction IPairEnergySetter.PairInteraction => PairInteraction;

        /// <summary>
        /// Constraint for the energy values
        /// </summary>
        public IValueConstraint<double, double> EnergyConstraint { get; set; }

        /// <summary>
        /// The pair interaction that is manipulated by the setter
        /// </summary>
        public PairInteraction PairInteraction { get; set; }

        /// <summary>
        /// The local energy entry list that containns all energy entries of the pair interaction
        /// </summary>
        public HashSet<PairEnergyEntry> EnergyEntries { get; set; }

        /// <summary>
        /// The energy data access provider to lock the energy data object
        /// </summary>
        /// <remarks> Used to lock the energy data object as long as the setter is writing energy values </remarks>
        public IDataAccessorProvider<EnergyModelData> DataAccessorProvider { get; set; }

        /// <summary>
        /// Creates new pair energy setter for the provided pair interaction
        /// </summary>
        /// <param name="pairInteraction"></param>
        public PairEnergySetter(PairInteraction pairInteraction)
        {
            PairInteraction = pairInteraction ?? throw new ArgumentNullException(nameof(pairInteraction));
            EnergyEntries = CreateEnergySet(pairInteraction);
        }

        /// <summary>
        /// Pushes the data into the wrapped pair interaction. Distributes an error throug the event provider if failed
        /// </summary>
        public override void PushData()
        {
            using (DataAccessorProvider.CreateInterface())
            {
                foreach (var item in EnergyEntries)
                {
                    if (!PairInteraction.TrySetEnergyEntry(item))
                    {
                        OnValuesPushed.OnError(new  InvalidOperationException("The state of the setter contains invalid objects"));
                        return;
                    }
                }
            }
            OnValuesPushed.OnNext();
        }

        /// <summary>
        /// Set a single energy value in the local energy dictionary and distributes the on value changed event
        /// </summary>
        /// <param name="energyEntry"></param>
        public void SetEnergyValue(in PairEnergyEntry energyEntry)
        {
            if (!EnergyConstraint.IsValid(energyEntry.Energy))
            {
                OnValueChanged.OnError(new ArgumentException("Energy value violates the value constraints", nameof(energyEntry)));
                return;
            }
            if (!EnergyEntries.Contains(energyEntry))
            {
                OnValueChanged.OnError(new ArgumentException("The passed energy entry does not exists in the pair interaction", nameof(energyEntry)));
                return;
            }

            EnergyEntries.Remove(energyEntry);
            EnergyEntries.Add(energyEntry);
            OnValueChanged.OnNext();
        }

        /// <summary>
        /// Sets multiple energy entries in the local set
        /// </summary>
        /// <param name="energyEntries"></param>
        public void SetEnergyValues(IEnumerable<PairEnergyEntry> energyEntries)
        {
            foreach (var item in energyEntries)
            {
                SetEnergyValue(item);
            }
        }

        /// <summary>
        /// Create the local energy set for manipulating values before pushing them into the model
        /// </summary>
        /// <param name="pairInteraction"></param>
        /// <returns></returns>
        protected HashSet<PairEnergyEntry> CreateEnergySet(PairInteraction pairInteraction)
        {
            return new HashSet<PairEnergyEntry>(pairInteraction.GetEnergyEntries());
        }
    }
}
