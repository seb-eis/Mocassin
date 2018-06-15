using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Model.Basic
{
    /// <summary>
    /// Defines the possible levels of index to refernce resolving performed on the object
    /// </summary>
    public enum IndexResolverLevel : int
    {
        ObjectOrList, Content
    }

    /// <summary>
    /// Marks a property (Single object or IList of objects) as a data reference that should be index resolved into the known refernce by the model data tracker
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class IndexResolvableAttribute : Attribute
    {
        /// <summary>
        /// Flags that marks the performed level of resolving (Default: Property itself is resolvable or implements non-generic IList with resolvables)
        /// </summary>
        public IndexResolverLevel Level { get; set; }
    }
}
