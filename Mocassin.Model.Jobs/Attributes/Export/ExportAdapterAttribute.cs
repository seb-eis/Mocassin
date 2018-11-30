using System;
using System.Linq;
using Mocassin.Model.Mml.Exceptions;

namespace Mocassin.Model.Mml
{
    /// <summary>
    ///     Attribute to mark a class as an adapter export for the Mml adapter container system
    /// </summary>
    public class ExportAdapterAttribute : MmlExportAttribute
    {
        /// <summary>
        ///     The required interface type for a specified export
        /// </summary>
        private static readonly Type RequiredInterfaceType = typeof(IDataAdapter<,>);

        /// <inheritdoc />
        public ExportAdapterAttribute()
        {
        }

        /// <inheritdoc />
        public ExportAdapterAttribute(Type exportType)
            : base(exportType)
        {
            if (exportType.GetInterfaces().All(x => x.GetGenericTypeDefinition() != RequiredInterfaceType))
                throw new InvalidExportException($"Type {exportType} does not implement required interface {RequiredInterfaceType}");
        }
    }
}