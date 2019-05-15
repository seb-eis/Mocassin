using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Mocassin.Framework.SQLiteCore;
using Mocassin.Model.Translator.Database.Entities.Other.Meta;

namespace Mocassin.Model.Translator
{
    /// <inheritdoc cref="IMocassinSimulationLibrary" />
    public sealed class SimulationLibraryContext : SqLiteContext<SimulationLibraryContext>, IMocassinSimulationLibrary
    {
        /// <summary>
        ///     List of actions performed on model building
        /// </summary>
        private static IList<Action<ModelBuilder>> ModelBuildActions { get; set; }

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
        public SimulationLibraryContext(string optionsBuilderParameterString)
            : base(optionsBuilderParameterString)
        {
        }

        /// <inheritdoc cref="IMocassinSimulationLibrary.SaveChanges" />
        public override int SaveChanges()
        {
            base.SaveChanges();

            using (var marshalService = new MarshalService())
            {
                PerformActionOnInteropEntities(a => a.ChangePropertyStatesToBinaries(marshalService));
                return base.SaveChanges();
            }
        }

        /// <inheritdoc />
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await base.SaveChangesAsync(cancellationToken);
            using (var marshalService = new MarshalService())
            {
                await PerformActionOnInteropEntitiesAsync(a => a.ChangePropertyStatesToBinaries(marshalService), cancellationToken);
                return await base.SaveChangesAsync(cancellationToken);
            }
        }

        /// <summary>
        ///     Performs the passed action on all properties of database set entries that are interop entities
        /// </summary>
        /// <param name="action"></param>
        private void PerformActionOnInteropEntities(Action<InteropEntityBase> action)
        {
            foreach (var dbSetProperty in GetDbSetPropertyInfos())
            {
                if (!typeof(InteropEntityBase).IsAssignableFrom(dbSetProperty.PropertyType.GetGenericArguments()[0]))
                    continue;

                foreach (var item in (IEnumerable<InteropEntityBase>) dbSetProperty.GetValue(this))
                    action(item);
            }
        }

        /// <summary>
        ///     Performs the passed action on all properties of database set entries that are interop entities asynchronously
        /// </summary>
        /// <param name="action"></param>
        /// <param name="cancellationToken"></param>
        private Task PerformActionOnInteropEntitiesAsync(Action<InteropEntityBase> action, CancellationToken cancellationToken = default)
        {
            var operationTasks = new List<Task>();
            foreach (var dbSetProperty in GetDbSetPropertyInfos())
            {
                if (cancellationToken.IsCancellationRequested)
                    return Task.FromCanceled(cancellationToken);

                if (!typeof(InteropEntityBase).IsAssignableFrom(dbSetProperty.PropertyType.GetGenericArguments()[0]))
                    continue;

                operationTasks.AddRange(from item in (IEnumerable<InteropEntityBase>) dbSetProperty.GetValue(this)
                    select Task.Run(() => action(item), cancellationToken));
            }

            return Task.WhenAll(operationTasks);
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