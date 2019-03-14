using System.Xml.Serialization;
using Mocassin.Model.Basic;
using Mocassin.Model.Transitions;

namespace Mocassin.UI.Xml.Base
{
    /// <summary>
    ///     Generic serializable class to store and provide key based references to specific <see cref="ModelObject"/> instances
    /// </summary>
    [XmlRoot]
    public class ModelObjectReferenceGraph<T> : ModelObjectGraph where T : ModelObject, new()
    {
        /// <inheritdoc />
        protected override ModelObject GetModelObjectInternal()
        {
            return new T {Key = Key};
        }
    }
}