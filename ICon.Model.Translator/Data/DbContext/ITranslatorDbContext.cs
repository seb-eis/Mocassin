using Microsoft.EntityFrameworkCore;

namespace ICon.Model.Translator
{
    /// <summary>
    /// Represents a translator database context for interop operations with the unmanaged simulation
    /// </summary>
    public interface ITranslatorDbContext
    {
        /// <summary>
        /// Database set for translated simulation packages
        /// </summary>
        DbSet<SimulationPackage> SimulationPackages { get; set; }

        /// <summary>
        /// Database set for translated structure models
        /// </summary>
        DbSet<StructureModel> StructureModels { get; set; }

        /// <summary>
        /// Database set for translated transition models
        /// </summary>
        DbSet<TransitionModel> TransitionModels { get; set; }

        /// <summary>
        /// Database set for translated energy models
        /// </summary>
        DbSet<EnergyModel> EnergyModels { get; set; }

        /// <summary>
        /// Database set for translated job models
        /// </summary>
        DbSet<JobModel> JobModels { get; set; }

        /// <summary>
        /// Database set for translated lattice models
        /// </summary>
        DbSet<LatticeModel> LatticeModels { get; set; }

        /// <summary>
        /// Database set for all environment definitions
        /// </summary>
        DbSet<EnvironmentDefinitionEntity> EnvironmentDefinitions { get; set; }

        /// <summary>
        /// Database set for all pair energy tables
        /// </summary>
        DbSet<PairEnergyTableEntity> PairEnergyTables { get; set; }

        /// <summary>
        /// Database set for all cluster energy tables
        /// </summary>
        DbSet<ClusterEnergyTableEntity> ClusterEnergyTables { get; set; }

        /// <summary>
        /// Database set for all jump collections
        /// </summary>
        DbSet<JumpCollectionEntity> JumpCollections { get; set; }

        /// <summary>
        /// Database set for alljump directions
        /// </summary>
        DbSet<JumpDirectionEntity> JumpDirections { get; set; }

        /// <summary>
        /// The database set for all entities that support storing as a blob
        /// </summary>
        DbSet<BlobEntityBase> Blobs { get; set; }

        /// <summary>
        /// Database set for sqlite load queries. Describes how the simulator pulls data from the database
        /// </summary>
        DbSet<SqliteQueryEntity> SqliteQueries { get; set; }

        /// <summary>
        /// Save the changes to the interop db context
        /// </summary>
        /// <returns></returns>
        int SaveChanges();
    }
}