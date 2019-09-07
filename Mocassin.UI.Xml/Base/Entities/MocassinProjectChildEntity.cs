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
    public abstract class MocassinProjectChildEntity<TParent> : ProjectObjectGraph where TParent : class
    {
        private TParent parent;

        /// <summary>
        ///     Get or set the parent navigation property
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        [NotMapped]
        public TParent Parent
        {
            get => parent;
            set => SetProperty(ref parent, value);
        }

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