using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mocassin.Model.Translator
{
    /// <summary>
    ///     Simulation environment definition. Defines and stores a single position environment for the simulation database
    /// </summary>
    public class EnvironmentDefinitionEntity : StructureModelComponentBase
    {
        /// <summary>
        ///     Static backing field for the structure model component state change actions
        /// </summary>
        private static IList<StateChangeAction> _stateChangeDelegates;

        /// <inheritdoc />
        protected override IList<StateChangeAction> StateChangeActions
        {
            get => _stateChangeDelegates;
            set => _stateChangeDelegates = value;
        }

        /// <summary>
        ///     The object id in the simulation context
        /// </summary>
        [Column("ObjectId")]
        public int ObjectId { get; set; }

        /// <summary>
        ///     The particle selection mask that describes which particles are part of the selection pool for this environment
        ///     center
        /// </summary>
        [Column("SelectionMask")]
        public long SelectionParticleMask { get; set; }

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
        [NotMapped, InteropProperty(nameof(PositionParticleIdsBinary))]
        public InteropObject<CByteBuffer64> PositionParticleIds { get; set; }

        /// <summary>
        ///     Fixed 64 byte buffer for particle ids that require delta update processes
        /// </summary>
        [NotMapped, InteropProperty(nameof(UpdateParticleIdsBinary))]
        public InteropObject<CByteBuffer64> UpdateParticleIds { get; set; }

        /// <summary>
        ///     The list of pair definition entities affiliated with the environment
        /// </summary>
        [NotMapped, OwnedBlobProperty(nameof(PairDefinitionListBinary))]
        public PairDefinitionListEntity PairDefinitionList { get; set; }

        /// <summary>
        ///     The list of cluster definition entities affiliated with the environment
        /// </summary>
        [NotMapped, OwnedBlobProperty(nameof(ClusterDefinitionListBinary))]
        public ClusterDefinitionListEntity ClusterDefinitionList { get; set; }
    }
}