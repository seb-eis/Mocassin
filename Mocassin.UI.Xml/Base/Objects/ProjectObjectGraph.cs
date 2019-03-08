using System;
using System.Globalization;
using System.Text;
using System.Xml.Serialization;
using Mocassin.Framework.Xml;
using Newtonsoft.Json;

namespace Mocassin.UI.Xml.Base
{
    /// <summary>
    ///     Base class for all serializable project object graphs that support xml and json serialization
    /// </summary>
    [XmlRoot]
    public abstract class ProjectObjectGraph
    {
        /// <summary>
        ///     Specifies the default serialization encoding
        /// </summary>
        public static Encoding DefaultEncoding { get; set; } = Encoding.UTF8;

        /// <summary>
        ///     Specifies the default culture info for serialization
        /// </summary>
        public static CultureInfo DefaultCultureInfo { get; set; } = CultureInfo.InvariantCulture;

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
        ///     Converts the <see cref="ProjectObjectGraph" /> to a json representation
        /// </summary>
        /// <param name="serializerSettings"></param>
        /// <returns></returns>
        public string ToJson(JsonSerializerSettings serializerSettings = null)
        {
            return JsonConvert.SerializeObject(this, serializerSettings ?? GetDefaultSerializerSettings());
        }

        /// <summary>
        ///     Populates the <see cref="ProjectObjectGraph" /> from its json representation
        /// </summary>
        /// <param name="json"></param>
        /// <param name="serializerSettings"></param>
        public void FromJson(string json, JsonSerializerSettings serializerSettings = null)
        {
            if (json == null) throw new ArgumentNullException(nameof(json));
            JsonConvert.PopulateObject(json, this, serializerSettings ?? GetDefaultSerializerSettings());
        }

        /// <summary>
        ///     Creates a deep copy of the <see cref="ProjectObjectGraph" /> using its json representation. By default the system
        ///     preserves object references
        /// </summary>
        /// <see cref="referencesHandling" />
        /// <returns></returns>
        public ProjectObjectGraph DeepCopy(PreserveReferencesHandling referencesHandling = PreserveReferencesHandling.Objects)
        {
            var settings = new JsonSerializerSettings {PreserveReferencesHandling = referencesHandling};
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
        ///     Get the default <see cref="JsonSerializerSettings" /> for json conversion
        /// </summary>
        /// <returns></returns>
        protected virtual JsonSerializerSettings GetDefaultSerializerSettings()
        {
            return new JsonSerializerSettings
            {
                PreserveReferencesHandling = PreserveReferencesHandling.Objects, 
                Culture = DefaultCultureInfo
            };
        }
    }
}