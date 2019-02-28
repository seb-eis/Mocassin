using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;
using Mocassin.Model.Basic;

namespace Mocassin.UI.Xml.Base
{
    /// <summary>
    ///     Base class for all serializable data objects that supply <see cref="ModelParameter" /> conversion for data input
    /// </summary>
    [XmlRoot]
    public abstract class XmlModelParameter : XmlEntity
    {
        /// <summary>
        ///     Get the input parameter object for the automated data input system of the model management
        /// </summary>
        /// <returns></returns>
        public ModelParameter GetInputObject()
        {
            var obj = GetPreparedModelObject();
            return obj;
        }

        /// <summary>
        ///     Get a prepared parameter input object with all specific input data set
        /// </summary>
        /// <returns></returns>
        protected abstract ModelParameter GetPreparedModelObject();
    }
}