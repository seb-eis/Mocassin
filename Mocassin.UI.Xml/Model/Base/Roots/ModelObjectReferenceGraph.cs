using System;
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
    public class ModelObjectReferenceGraph<T> : ModelObjectGraph where T : ModelObject, new()
    {
        [XmlIgnore]
        [NotMapped]
        [JsonIgnore]
        private ModelObjectGraph targetGraph;

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
        [XmlAttribute("Name")]
        [JsonProperty("Name")]
        [NotMapped]
        public override string Name
        {
            get => TargetGraph?.Name ?? base.Name; 
            set => base.Name = value;
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

        /// <inheritdoc />
        protected override ModelObject GetModelObjectInternal()
        {
            return new T {Key = TargetGraph?.Key ?? Key};
        }
    }
}