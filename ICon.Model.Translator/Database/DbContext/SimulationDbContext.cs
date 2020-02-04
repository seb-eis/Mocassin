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
            var connection = Database.GetDbConnection();
            try
            {
                connection.Open();
                using var command = connection.CreateCommand();
                command.CommandText = journalMode switch
                {
                    DbJournalMode.Delete => "pragma journal_mode=delete",
                    DbJournalMode.Truncate => "pragma journal_mode=truncate",
                    DbJournalMode.Persist => "pragma journal_mode=persist",
                    DbJournalMode.Memory => "pragma journal_mode=memory",
                    DbJournalMode.Wal => "pragma journal_mode=wal",
                    DbJournalMode.Off => "pragma journal_mode=off",
                    _ => throw new ArgumentOutOfRangeException(nameof(journalMode), journalMode, null)
                };
                command.ExecuteNonQuery();
            }
            finally
            {
                connection.Close();
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