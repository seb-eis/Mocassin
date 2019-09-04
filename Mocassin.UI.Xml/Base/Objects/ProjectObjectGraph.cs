using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml.Serialization;
using Mocassin.Framework.Xml;
using Newtonsoft.Json;

namespace Mocassin.UI.Xml.Base
{
    /// <summary>
    ///     Base class for all JSON/Xml serializable project objects with a dummy INotifyPropertyChanged implementation for the WPF binding infrastructure
    /// </summary>
    /// <remarks> INotifyPropertyChanged is implemented as a dummy to prevent WPF from using PropertyDescriptor type bindings, which causes memory leaks </remarks>
    [XmlRoot]
    public abstract class ProjectObjectGraph : INotifyPropertyChanged
    {
        /// <summary>
        ///     Property changed event (Dummy)
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///     Notify about a property change (Dummy)
        /// </summary>
        /// <param name="propertyName"></param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

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
        [XmlAttribute("Name")]
        [JsonProperty("Name")]
        [Column("Name")]
        public virtual string Name { get; set; }

        /// <summary>
        ///     Converts the <see cref="ProjectObjectGraph" /> to ist xml representation
        /// </summary>
        /// <param name="xmlEventHandlers"></param>
        /// <returns></returns>
        public string ToXml(XmlEventHandlers xmlEventHandlers = null)
        {
            return XmlStreamService.Serialize(this, DefaultEncoding, xmlEventHandlers ?? GetDefaultXmlEventHandlers());
        }

        /// <summary>
        ///     Populates the <see cref="ProjectObjectGraph" /> from its xml representation
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="xmlEventHandlers"></param>
        /// <remarks> Warning currently requires two serialization steps and is slow! </remarks>
        public void FromXml(string xml, XmlEventHandlers xmlEventHandlers = null)
        {
            if (xml == null) throw new ArgumentNullException(nameof(xml));
            var obj = (ProjectObjectGraph) XmlStreamService.Deserialize(xml, GetType(), xmlEventHandlers);
            FromJson(obj.ToJson());
        }

        /// <summary>
        ///     Creates a <see cref="ProjectObjectGraph" /> from an xml representation
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xml"></param>
        /// <param name="xmlEventHandlers"></param>
        /// <returns></returns>
        public static T CreateFromXml<T>(string xml, XmlEventHandlers xmlEventHandlers = null) where T : ProjectObjectGraph
        {
            if (xml == null) throw new ArgumentNullException(nameof(xml));
            var obj = (T) XmlStreamService.Deserialize(xml, typeof(T), xmlEventHandlers);
            return obj;
        }

        /// <summary>
        ///     Converts the <see cref="ProjectObjectGraph" /> to a json representation
        /// </summary>
        /// <param name="serializerSettings"></param>
        /// <returns></returns>
        public string ToJson(JsonSerializerSettings serializerSettings = null)
        {
            return JsonConvert.SerializeObject(this, serializerSettings ?? GetDefaultJsonSerializerSettings());
        }

        /// <summary>
        ///     Populates the <see cref="ProjectObjectGraph" /> from its json representation
        /// </summary>
        /// <param name="json"></param>
        /// <param name="serializerSettings"></param>
        public virtual void FromJson(string json, JsonSerializerSettings serializerSettings = null)
        {
            if (json == null) throw new ArgumentNullException(nameof(json));
            JsonConvert.PopulateObject(json, this, serializerSettings ?? GetDefaultJsonSerializerSettings());
        }

        /// <summary>
        ///     Populates the <see cref="ProjectObjectGraph" /> from its json representation
        /// </summary>
        /// <param name="json"></param>
        /// <param name="serializerSettings"></param>
        public static T CreateFromJson<T>(string json, JsonSerializerSettings serializerSettings = null) where T : ProjectObjectGraph, new()
        {
            if (json == null) throw new ArgumentNullException(nameof(json));
            var obj = new T();
            obj.FromJson(json, serializerSettings);
            return obj;
        }

        /// <summary>
        ///     Creates a deep copy of the <see cref="ProjectObjectGraph" /> using its json representation. By default the system
        ///     preserves object references
        /// </summary>
        /// <see cref="referencesHandling" />
        /// <returns></returns>
        public virtual ProjectObjectGraph DeepCopy(PreserveReferencesHandling referencesHandling = PreserveReferencesHandling.All)
        {
            var settings = new JsonSerializerSettings
            {
                PreserveReferencesHandling = referencesHandling,
                TypeNameHandling = TypeNameHandling.Auto
            };
            return (ProjectObjectGraph) JsonConvert.DeserializeObject(ToJson(), GetType(), settings);
        }

        /// <summary>
        ///     Get the default <see cref="XmlEventHandlers" /> for xml serialization
        /// </summary>
        /// <returns></returns>
        protected virtual XmlEventHandlers GetDefaultXmlEventHandlers()
        {
            return null;
        }

        /// <summary>
        ///     Get the default <see cref="JsonSerializerSettings" /> for json serialization
        /// </summary>
        /// <returns></returns>
        protected virtual JsonSerializerSettings GetDefaultJsonSerializerSettings()
        {
            return new JsonSerializerSettings
            {
                PreserveReferencesHandling = PreserveReferencesHandling.All,
                TypeNameHandling = TypeNameHandling.Auto,
                Culture = DefaultCultureInfo
            };
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return Name ?? base.ToString();
        }
    }
}