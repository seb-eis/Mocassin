using System.Xml.Serialization;
using Mocassin.Model.ModelProject;
using Mocassin.UI.Xml.Base;
using Mocassin.UI.Xml.Main;

namespace Mocassin.UI.Xml.Customization
{
    /// <summary>
    ///     Abstract base class for all serializable data objects that enable customization of <see cref="IModelProject" />
    ///     auto generated content
    /// </summary>
    [XmlRoot]
    public abstract class ModelCustomizationEntity : MocassinProjectChildEntity<MocassinProjectGraph>
    {
        /// <summary>
        ///     Pushes all set data on the customization to the passed <see cref="IModelProject" />
        /// </summary>
        /// <param name="modelProject"></param>
        public abstract void PushToModel(IModelProject modelProject);
    }
}