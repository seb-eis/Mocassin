using System;
using System.Linq;
using Mocassin.Model.Mml.Exceptions;

namespace Mocassin.Model.Mml
{
    /// <summary>
    ///     Attribute to mark a class as an export for the Mml adapter container system
    /// </summary>
    public class ExportExtractorAttribute : MmlExportAttribute
    {
        /// <summary>
        ///     The required interface type for a specified export
        /// </summary>
        private static readonly Type RequiredInterfaceType = typeof(IDataExtractor<,>);

        /// <inheritdoc />
        public ExportExtractorAttribute()
        {
        }

        /// <inheritdoc />
        public ExportExtractorAttribute(Type exportType)
            : base(exportType)
        {
            if (exportType.GetInterfaces().All(x => x.GetGenericTypeDefinition() != RequiredInterfaceType))
                throw new InvalidExportException($"Type {exportType} does not implement required interface {RequiredInterfaceType}");
        }
    }
}