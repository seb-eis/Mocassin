using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Model.Basic
{
    /// <summary>
    /// Defines the type of linking action performed. Use 'Content' linking if only the content of the object or list of objects requires linking and not the objects itself
    /// </summary>
    public enum LinkableType : int
    {
        Value, Content
    }

    /// <summary>
    /// Marks a property (Single object or IList of objects) as a data reference that should be index resolved into the known refernce by the model data tracker
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class LinkableByIndexAttribute : Attribute
    {
        /// <summary>
        /// Flags that marks the performed level of resolving (Default: Property itself is resolvable or implements non-generic IList with resolvables)
        /// </summary>
        public LinkableType LinkableType { get; set; }
    }
}
