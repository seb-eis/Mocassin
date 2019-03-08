using Microsoft.EntityFrameworkCore;

namespace Mocassin.Model.Translator
{
    /// <summary>
    ///     Represents a translator database context for interop operations with the unmanaged simulation program
    /// </summary>
    public interface IMocassinSimulationLibrary
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
        ///     The database set for all entities that support storing as a blob
        /// </summary>
        DbSet<BlobEntityBase> Blobs { get; }

        /// <summary>
        ///     Database set for sqlite load queries. Describes how the simulator pulls data from the database
        /// </summary>
        DbSet<SqliteQueryEntity> SqliteQueries { get; }

        /// <summary>
        ///     Save the changes to the interop db context
        /// </summary>
        /// <returns></returns>
        int SaveChanges();
    }
}