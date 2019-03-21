using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;
using Mocassin.Model.Basic;
using Mocassin.Model.Transitions;
using Newtonsoft.Json;

namespace Mocassin.UI.Xml.Base
{
    /// <summary>
    ///     Generic serializable class to store and provide key based references to specific <see cref="ModelObject"/> instances
    /// </summary>
    [XmlRoot]
    public class ModelObjectReferenceGraph<T> : ModelObjectGraph where T : ModelObject, new()
    {
        /// <inheritdoc />
        [XmlAttribute("DisplayName")]
        [JsonProperty("DisplayName")]
        [NotMapped]
        public override string DisplayName { get => base.DisplayName ?? $"@{Key}"; set => base.DisplayName = value; }

        /// <inheritdoc />
        protected override ModelObject GetModelObjectInternal()
        {
            return new T {Key = Key};
        }
    }
}