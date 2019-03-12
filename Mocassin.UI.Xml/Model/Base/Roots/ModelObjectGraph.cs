using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;
using Mocassin.Model.Basic;
using Newtonsoft.Json;

namespace Mocassin.UI.Xml.Base
{
    /// <summary>
    ///     Base class for all serializable data objects that supply <see cref="ModelObject" /> conversion for data input
    /// </summary>
    [XmlRoot]
    public abstract class ModelObjectGraph : ProjectObjectGraph
    {
        /// <summary>
        ///     Key value backing field
        /// </summary>
        [XmlIgnore] private string key;

        /// <summary>
        ///     Key reference value backing field
        /// </summary>
        [XmlIgnore] private string keyReference;

        /// <summary>
        ///     Get a boolean if the object is a reference
        /// </summary>
        [XmlIgnore]
        [NotMapped]
        [JsonIgnore]
        public bool IsReference => keyReference != null;

        /// <summary>
        ///     The key of the model object. Setting this value erases the reference
        /// </summary>
        [XmlAttribute("Key")]
        [Column("Key")]
        [JsonProperty("Key")]
        public string Key
        {
            get => key;
            set
            {
                keyReference = null;
                key = value;
            }
        }

        /// <summary>
        ///     The key reference of the model object. Setting this property erases the key
        /// </summary>
        [XmlAttribute("Ref")]
        [Column("Ref")]
        [JsonProperty("KeyReference")]
        public string KeyReference
        {
            get => keyReference;
            set
            {
                key = null;
                keyReference = value;
            }
        }

        /// <summary>
        ///     Get the set key or key reference of the object and throws an exception if both are null
        /// </summary>
        /// <returns></returns>
        public string GetKey()
        {
            return Key ?? KeyReference ?? throw new InvalidOperationException("Both key options are null");
        }

        /// <summary>
        ///     Get the input <see cref="ModelObject" /> for the automated data input system of the model management
        /// </summary>
        /// <returns></returns>
        public ModelObject GetInputObject()
        {
            var obj = GetModelObjectInternal();
            obj.Key = Key ?? KeyReference;
            obj.Index = -1;
            return obj;
        }

        /// <summary>
        ///     Get a prepared <see cref="ModelObject" /> with all specific input data set
        /// </summary>
        /// <returns></returns>
        protected abstract ModelObject GetModelObjectInternal();
    }
}