using System;
using Microsoft.EntityFrameworkCore;
using Mocassin.Framework.SQLiteCore;
using Mocassin.Model.Translator.Database.Entities.Other.Meta;

namespace Mocassin.Model.Translator
{
    /// <inheritdoc cref="ISimulationLibrary" />
    public sealed class SimulationDbContext : SqLiteContext, ISimulationLibrary
    {
        /// <inheritdoc />
        public DbSet<SimulationJobPackageModel> SimulationPackages { get; set; }

        /// <inheritdoc />
        public DbSet<SimulationStructureModel> StructureModels { get; set; }

        /// <inheritdoc />
        public DbSet<SimulationTransitionModel> TransitionModels { get; set; }

        /// <inheritdoc />
        public DbSet<SimulationEnergyModel> EnergyModels { get; set; }

        /// <inheritdoc />
        public DbSet<SimulationJobModel> JobModels { get; set; }

        /// <inheritdoc />
        public DbSet<SimulationLatticeModel> LatticeModels { get; set; }

        /// <inheritdoc />
        public DbSet<EnvironmentDefinitionEntity> EnvironmentDefinitions { get; set; }

        /// <inheritdoc />
        public DbSet<PairEnergyTableEntity> PairEnergyTables { get; set; }

        /// <inheritdoc />
        public DbSet<ClusterEnergyTableEntity> ClusterEnergyTables { get; set; }

        /// <inheritdoc />
        public DbSet<JumpCollectionEntity> JumpCollections { get; set; }

        /// <inheritdoc />
        public DbSet<JumpDirectionEntity> JumpDirections { get; set; }

        /// <inheritdoc />
        public DbSet<JobMetaDataEntity> JobMetaData { get; set; }

        /// <inheritdoc />
        public DbSet<JobResultDataEntity> JobResultData { get; set; }

        /// <inheritdoc />
        public SimulationDbContext(string optionsBuilderParameterString)
            : base(optionsBuilderParameterString)
        {
        }

        /// <inheritdoc cref="ISimulationLibrary.SaveChanges" />
        public override int SaveChanges()
        {
            EnsureInteropObjectsAreInBinaryState();
            return base.SaveChanges();
        }

        /// <inheritdoc />
        public void SetJournalMode(DbJournalMode journalMode)
        {
            switch (journalMode)
            {
                case DbJournalMode.Delete:
                    ExecuteCommand("pragma journal_mode=delete");
                    break;
                case DbJournalMode.Truncate:
                    ExecuteCommand("pragma journal_mode=truncate");
                    break;
                case DbJournalMode.Persist:
                    ExecuteCommand("pragma journal_mode=persist");
                    break;
                case DbJournalMode.Memory:
                    ExecuteCommand("pragma journal_mode=memory");
                    break;
                case DbJournalMode.Wal:
                    ExecuteCommand("pragma journal_mode=wal");
                    break;
                case DbJournalMode.Off:
                    ExecuteCommand("pragma journal_mode=off");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(journalMode), journalMode, null);
            }
        }

        /// <inheritdoc />
        public void EnsureInteropObjectsAreInObjectState()
        {
            using var marshalService = new MarshalService();
            MutateTrackedInteropEntities(interopEntity => interopEntity.ChangePropertyStatesToObjects(marshalService));
        }

        /// <inheritdoc />
        public void EnsureInteropObjectsAreInBinaryState()
        {
            using var marshalService = new MarshalService();
            MutateTrackedInteropEntities(interopEntity => interopEntity.ChangePropertyStatesToBinaries(marshalService));
        }

        /// <summary>
        ///     Performs the passed action on all tracked entities of type <see cref="InteropEntityBase" />
        /// </summary>
        /// <param name="action"></param>
        private void MutateTrackedInteropEntities(Action<InteropEntityBase> action)
        {
            var entityEntries = ChangeTracker.Entries<InteropEntityBase>();
            foreach (var item in entityEntries) action(item.Entity);
        }
    }
}