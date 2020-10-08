﻿using Mocassin.Model.Translator;
using Mocassin.UI.Data.Base;
using Mocassin.UI.Data.Customization;
using Mocassin.UI.Data.Jobs;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Mocassin.UI.Data.Main
{
    /// <summary>
    ///     The main project data root for a Mocassin project data and database creation instructions
    /// </summary>
    [XmlRoot]
    public class MocassinProject : ProjectDataEntity
    {
        private ObservableCollection<ProjectCustomizationTemplate> customizationTemplates;
        private bool isActiveProject;
        private ObservableCollection<ProjectJobSetTemplate> jobSetTemplates;
        private string projectGuid;
        private ProjectModelData projectModelData;
        private ResourcesData resources;
        private ObservableCollection<SimulationDbBuildTemplate> simulationDbBuildTemplates;
        private string version = "0.0.0.0";

        /// <summary>
        ///     Get or set a Guid for the project
        /// </summary>
        [XmlAttribute]
        public string ProjectGuid
        {
            get => projectGuid;
            set => SetProperty(ref projectGuid, value);
        }

        /// <summary>
        ///     Get or set the affiliated GUI version string
        /// </summary>
        [XmlAttribute, NotMapped]
        public string Version
        {
            get => version;
            set => SetProperty(ref version, value);
        }

        /// <summary>
        ///     Get or set a name for the project (Alias property)
        /// </summary>
        [XmlIgnore, JsonIgnore]
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
        [XmlIgnore, JsonIgnore, Column("ProjectJson")]
        public virtual string Json
        {
            get => ToJson();
            set => FromJson(value);
        }

        /// <summary>
        ///     Get or set the <see cref="ProjectModelData" /> that defines the reference information
        /// </summary>
        [XmlElement, NotMapped]
        public ProjectModelData ProjectModelData
        {
            get => projectModelData;
            set => SetProperty(ref projectModelData, value);
        }

        /// <summary>
        ///     Get or set the list of <see cref="ProjectCustomizationTemplate" /> that defines parameters for auto generated
        ///     content
        /// </summary>
        [XmlArray, NotMapped]
        public ObservableCollection<ProjectCustomizationTemplate> CustomizationTemplates
        {
            get => customizationTemplates;
            set => SetProperty(ref customizationTemplates, value);
        }

        /// <summary>
        ///     Get or set the list of <see cref="ProjectJobSetTemplate" /> that defines
        ///     <see cref="ISimulationLibrary" /> build instructions
        /// </summary>
        [XmlArray, NotMapped]
        public ObservableCollection<ProjectJobSetTemplate> JobSetTemplates
        {
            get => jobSetTemplates;
            set => SetProperty(ref jobSetTemplates, value);
        }

        /// <summary>
        ///     Get or set the list of <see cref="SimulationDbBuildTemplate" /> that defines
        ///     a full translation instruction for projects into databases
        /// </summary>
        [XmlArray, NotMapped]
        public ObservableCollection<SimulationDbBuildTemplate> SimulationDbBuildTemplates
        {
            get => simulationDbBuildTemplates;
            set => SetProperty(ref simulationDbBuildTemplates, value);
        }

        /// <summary>
        ///     Get or set the <see cref="ResourcesData" /> for the project
        /// </summary>
        [NotMapped, XmlIgnore]
        public ResourcesData Resources
        {
            get => resources;
            set => SetProperty(ref resources, value);
        }

        /// <summary>
        ///     Get or set a boolean flag if the project is set to be active
        /// </summary>
        [NotMapped, XmlIgnore]
        public bool IsActiveProject
        {
            get => isActiveProject;
            set => SetProperty(ref isActiveProject, value);
        }

        /// <summary>
        ///     Create empty <see cref="MocassinProject" />
        /// </summary>
        public MocassinProject()
        {
            CustomizationTemplates = new ObservableCollection<ProjectCustomizationTemplate>();
            JobSetTemplates = new ObservableCollection<ProjectJobSetTemplate>();
            SimulationDbBuildTemplates = new ObservableCollection<SimulationDbBuildTemplate>();
            Resources = new ResourcesData();
        }

        /// <summary>
        ///     Creates a new empty default <see cref="MocassinProject" />
        /// </summary>
        /// <returns></returns>
        public static MocassinProject CreateNew()
        {
            var guid = Guid.NewGuid().ToString();
            var project = new MocassinProject
            {
                ProjectGuid = guid,
                ProjectName = "New project",
                Version = Assembly.GetCallingAssembly().GetName().Version.ToString(),
                ProjectModelData = ProjectModelData.CreateNew()
            };

            project.ProjectModelData.Parent = project;
            return project;
        }


        /// <inheritdoc />
        public override void FromJson(string json, JsonSerializerSettings serializerSettings = null)
        {
            serializerSettings ??= GetDefaultJsonSerializerSettings();
            serializerSettings.SerializationBinder = new MocassinProjectSerializationBinder();
            base.FromJson(json, serializerSettings);
            RestoreParentReferences();
        }

        /// <summary>
        ///     Restores the internal parent references and other missing references that are not covered though the JSON
        ///     serialization
        /// </summary>
        private void RestoreParentReferences()
        {
            ProjectModelData.Parent = this;
            foreach (var item in CustomizationTemplates) item.Parent = this;
            foreach (var item in JobSetTemplates) item.Parent = this;
            foreach (var item in SimulationDbBuildTemplates)
            {
                item.Parent = this;
                item.RestoreBuildReferences();
            }
        }
    }
}