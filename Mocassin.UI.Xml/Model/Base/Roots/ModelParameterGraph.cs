using System.Xml.Serialization;
using Mocassin.Model.Basic;

namespace Mocassin.UI.Xml.Base
{
    /// <summary>
    ///     Base class for all serializable data objects that supply <see cref="ModelParameter" /> conversion for data input
    /// </summary>
    [XmlRoot]
    public abstract class ModelParameterGraph : ExtensibleProjectObjectGraph
    {
        /// <summary>
        ///     Get the <see cref="ModelParameter" /> object for the automated data input system of the model management
        /// </summary>
        /// <returns></returns>
        public ModelParameter GetInputObject()
        {
            var obj = GetModelObjectInternal();
            return obj;
        }

        /// <summary>
        ///     Get a prepared <see cref="ModelParameter" /> object with all specific input data set
        /// </summary>
        /// <returns></returns>
        protected abstract ModelParameter GetModelObjectInternal();
    }
}