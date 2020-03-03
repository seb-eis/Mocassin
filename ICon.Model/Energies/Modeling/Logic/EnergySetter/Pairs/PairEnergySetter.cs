using System;
using System.Collections.Generic;
using Mocassin.Framework.Constraints;
using Mocassin.Model.Basic;

namespace Mocassin.Model.Energies
{
    /// <inheritdoc cref="IPairEnergySetter" />
    public class PairEnergySetter : ValueSetter, IPairEnergySetter
    {
        /// <inheritdoc />
        IReadOnlyCollection<PairEnergyEntry> IPairEnergySetter.EnergyEntries => EnergyEntries;

        /// <inheritdoc />
        IPairInteraction IPairEnergySetter.PairInteraction => PairInteraction;

        /// <inheritdoc />
        public IValueConstraint<double, double> EnergyConstraint { get; set; }

        /// <summary>
        ///     The pair interaction that is manipulated by the setter
        /// </summary>
        public PairInteraction PairInteraction { get; set; }

        /// <summary>
        ///     The local energy entry list that contains all energy entries of the pair interaction
        /// </summary>
        public HashSet<PairEnergyEntry> EnergyEntries { get; set; }

        /// <summary>
        ///     The energy data access provider to lock the energy data object
        /// </summary>
        /// <remarks> Used to lock the energy data object as long as the setter is writing energy values </remarks>
        public IDataAccessorSource<EnergyModelData> DataAccessorSource { get; set; }

        /// <summary>
        ///     Get the <see cref="IEnergyQueryPort" /> to invalidate cached data
        /// </summary>
        protected IEnergyQueryPort EnergyQueryPort { get; }

        /// <inheritdoc />
        public PairEnergySetter(PairInteraction pairInteraction, IEnergyQueryPort energyQueryPort)
        {
            EnergyQueryPort = energyQueryPort ?? throw new ArgumentNullException(nameof(energyQueryPort));
            PairInteraction = pairInteraction ?? throw new ArgumentNullException(nameof(pairInteraction));
            EnergyEntries = CreateEnergySet(pairInteraction);
        }

        /// <inheritdoc />
        public override void PushData()
        {
            using (DataAccessorSource.Create())
            {
                foreach (var item in EnergyEntries)
                {
                    if (PairInteraction.TrySetEnergyEntry(item))
                        continue;

                    OnValuesPushed.OnError(new InvalidOperationException("The state of the setter contains invalid objects"));
                    return;
                }
            }

            EnergyQueryPort?.Query(x => x.ClearCachedData());
            OnValuesPushed.OnNext();
        }

        /// <inheritdoc />
        public void SetEnergyValue(PairEnergyEntry energyEntry)
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

        /// <inheritdoc />
        public void SetEnergyValues(IEnumerable<PairEnergyEntry> energyEntries)
        {
            foreach (var item in energyEntries)
                SetEnergyValue(item);
        }

        /// <summary>
        ///     Create the local energy set for manipulating values before pushing them into the model
        /// </summary>
        /// <param name="pairInteraction"></param>
        /// <returns></returns>
        protected HashSet<PairEnergyEntry> CreateEnergySet(PairInteraction pairInteraction)
        {
            return new HashSet<PairEnergyEntry>(pairInteraction.GetEnergyEntries());
        }
    }
}