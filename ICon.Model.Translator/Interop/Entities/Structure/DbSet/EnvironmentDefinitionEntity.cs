using System.ComponentModel.DataAnnotations.Schema;

namespace Mocassin.Model.Translator
{
    /// <summary>
    ///     Simulation environment definition. Defines and stores a single position environment for the simulation database
    /// </summary>
    public class EnvironmentDefinitionEntity : StructureModelComponentBase
    {
        /// <summary>
        ///     The object id in the simulation context
        /// </summary>
        [Column("ObjectId")]
        public int ObjectId { get; set; }

        /// <summary>
        ///     Position particle ids blob conversion backing property
        /// </summary>
        [Column("PositionParticleIds")]
        public byte[] PositionParticleIdsBinary { get; set; }

        /// <summary>
        ///     Update particle ids blob conversion backing property
        /// </summary>
        [Column("UpdateParticleIds")]
        public byte[] UpdateParticleIdsBinary { get; set; }

        /// <summary>
        ///     Pair definition list blob conversion backing property
        /// </summary>
        [Column("PairDefinitions")]
        public byte[] PairDefinitionListBinary { get; set; }

        /// <summary>
        ///     Cluster definition list blo conversion backing property
        /// </summary>
        [Column("ClusterDefinitions")]
        public byte[] ClusterDefinitionListBinary { get; set; }

        /// <summary>
        ///     Fixed 64 byte buffer for possible position particle ids
        /// </summary>
        [NotMapped]
        [InteropProperty(nameof(PositionParticleIdsBinary))]
        public ByteBuffer64 PositionParticleIds { get; set; }

        /// <summary>
        ///     Fixed 64 byte buffer for particle ids that require delta update processes
        /// </summary>
        [NotMapped]
        [InteropProperty(nameof(UpdateParticleIdsBinary))]
        public ByteBuffer64 UpdateParticleIds { get; set; }

        /// <summary>
        ///     The list of pair definition entities affiliated with the environment
        /// </summary>
        [NotMapped]
        [OwnedBlobProperty(nameof(PairDefinitionListBinary))]
        public PairDefinitionListEntity PairDefinitionList { get; set; }

        /// <summary>
        ///     The list of cluster definition entities affiliated with the environment
        /// </summary>
        [NotMapped]
        [OwnedBlobProperty(nameof(ClusterDefinitionListBinary))]
        public ClusterDefinitionListEntity GetClusterDefinitionList { get; set; }
    }
}