using System.ComponentModel.DataAnnotations.Schema;

namespace Mocassin.Model.Translator
{
    /// <summary>
    ///     Cluster energy table entity that carries the cluster simulation energy information in te interop database
    /// </summary>
    public class ClusterEnergyTableEntity : EnergyModelComponentBase
    {
        /// <summary>
        ///     Fixed 64 bytes buffer for the redirection of particle ids to table ids
        /// </summary>
        [NotMapped]
        [InteropProperty(nameof(ParticleToTableIdsBinary))]
        public InteropObject<CByteBuffer64> ParticleToTableIds { get; set; }

        /// <summary>
        ///     The 2D energy table entity that stores the energy values mapped by table id and cluster code id
        /// </summary>
        [NotMapped]
        [OwnedBlobProperty(nameof(EnergyTableBinary))]
        public EnergyTableEntity EnergyTable { get; set; }

        /// <summary>
        ///     The occupation code list that is used to lookup cluster code ids
        /// </summary>
        [NotMapped]
        [OwnedBlobProperty(nameof(OccupationCodeListBinary))]
        public OccupationCodeListEntity OccupationCodeList { get; set; }

        /// <summary>
        ///     The object id in the simulation context
        /// </summary>
        [Column("ObjectId")]
        public int ObjectId { get; set; }

        /// <summary>
        ///     Energy table blob conversion property
        /// </summary>
        [Column("EnergyTable")]
        public byte[] EnergyTableBinary { get; set; }

        /// <summary>
        ///     Occupation code list blob conversion property
        /// </summary>
        [Column("OccupationCodes")]
        public byte[] OccupationCodeListBinary { get; set; }

        /// <summary>
        ///     Particle to table id blob conversion property
        /// </summary>
        [Column("TableIndexing")]
        public byte[] ParticleToTableIdsBinary { get; set; }
    }
}