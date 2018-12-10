using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mocassin.Model.Translator
{
    /// <summary>
    ///     Jump collection entity. Defines and stores a collection of affiliated jump directions for the simulation database
    /// </summary>
    public class JumpCollectionEntity : TransitionModelComponentBase
    {
        /// <summary>
        ///     The list of affiliated jump direction entities
        /// </summary>
        public List<JumpDirectionEntity> JumpDirections { get; set; }

        /// <summary>
        ///     The list of existing jump roles for the collection
        /// </summary>
        [NotMapped]
        [OwnedBlobProperty(nameof(JumpRuleListBinary))]
        public JumpRuleListEntity JumpRuleList { get; set; }

        /// <summary>
        ///     The object id in the simulation context
        /// </summary>
        [Column("ObjectId")]
        public int ObjectId { get; set; }

        /// <summary>
        ///     The particle mask that describes all possible selectable particles
        /// </summary>
        [Column("SelectableParticlesMask")]
        public long SelectableParticlesMask { get; set; }

        /// <summary>
        ///     Jump rule list blob conversion backing property
        /// </summary>
        [Column("JumpRules")]
        public byte[] JumpRuleListBinary { get; set; }
    }
}