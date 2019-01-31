using System.Xml.Serialization;
using Mocassin.Model.ModelProject;

namespace Mocassin.UI.Xml.CustomizationData
{
    /// <summary>
    ///     Abstract base class for all serializable data objects that enable customization of <see cref="IModelProject" />
    ///     auto generated content
    /// </summary>
    [XmlRoot]
    public abstract class XmlModelCustomizationData
    {
        /// <summary>
        ///     Pushes all set data on the customization to the passed <see cref="IModelProject" />
        /// </summary>
        /// <param name="modelProject"></param>
        public abstract void PushToModel(IModelProject modelProject);
    }
}