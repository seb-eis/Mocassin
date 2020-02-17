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
        ///     The data accessor provider used to lock the data object by the setter adapters
        /// </summary>
        protected IDataAccessorSource<EnergyModelData> DataAccessorSource { get; }

        /// <summary>
        ///     Get or set the <see cref="IEnergyQueryPort" /> to invalidate cached data
        /// </summary>
        protected IEnergyQueryPort EnergyQueryPort { get; }

        /// <inheritdoc />
        public IValueConstraint<double, double> PairEnergyConstraint { get; set; }

        /// <inheritdoc />
        public IValueConstraint<double, double> GroupEnergyConstraint { get; set; }

        /// <summary>
        ///     Creates new energy setter provider that uses the provided <see cref="IDataAccessorSource{TData}" /> for data
        ///     manipulation and <see cref="IEnergyQueryPort" /> for cache invalidation
        /// </summary>
        /// <param name="dataAccessorSource"></param>
        /// <param name="energyQueryPort"></param>
        public EnergySetterProvider(IDataAccessorSource<EnergyModelData> dataAccessorSource, IEnergyQueryPort energyQueryPort)
        {
            EnergyQueryPort = energyQueryPort ?? throw new ArgumentNullException(nameof(energyQueryPort));
            DataAccessorSource = dataAccessorSource ?? throw new ArgumentNullException(nameof(dataAccessorSource));
        }

        /// <inheritdoc />
        public IReadOnlyList<IPairEnergySetter> GetStablePairEnergySetters()
        {
            using var accessor = DataAccessorSource.CreateUnsafe();
            return accessor.Query(x => x.StablePairInteractions.Select(y => GetStablePairEnergySetter(y.Index)).ToList());
        }

        /// <inheritdoc />
        public IPairEnergySetter GetStablePairEnergySetter(int index)
        {
            using var accessor = DataAccessorSource.CreateUnsafe();
            return new PairEnergySetter(accessor.Query(x => x.StablePairInteractions[index]), EnergyQueryPort)
            {
                DataAccessorSource = DataAccessorSource,
                EnergyConstraint = PairEnergyConstraint
            };
        }

        /// <inheritdoc />
        public IReadOnlyList<IPairEnergySetter> GetUnstablePairEnergySetters()
        {
            using var accessor = DataAccessorSource.CreateUnsafe();
            return accessor.Query(x => x.UnstablePairInteractions.Select(y => GetUnstablePairEnergySetter(y.Index)).ToList());
        }

        /// <inheritdoc />
        public IPairEnergySetter GetUnstablePairEnergySetter(int index)
        {
            using var accessor = DataAccessorSource.CreateUnsafe();
            return new PairEnergySetter(accessor.Query(x => x.UnstablePairInteractions[index]), EnergyQueryPort)
            {
                DataAccessorSource = DataAccessorSource,
                EnergyConstraint = PairEnergyConstraint
            };
        }

        /// <inheritdoc />
        public IReadOnlyList<IGroupEnergySetter> GetGroupEnergySetters()
        {
            using var accessor = DataAccessorSource.CreateUnsafe();
            return accessor.Query(x => x.GroupInteractions.Select(y => GetGroupEnergySetter(y.Index)).ToList());
        }

        /// <inheritdoc />
        public IGroupEnergySetter GetGroupEnergySetter(int index)
        {
            using var accessor = DataAccessorSource.CreateUnsafe();
            var groupInteraction = accessor.Query(x => x.GroupInteractions[index]);
            if (groupInteraction.IsDeprecated)
                return GroupEnergySetter.CreateEmpty();

            return new GroupEnergySetter(groupInteraction, EnergyQueryPort)
            {
                DataAccessorSource = DataAccessorSource,
                EnergyConstraint = GroupEnergyConstraint
            };
        }
    }
}