using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ICon.Model.Translator
{
    /// <summary>
    /// Base class for all monte carlo simulation components
    /// </summary>
    public abstract class McsComponent : EntityBase
    {
        /// <summary>
        /// The mccs parent context key
        /// </summary>
        [ForeignKey(nameof(MccsParent))]
        public int MccsParentId { get; set; }

        /// <summary>
        /// The mccs parent of the component
        /// </summary>
        public McsParent MccsParent { get; set; }
    }
}
