using System;

namespace Mocassin.Model.Basic
{
    /// <summary>
    ///     Defines the type of linking action performed. Use 'Content' reference level if only the content of the object or
    ///     list of objects requires linking and not the objects itself
    /// </summary>
    public enum ReferenceLevel
    {
        Value,
        Content
    }

    /// <summary>
    ///     Marks a property (Single object or IList of objects) as a data reference that should be index/key resolved into
    ///     the known reference by the model data tracker
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class UseTrackedDataAttribute : Attribute
    {
        /// <summary>
        ///     Flags that marks the performed level of resolving (Default: Property itself is resolvable or implements non-generic
        ///     IList)
        /// </summary>
        public ReferenceLevel ReferenceLevel { get; set; }
    }
}