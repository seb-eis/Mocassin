using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Mocassin.Framework.SQLiteCore;
using Mocassin.Model.Translator.Database.Entities.Other.Meta;

namespace Mocassin.Model.Translator
{
    /// <inheritdoc cref="ISimulationLibrary" />
    public sealed class SimulationDbContext : SqLiteContext<SimulationDbContext>, ISimulationLibrary
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
            using var marshalService = new MarshalService();
            PerformActionOnInteropEntities(a => a.ChangePropertyStatesToBinaries(marshalService));
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

        /// <summary>
        ///     Performs the passed action on all properties of database set entries that are interop entities
        /// </summary>
        /// <param name="action"></param>
        private void PerformActionOnInteropEntities(Action<InteropEntityBase> action)
        {
            foreach (var propertyInfo in GetDbSetPropertyInfos())
            {
                if (!typeof(InteropEntityBase).IsAssignableFrom(propertyInfo.PropertyType.GetGenericArguments()[0])) continue;

                var dbSet = propertyInfo.GetValue(this);
                var local = dbSet?.GetType().GetProperty("Local")?.GetValue(dbSet)
                            ?? throw new InvalidOperationException("Could not get local view of interop entity database set");

                foreach (var item in (IEnumerable<InteropEntityBase>) local) action(item);
            }
        }

        /// <summary>
        ///     Gets all database sets property info of the context by reflection
        /// </summary>
        /// <returns></returns>
        private IEnumerable<PropertyInfo> GetDbSetPropertyInfos()
        {
            return GetType().GetProperties()
                .Where(a => a.PropertyType.IsGenericType)
                .Where(item => item.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>));
        }
    }
}