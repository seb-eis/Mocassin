using System;

namespace Mocassin.Model.Basic
{
    /// <summary>
    ///     Defines the level of reference correction for properties marked with <see cref="UseTrackedDataAttribute" />
    /// </summary>
    public enum ReferenceCorrectionLevel
    {
        /// <summary>
        ///     Defines a full correction where also the property or list content itself is replaced if required
        /// </summary>
        Full,

        /// <summary>
        ///     Defines a content only correction where the property or list content itself is not replaced, only the properties on
        ///     these instances
        /// </summary>
        IgnoreTopLevel
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
        public ReferenceCorrectionLevel ReferenceCorrectionLevel { get; set; }
    }
}