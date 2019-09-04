using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;
using Mocassin.Model.Translator;
using Mocassin.UI.Xml.Base;
using Mocassin.UI.Xml.Customization;
using Mocassin.UI.Xml.Jobs;
using Mocassin.UI.Xml.Model;
using Newtonsoft.Json;

namespace Mocassin.UI.Xml.Main
{
    /// <summary>
    ///     The main project data root for a Mocassin project data and database creation instructions
    /// </summary>
    [XmlRoot("MocassinProject")]
    public class MocassinProjectGraph : MocassinProjectEntity
    {
        /// <summary>
        ///     Get or set a Guid for the project
        /// </summary>
        [XmlAttribute("ProjectGuid")]
        public string ProjectGuid { get; set; }

        /// <summary>
        ///     Get or set a name for the project
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        public string ProjectName
        {
            get => Name;
            set => Name = value;
        }

        /// <summary>
        ///     Get or set the contents of the object by a json string representation
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        [Column("ProjectJson")]
        public virtual string Json
        {
            get => ToJson();
            set => FromJson(value);
        }

        /// <summary>
        ///     Get or set the <see cref="Xml.Model.ProjectModelGraph" /> that defines the reference information
        /// </summary>
        [XmlElement("ProjectModelGraph")]
        [NotMapped]
        public ProjectModelGraph ProjectModelGraph { get; set; }

        /// <summary>
        ///     Get or set the list of <see cref="ProjectCustomizationGraph" /> that defines parameters for auto generated content
        /// </summary>
        [XmlArray("ModelCustomizations")]
        [XmlArrayItem("ModelCustomization")]
        [NotMapped]
        public List<ProjectCustomizationGraph> ProjectCustomizationGraphs { get; set; }

        /// <summary>
        ///     Get or set the list of <see cref="ProjectJobTranslationGraph" /> that defines
        ///     <see cref="ISimulationLibrary" /> build instructions
        /// </summary>
        [XmlArray("DbCreationInstructions")]
        [XmlArrayItem("DbCreationInstruction")]
        [NotMapped]
        public List<ProjectJobTranslationGraph> ProjectJobTranslationGraphs { get; set; }

        /// <summary>
        ///     Get or set the list of <see cref="MocassinProjectBuildGraph" /> that defines
        ///     a full translation instruction for projects into databases
        /// </summary>
        [XmlArray("BuildGraphs")]
        [XmlArrayItem("BuildGraph")]
        [NotMapped]
        public List<MocassinProjectBuildGraph> ProjectBuildGraphs { get; set; }

        /// <summary>
        ///     Get or set the <see cref="ResourcesGraph"/> for the project
        /// </summary>
        [NotMapped]
        [XmlIgnore]
        public ResourcesGraph Resources { get; set; }

        /// <summary>
        ///     Create empty <see cref="MocassinProjectGraph" />
        /// </summary>
        public MocassinProjectGraph()
        {
            ProjectCustomizationGraphs = new List<ProjectCustomizationGraph>();
            ProjectJobTranslationGraphs = new List<ProjectJobTranslationGraph>();
            ProjectBuildGraphs = new List<MocassinProjectBuildGraph>();
            Resources = new ResourcesGraph();
        }

        /// <summary>
        ///     Creates a new empty default <see cref="MocassinProjectGraph" />
        /// </summary>
        /// <returns></returns>
        public static MocassinProjectGraph CreateNew()
        {
            var guid = Guid.NewGuid().ToString();
            return new MocassinProjectGraph
            {
                ProjectGuid = guid,
                ProjectName = "New project",
                ProjectModelGraph = ProjectModelGraph.CreateNew()
            };
        }


        /// <inheritdoc />
        public override void FromJson(string json, JsonSerializerSettings serializerSettings = null)
        {
            base.FromJson(json, serializerSettings);
            RestoreParentReferences();
        }

        /// <summary>
        ///     Restores the internal parent references and other missing references that are not covered though the JSON
        ///     serialization
        /// </summary>
        private void RestoreParentReferences()
        {
            ProjectModelGraph.Parent = this;
            foreach (var item in ProjectCustomizationGraphs) item.Parent = this;
            foreach (var item in ProjectJobTranslationGraphs) item.Parent = this;
            foreach (var item in ProjectBuildGraphs)
            {
                item.Parent = this;
                item.RestoreBuildReferences();
            }
        }
    }
}