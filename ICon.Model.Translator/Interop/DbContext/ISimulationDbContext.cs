using Microsoft.EntityFrameworkCore;

namespace Mocassin.Model.Translator
{
    /// <summary>
    ///     Represents a translator database context for interop operations with the unmanaged simulation program
    /// </summary>
    public interface ISimulationDbContext
    {
        /// <summary>
        ///     Database set for translated simulation packages
        /// </summary>
        DbSet<SimulationJobPackageModel> SimulationPackages { get; set; }

        /// <summary>
        ///     Database set for translated structure models
        /// </summary>
        DbSet<SimulationStructureModel> StructureModels { get; set; }

        /// <summary>
        ///     Database set for translated transition models
        /// </summary>
        DbSet<SimulationTransitionModel> TransitionModels { get; set; }

        /// <summary>
        ///     Database set for translated energy models
        /// </summary>
        DbSet<SimulationEnergyModel> EnergyModels { get; set; }

        /// <summary>
        ///     Database set for translated job models
        /// </summary>
        DbSet<SimulationJobModel> JobModels { get; set; }

        /// <summary>
        ///     Database set for translated lattice models
        /// </summary>
        DbSet<SimulationLatticeModel> LatticeModels { get; set; }

        /// <summary>
        ///     Database set for all environment definitions
        /// </summary>
        DbSet<EnvironmentDefinitionEntity> EnvironmentDefinitions { get; set; }

        /// <summary>
        ///     Database set for all pair energy tables
        /// </summary>
        DbSet<PairEnergyTableEntity> PairEnergyTables { get; set; }

        /// <summary>
        ///     Database set for all cluster energy tables
        /// </summary>
        DbSet<ClusterEnergyTableEntity> ClusterEnergyTables { get; set; }

        /// <summary>
        ///     Database set for all jump collections
        /// </summary>
        DbSet<JumpCollectionEntity> JumpCollections { get; set; }

        /// <summary>
        ///     Database set for all jump directions
        /// </summary>
        DbSet<JumpDirectionEntity> JumpDirections { get; set; }

        /// <summary>
        ///     The database set for all entities that support storing as a blob
        /// </summary>
        DbSet<BlobEntityBase> Blobs { get; set; }

        /// <summary>
        ///     Database set for sqlite load queries. Describes how the simulator pulls data from the database
        /// </summary>
        DbSet<SqliteQueryEntity> SqliteQueries { get; set; }

        /// <summary>
        ///     Save the changes to the interop db context
        /// </summary>
        /// <returns></returns>
        int SaveChanges();
    }
}