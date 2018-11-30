using System;

namespace Mocassin.Model.Mml
{
    /// <summary>
    ///     Abstract base attribute for Mml import attribute implementations of classes. Marks a property for import of a
    ///     specific type from an mml container
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public abstract class MmlImportAttribute : Attribute
    {
        /// <summary>
        ///     The type the class that should be imported
        /// </summary>
        public Type ImportType { get; }

        /// <summary>
        ///     Create new Mml import attribute without a specific import type
        /// </summary>
        protected MmlImportAttribute()
        {
        }

        /// <summary>
        ///     Create new Mml import attribute with a specified import type
        /// </summary>
        /// <param name="importType"></param>
        protected MmlImportAttribute(Type importType)
        {
            ImportType = importType ?? throw new ArgumentNullException(nameof(importType));
        }
    }
}