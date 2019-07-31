using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace Mocassin.UI.Xml.Base
{
    /// <summary>
    ///     Generic base for <see cref="MocassinProjectEntity" /> that have a parent entity and are stored by their json data
    /// </summary>
    /// <typeparam name="TParent"></typeparam>
    [XmlRoot]
    public abstract class MocassinProjectChildEntity<TParent> : MocassinProjectEntity where TParent : class
    {
        /// <summary>
        ///     Get or set the context id of the parent entity
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        public int ParentId { get; set; }

        /// <summary>
        ///     Get or set the parent navigation property
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        [ForeignKey(nameof(ParentId))]
        public TParent Parent { get; set; }

        /// <summary>
        ///     Get or set the contents of the object by a json string representation
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        [NotMapped]
        public virtual string Json
        {
            get => ToJson();
            set => FromJson(value);
        }
    }
}