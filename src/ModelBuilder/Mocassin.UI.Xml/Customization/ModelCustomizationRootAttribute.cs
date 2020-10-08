using System;

namespace Mocassin.UI.Data.Customization
{
    /// <summary>
    ///     Attribute to mark a property as a model customization root for
    ///     <see cref="Mocassin.Model.ModelProject.IModelProject" /> auto generated content manipulation
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ModelCustomizationRootAttribute : Attribute
    {
    }
}