using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using Mocassin.Framework.Extensions;
using Mocassin.Model.Particles;
using Mocassin.Model.Translator;
using Mocassin.UI.Data.Base;
using Mocassin.UI.Data.Customization;
using Mocassin.UI.Data.Jobs;
using Mocassin.UI.Data.ParticleModel;
using Newtonsoft.Json;

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
        public override ProjectDataObject DeepCopy(PreserveReferencesHandling referencesHandling = PreserveReferencesHandling.All)
        {
            var copy = (MocassinProject) base.DeepCopy(referencesHandling);
            copy.RestoreParentReferences();
            return copy;
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
        ///     and XML serialization
        /// </summary>
        public void RestoreParentReferences()
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

        /// <summary>
        ///     Restores the all targets of existing <see cref="ModelObjectReference{T}" />. This call is required when deserializing from XML
        /// </summary>
        public void RestoreModelObjectReferenceTargets()
        {
            static void RecurseFind(object root, ISet<object> searched, ISet<ModelDataObject> foundData, ICollection<object> foundRefs)
            {
                if (root is null) return;
                searched.Add(root);
                var rootType = root.GetType();
                switch (root)
                {
                    case ModelDataObject dataObject:
                        foundData.Add(dataObject);
                        break;
                    case { } objRef when
                        rootType.IsGenericType &&
                        rootType.GetGenericTypeDefinition() == typeof(ModelObjectReference<>):
                        foundRefs.Add(objRef);
                        return;
                }

                if (root is IEnumerable<object> set)
                {
                    set.Where(x => !x.GetType().IsValueType)
                       .Action(x => RecurseFind(x, searched, foundData, foundRefs))
                       .Load();
                }
                else
                {
                    root.GetType()
                        .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                        .Where(x => !x.PropertyType.IsValueType && x.PropertyType != typeof(string) && x.GetIndexParameters().Length == 0)
                        .Select(x => x.GetValue(root))
                        .Where(x => !searched.Contains(x))
                        .Action(x => RecurseFind(x, searched, foundData, foundRefs))
                        .Load();
                }
            }

            static void Link(ISet<ModelDataObject> foundData, IEnumerable<object> foundRefs)
            {
                foreach (var item in foundRefs)
                {
                    var targetProp = item.GetType().GetProperty("Target");
                    var key = (string) item.GetType().GetProperty("Key")!.GetValue(item);
                    var target = key == ParticleData.VoidParticle.Key && item is ModelObjectReference<Particle>
                        ? ParticleData.VoidParticle
                        : foundData.Single(x => x.Key == key);
                    targetProp!.SetValue(item, target);
                }
            }

            var searched = new HashSet<object>();
            var foundData = new HashSet<ModelDataObject>();
            var foundRefs = new List<object>();

            RecurseFind(this, searched, foundData, foundRefs);
            Link(foundData, foundRefs);
        }

        /// <summary>
        ///     Creates a new <see cref="MocassinProject" /> from its xml representation
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static MocassinProject LoadFromXml(string xml)
        {
            var project = CreateFromXml<MocassinProject>(xml);
            project.RestoreParentReferences();
            project.RestoreModelObjectReferenceTargets();
            return project;
        }
    }
}