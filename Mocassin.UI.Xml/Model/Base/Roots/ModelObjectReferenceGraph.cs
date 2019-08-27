using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;
using Mocassin.Model.Basic;
using Newtonsoft.Json;

namespace Mocassin.UI.Xml.Base
{
    /// <summary>
    ///     Generic serializable class to store and provide key based references to specific <see cref="ModelObject" />
    ///     instances
    /// </summary>
    [XmlRoot]
    public sealed class ModelObjectReferenceGraph<T> :
        IEquatable<ModelObjectReferenceGraph<T>>,
        IDuplicable<ModelObjectReferenceGraph<T>> where T : ModelObject, new()
    {
        [XmlIgnore]
        [NotMapped]
        [JsonIgnore]
        private ModelObjectGraph targetGraph;

        /// <summary>
        ///     Get or set the key of the reference
        /// </summary>
        [JsonIgnore]
        [XmlAttribute("Key")]
        public string Key { get; set; }

        /// <summary>
        ///     Get the name of the object reference
        /// </summary>
        [JsonIgnore]
        [XmlIgnore]
        public string Name => TargetGraph?.Name ?? "TargetNull";

        /// <summary>
        ///     Get or set the reference target <see cref="ModelObjectGraph" />. Only serialized in JSON mode with reference
        ///     handling
        /// </summary>
        [XmlIgnore]
        [NotMapped]
        [JsonProperty("Target")]
        public ModelObjectGraph TargetGraph
        {
            get => targetGraph;
            set
            {
                targetGraph = value;
                Key = value?.Key;
            }
        }

        /// <inheritdoc />
        public ModelObjectReferenceGraph()
        {
        }

        /// <summary>
        ///     Creates new <see cref="ModelObjectReferenceGraph{T}"/> that targets the passed graph
        /// </summary>
        /// <param name="targetGraph"></param>
        public ModelObjectReferenceGraph(ModelObjectGraph targetGraph)
        {
            TargetGraph = targetGraph ?? throw new ArgumentNullException(nameof(targetGraph));
        }

        /// <summary>
        ///     Get the internal model object
        /// </summary>
        /// <returns></returns>
        public ModelObject GetInputObject()
        {
            return new T {Key = TargetGraph?.Key ?? Key};
        }

        /// <inheritdoc />
        public bool Equals(ModelObjectReferenceGraph<T> other)
        {
            if (other == null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Key == other.Key || ReferenceEquals(TargetGraph, other.TargetGraph);
        }

        /// <inheritdoc />
        public ModelObjectReferenceGraph<T> Duplicate()
        {
            return new ModelObjectReferenceGraph<T>(TargetGraph);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return Equals(obj as ModelObjectReferenceGraph<T>);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            var hashCode = 1207054110;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(TargetGraph?.Key);
            return hashCode;
        }

        /// <inheritdoc />
        object IDuplicable.Duplicate()
        {
            return Duplicate();
        }
    }
}