using System;
using System.Linq;
using Mocassin.Model.Mml.Exceptions;

namespace Mocassin.Model.Mml
{
    /// <summary>
    ///     Attribute to mark a property as an imported adapter for the Mml adapter container system
    /// </summary>
    public class ImportAdapterAttribute : MmlImportAttribute
    {
        /// <summary>
        ///     The required interface type for a specified export
        /// </summary>
        private static readonly Type RequiredInterfaceType = typeof(IDataAdapter<,>);


        /// <inheritdoc />
        public ImportAdapterAttribute()
        {
        }

        /// <inheritdoc />
        public ImportAdapterAttribute(Type importType)
            : base(importType)
        {
            if (importType.GetInterfaces().All(x => x.GetGenericTypeDefinition() != RequiredInterfaceType))
                throw new InvalidImportException($"Type {importType} does not implement required interface {RequiredInterfaceType}");
        }
    }
}