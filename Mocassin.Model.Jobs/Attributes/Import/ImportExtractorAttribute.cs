using System;
using System.Linq;
using Mocassin.Model.Mml.Exceptions;

namespace Mocassin.Model.Mml
{
    /// <summary>
    ///     Attribute to mark a property as an imported extractor for the Mml adapter container system
    /// </summary>
    public class ImportExtractorAttribute : MmlImportAttribute
    {
        /// <summary>
        ///     The required interface type for a specified export
        /// </summary>
        private static readonly Type RequiredInterfaceType = typeof(IDataExtractor<,>);


        /// <inheritdoc />
        public ImportExtractorAttribute()
        {
        }

        /// <inheritdoc />
        public ImportExtractorAttribute(Type importType)
            : base(importType)
        {
            if (importType.GetInterfaces().All(x => x.GetGenericTypeDefinition() != RequiredInterfaceType))
                throw new InvalidImportException($"Type {importType} does not implement required interface {RequiredInterfaceType}");
        }
    }
}