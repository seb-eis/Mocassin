using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ICon.Model.Translator
{
    /// <summary>
    /// Base class for all MccsParent component entities
    /// </summary>
    public abstract class MccsComponentEntity : EntityBase
    {
        /// <summary>
        /// The mccs parent context key
        /// </summary>
        [ForeignKey(nameof(MccsParent))]
        public int ParentKey { get; set; }

        /// <summary>
        /// The mccs parent of the component
        /// </summary>
        public MccsParent MccsParent { get; set; }
    }
}
