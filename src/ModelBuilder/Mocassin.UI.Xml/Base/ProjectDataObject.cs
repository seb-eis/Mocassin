using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Text;
using System.Xml.Serialization;
using Mocassin.Framework.Xml;
using Newtonsoft.Json;

namespace Mocassin.UI.Data.Base
{
    /// <summary>
    ///     Base class for all JSON/Xml serializable project objects with a INotifyPropertyChanged implementation for the
    ///     WPF binding infrastructure
    /// </summary>
    [XmlRoot]
    public abstract class ProjectDataObject : PropertyChangeNotifier
    {
        private string name;

        /// <summary>
        ///     Specifies the default serialization encoding
        /// </summary>
        [JsonIgnore]
        public static Encoding DefaultEncoding => Encoding.UTF8;

        /// <summary>
        ///     Specifies the default culture info for serialization
        /// </summary>
        [JsonIgnore]
        public static CultureInfo DefaultCultureInfo => CultureInfo.InvariantCulture;

        /// <summary>
        ///     Get or set a display name for the object graph
        /// </summary>
        [XmlAttribute("Name"), JsonProperty("Name"), Column("Name")]
        public virtual string Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }

        /// <summary>
        ///     Converts the <see cref="ProjectDataObject" /> to ist xml representation
        /// </summary>
        /// <param name="xmlEventHandlers"></param>
        /// <returns></returns>
        public string ToXml(XmlEventHandlers xmlEventHandlers = null) =>
            XmlSerializationHelper.Serialize(this, DefaultEncoding, xmlEventHandlers ?? GetDefaultXmlEventHandlers());

        /// <summary>
        ///     Creates a <see cref="ProjectDataObject" /> from an xml representation
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xml"></param>
        /// <param name="xmlEventHandlers"></param>
        /// <returns></returns>
        public static T CreateFromXml<T>(string xml, XmlEventHandlers xmlEventHandlers = null) where T : ProjectDataObject
        {
            if (xml == null) throw new ArgumentNullException(nameof(xml));
            var obj = (T) XmlSerializationHelper.Deserialize(xml, typeof(T), xmlEventHandlers);
            return obj;
        }

        /// <summary>
        ///     Converts the <see cref="ProjectDataObject" /> to a json representation
        /// </summary>
        /// <param name="serializerSettings"></param>
        /// <returns></returns>
        public string ToJson(JsonSerializerSettings serializerSettings = null) =>
            JsonConvert.SerializeObject(this, serializerSettings ?? GetDefaultJsonSerializerSettings());

        /// <summary>
        ///     Populates the <see cref="ProjectDataObject" /> from its json representation
        /// </summary>
        /// <param name="json"></param>
        /// <param name="serializerSettings"></param>
        public virtual void FromJson(string json, JsonSerializerSettings serializerSettings = null)
        {
            if (json == null) throw new ArgumentNullException(nameof(json));
            JsonConvert.PopulateObject(json, this, serializerSettings ?? GetDefaultJsonSerializerSettings());
        }

        /// <summary>
        ///     Populates the <see cref="ProjectDataObject" /> from its json representation
        /// </summary>
        /// <param name="json"></param>
        /// <param name="serializerSettings"></param>
        public static T CreateFromJson<T>(string json, JsonSerializerSettings serializerSettings = null) where T : ProjectDataObject, new()
        {
            if (json == null) throw new ArgumentNullException(nameof(json));
            var obj = new T();
            obj.FromJson(json, serializerSettings);
            return obj;
        }

        /// <summary>
        ///     Creates a deep copy of the <see cref="ProjectDataObject" /> using its json representation. By default the system
        ///     preserves object references
        /// </summary>
        /// <param name="referencesHandling"></param>
        /// <returns></returns>
        public virtual ProjectDataObject DeepCopy(PreserveReferencesHandling referencesHandling = PreserveReferencesHandling.All)
        {
            var settings = new JsonSerializerSettings
            {
                PreserveReferencesHandling = referencesHandling,
                TypeNameHandling = TypeNameHandling.Auto
            };
            return (ProjectDataObject) JsonConvert.DeserializeObject(ToJson(), GetType(), settings);
        }

        /// <summary>
        ///     Get the default <see cref="XmlEventHandlers" /> for xml serialization
        /// </summary>
        /// <returns></returns>
        protected virtual XmlEventHandlers GetDefaultXmlEventHandlers() => null;

        /// <summary>
        ///     Get the default <see cref="JsonSerializerSettings" /> for json serialization
        /// </summary>
        /// <returns></returns>
        protected virtual JsonSerializerSettings GetDefaultJsonSerializerSettings() =>
            new JsonSerializerSettings
            {
                PreserveReferencesHandling = PreserveReferencesHandling.All,
                TypeNameHandling = TypeNameHandling.Auto,
                Culture = DefaultCultureInfo
            };

        /// <inheritdoc />
        public override string ToString() => Name ?? base.ToString();
    }
}