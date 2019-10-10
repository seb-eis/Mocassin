using System;
using Microsoft.EntityFrameworkCore;
using Mocassin.Model.Translator.Database.Entities.Other.Meta;

namespace Mocassin.Model.Translator
{
    /// <summary>
    ///     Provides an interface for accessing simulation libraries
    /// </summary>
    public interface ISimulationLibrary : IDisposable
    {
        /// <summary>
        ///     Database set for translated simulation packages
        /// </summary>
        DbSet<SimulationJobPackageModel> SimulationPackages { get; }

        /// <summary>
        ///     Database set for translated structure models
        /// </summary>
        DbSet<SimulationStructureModel> StructureModels { get; }

        /// <summary>
        ///     Database set for translated transition models
        /// </summary>
        DbSet<SimulationTransitionModel> TransitionModels { get; }

        /// <summary>
        ///     Database set for translated energy models
        /// </summary>
        DbSet<SimulationEnergyModel> EnergyModels { get; }

        /// <summary>
        ///     Database set for translated job models
        /// </summary>
        DbSet<SimulationJobModel> JobModels { get; }

        /// <summary>
        ///     Database set for translated lattice models
        /// </summary>
        DbSet<SimulationLatticeModel> LatticeModels { get; }

        /// <summary>
        ///     Database set for all environment definitions
        /// </summary>
        DbSet<EnvironmentDefinitionEntity> EnvironmentDefinitions { get; }

        /// <summary>
        ///     Database set for all pair energy tables
        /// </summary>
        DbSet<PairEnergyTableEntity> PairEnergyTables { get; }

        /// <summary>
        ///     Database set for all cluster energy tables
        /// </summary>
        DbSet<ClusterEnergyTableEntity> ClusterEnergyTables { get; }

        /// <summary>
        ///     Database set for all jump collections
        /// </summary>
        DbSet<JumpCollectionEntity> JumpCollections { get; }

        /// <summary>
        ///     Database set for all jump directions
        /// </summary>
        DbSet<JumpDirectionEntity> JumpDirections { get; }

        /// <summary>
        ///     Database set for all job meta information
        /// </summary>
        DbSet<JobMetaDataEntity> JobMetaData { get; }

        /// <summary>
        ///     Database set for all job result information
        /// </summary>
        DbSet<JobResultDataEntity> JobResultData { get; }

        /// <summary>
        ///     Save the changes to the interop db context
        /// </summary>
        /// <returns></returns>
        int SaveChanges();
    }
}