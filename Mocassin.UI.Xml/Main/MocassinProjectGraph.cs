using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private string projectGuid;
        private ProjectModelGraph projectModelGraph;
        private ObservableCollection<ProjectCustomizationGraph> projectCustomizationGraphs;
        private ObservableCollection<ProjectJobTranslationGraph> projectJobTranslationGraphs;
        private ObservableCollection<MocassinProjectBuildGraph> projectBuildGraphs;
        private ResourcesGraph resources;

        /// <summary>
        ///     Get or set a Guid for the project
        /// </summary>
        [XmlAttribute("ProjectGuid")]
        public string ProjectGuid
        {
            get => projectGuid;
            set => SetProperty(ref projectGuid, value);
        }

        /// <summary>
        ///     Get or set a name for the project (Alias property)
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        public string ProjectName
        {
            get => Name;
            set
            {
                Name = value;
                OnPropertyChanged();
            }
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
        public ProjectModelGraph ProjectModelGraph
        {
            get => projectModelGraph;
            set => SetProperty(ref projectModelGraph, value);
        }

        /// <summary>
        ///     Get or set the list of <see cref="ProjectCustomizationGraph" /> that defines parameters for auto generated content
        /// </summary>
        [XmlArray("ModelCustomizations")]
        [XmlArrayItem("ModelCustomization")]
        [NotMapped]
        public ObservableCollection<ProjectCustomizationGraph> ProjectCustomizationGraphs
        {
            get => projectCustomizationGraphs;
            set => SetProperty(ref projectCustomizationGraphs, value);
        }

        /// <summary>
        ///     Get or set the list of <see cref="ProjectJobTranslationGraph" /> that defines
        ///     <see cref="ISimulationLibrary" /> build instructions
        /// </summary>
        [XmlArray("DbCreationInstructions")]
        [XmlArrayItem("DbCreationInstruction")]
        [NotMapped]
        public ObservableCollection<ProjectJobTranslationGraph> ProjectJobTranslationGraphs
        {
            get => projectJobTranslationGraphs;
            set => SetProperty(ref projectJobTranslationGraphs, value);
        }

        /// <summary>
        ///     Get or set the list of <see cref="MocassinProjectBuildGraph" /> that defines
        ///     a full translation instruction for projects into databases
        /// </summary>
        [XmlArray("BuildGraphs")]
        [XmlArrayItem("BuildGraph")]
        [NotMapped]
        public ObservableCollection<MocassinProjectBuildGraph> ProjectBuildGraphs
        {
            get => projectBuildGraphs;
            set => SetProperty(ref projectBuildGraphs, value);
        }

        /// <summary>
        ///     Get or set the <see cref="ResourcesGraph" /> for the project
        /// </summary>
        [NotMapped]
        [XmlIgnore]
        public ResourcesGraph Resources
        {
            get => resources;
            set => SetProperty(ref resources, value);
        }

        /// <summary>
        ///     Create empty <see cref="MocassinProjectGraph" />
        /// </summary>
        public MocassinProjectGraph()
        {
            ProjectCustomizationGraphs = new ObservableCollection<ProjectCustomizationGraph>();
            ProjectJobTranslationGraphs = new ObservableCollection<ProjectJobTranslationGraph>();
            ProjectBuildGraphs = new ObservableCollection<MocassinProjectBuildGraph>();
            Resources = new ResourcesGraph();
        }

        /// <summary>
        ///     Creates a new empty default <see cref="MocassinProjectGraph" />
        /// </summary>
        /// <returns></returns>
        public static MocassinProjectGraph CreateNew()
        {
            var guid = Guid.NewGuid().ToString();
            var project = new MocassinProjectGraph
            {
                ProjectGuid = guid,
                ProjectName = "New project",
                ProjectModelGraph = ProjectModelGraph.CreateNew()
            };

            project.ProjectModelGraph.Parent = project;
            return project;
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