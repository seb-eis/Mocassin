using System;
using System.Collections.Generic;
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
        ///     Get or set the <see cref="Xml.Model.ProjectModelGraph" /> that defines the reference information
        /// </summary>
        [XmlElement("ProjectModelGraph")]
        public ProjectModelGraph ProjectModelGraph { get; set; }

        /// <summary>
        ///     Get or set the list of <see cref="ProjectCustomizationGraph" /> that defines parameters for auto generated content
        /// </summary>
        [XmlArray("ModelCustomizations")]
        [XmlArrayItem("ModelCustomization")]
        public List<ProjectCustomizationGraph> ProjectCustomizationGraphs { get; set; }

        /// <summary>
        ///     Get or set the list of <see cref="ProjectJobTranslationGraph" /> that defines
        ///     <see cref="IMocassinSimulationLibrary" /> build instructions
        /// </summary>
        [XmlArray("DbCreationInstructions")]
        [XmlArrayItem("DbCreationInstruction")]
        public List<ProjectJobTranslationGraph> ProjectJobTranslationGraphs { get; set; }

        /// <summary>
        ///     Get or set the list of <see cref="MocassinProjectBuildGraph" /> that defines
        ///     a full translation instruction for projects into databases
        /// </summary>
        [XmlArray("BuildGraphs")]
        [XmlArrayItem("BuildGraph")]
        public List<MocassinProjectBuildGraph> ProjectBuildGraphs { get; set; }

        /// <summary>
        ///     Create empty <see cref="MocassinProjectGraph"/>
        /// </summary>
        public MocassinProjectGraph()
        {
            ProjectCustomizationGraphs = new List<ProjectCustomizationGraph>();
            ProjectJobTranslationGraphs = new List<ProjectJobTranslationGraph>();
            ProjectBuildGraphs = new List<MocassinProjectBuildGraph>();
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
    }
}