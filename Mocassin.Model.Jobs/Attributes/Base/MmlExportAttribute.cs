using System;

namespace Mocassin.Model.Mml
{
    /// <summary>
    ///     Abstract base attribute for Mml export attribute implementations of classes. Marks a class for export into
    ///     the mml container systems as a specific type
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public abstract class MmlExportAttribute : Attribute
    {
        /// <summary>
        ///     The type the class should be exported as
        /// </summary>
        public Type ExportType { get; }

        /// <summary>
        ///     Create new Mml export attribute without a specific export type
        /// </summary>
        protected MmlExportAttribute()
        {
        }

        /// <summary>
        ///     Create new Mml export attribute with a specified export type
        /// </summary>
        /// <param name="exportType"></param>
        protected MmlExportAttribute(Type exportType)
        {
            ExportType = exportType ?? throw new ArgumentNullException(nameof(exportType));
        }
    }
}