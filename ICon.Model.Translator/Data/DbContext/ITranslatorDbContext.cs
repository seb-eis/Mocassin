using Microsoft.EntityFrameworkCore;

namespace ICon.Model.Translator
{
    /// <summary>
    /// Represents a translator database context for interop operations with the unmanaged simulation
    /// </summary>
    public interface ITranslatorDbContext
    {
        DbSet<BlobEntityBase> Blobs { get; set; }

        DbSet<ClusterEnergyTableEntity> ClusterEnergyTables { get; set; }

        DbSet<EnergyModel> EnergyModels { get; set; }

        DbSet<EnvironmentDefinitionEntity> EnvironmentDefinitions { get; set; }

        DbSet<JobModel> JobModels { get; set; }

        DbSet<JumpCollectionEntity> JumpCollections { get; set; }

        DbSet<JumpDirectionEntity> JumpDirections { get; set; }

        DbSet<LatticeModel> LatticeModels { get; set; }

        DbSet<PairEnergyTableEntity> PairEnergyTables { get; set; }

        DbSet<SimulationPackage> SimulationPackages { get; set; }

        DbSet<StructureModel> StructureModels { get; set; }

        DbSet<TransitionModel> TransitionModels { get; set; }

        DbSet<SqliteQueryEntity> SqliteQueries { get; set; }

        int SaveChanges();
    }
}