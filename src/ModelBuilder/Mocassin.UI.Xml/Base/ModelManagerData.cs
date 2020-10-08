using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Mocassin.Model.Basic;

namespace Mocassin.UI.Data.Base
{
    /// <summary>
    ///     Base class for all serializable data objects that supply the data required for
    ///     <see cref="Mocassin.Model.Basic.IModelManager" /> input pipelines
    /// </summary>
    [XmlRoot]
    public abstract class ModelManagerData : PropertyChangeNotifier
    {
        /// <summary>
        ///     Get the sequence of model input parameters defined in the data root
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerable<IModelParameter> GetInputParameters();

        /// <summary>
        ///     Get the sequence of model input objects defined in the data root
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerable<IModelObject> GetInputObjects();

        /// <summary>
        ///     Get the complete sequence of model parameters and model objects
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<object> GetInputSequence() => GetInputParameters().Cast<object>().Concat(GetInputObjects());
    }
}