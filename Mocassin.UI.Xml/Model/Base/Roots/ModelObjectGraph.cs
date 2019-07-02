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
        ///     The key of the model object. Has to be unique within the object graph type group
        /// </summary>
        [XmlAttribute("Key")]
        [Column("Key")]
        [JsonProperty("Key")]
        public string Key { get; set; }

        /// <summary>
        ///     Creates a new <see cref="ModelObjectGraph"/> with a unique object key
        /// </summary>
        protected ModelObjectGraph()
        {
            Key = Guid.NewGuid().ToString();
        }

        /// <summary>
        ///     Get the input <see cref="ModelObject" /> for the automated data input system of the model management
        /// </summary>
        /// <returns></returns>
        public ModelObject GetInputObject()
        {
            var obj = GetModelObjectInternal();
            obj.Key = Key ?? Guid.NewGuid().ToString();
            obj.Name = Name;
            obj.Index = -1;
            return obj;
        }

        /// <summary>
        ///     Get a prepared <see cref="ModelObject" /> with all specific input data set
        /// </summary>
        /// <returns></returns>
        protected abstract ModelObject GetModelObjectInternal();

        /// <inheritdoc />
        public override string ToString()
        {
            return string.IsNullOrWhiteSpace(Name) 
                ? $"[{Key}]" 
                : Name;
        }
    }
}