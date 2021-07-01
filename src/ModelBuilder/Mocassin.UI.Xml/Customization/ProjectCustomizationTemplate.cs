using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using System.Xml.Serialization;
using Mocassin.Model.Energies;
using Mocassin.Model.ModelProject;
using Mocassin.Model.Transitions;
using Mocassin.UI.Data.Base;

namespace Mocassin.UI.Data.Customization
{
    /// <summary>
    ///     The main root for storage and serialization of customizable parametrization data of <see cref="IModelProject" />
    /// </summary>
    [XmlRoot("MocassinModelCustomizationGraph")]
    public class ProjectCustomizationTemplate : ModelCustomizationData, IDuplicable<ProjectCustomizationTemplate>
    {
        private EnergyModelCustomizationData energyModelCustomization;
        private string key;
        private TransitionModelCustomizationData transitionModelCustomization;

        /// <summary>
        ///     Get the <see cref="ProjectCustomizationTemplate" /> that represents an empty customization
        /// </summary>
        public static ProjectCustomizationTemplate Empty { get; } = new ProjectCustomizationTemplate {Name = "[Empty]"};

        /// <summary>
        ///     Get or set a key for the customization
        /// </summary>
        [XmlAttribute("Key"), NotMapped]
        public string Key
        {
            get => key;
            set => SetProperty(ref key, value);
        }

        /// <summary>
        ///     Get or set the <see cref="EnergyModelCustomizationData" /> that stores energy customization data
        /// </summary>
        [XmlElement("EnergyCustomization"), ModelCustomizationRoot, NotMapped]
        public EnergyModelCustomizationData EnergyModelCustomization
        {
            get => energyModelCustomization;
            set => SetProperty(ref energyModelCustomization, value);
        }

        /// <summary>
        ///     Get or set the <see cref="TransitionModelCustomizationData" /> that stores transition customization data
        /// </summary>
        [XmlElement("TransitionCustomization"), ModelCustomizationRoot, NotMapped]
        public TransitionModelCustomizationData TransitionModelCustomization
        {
            get => transitionModelCustomization;
            set => SetProperty(ref transitionModelCustomization, value);
        }

        /// <summary>
        ///     Creates new <see cref="ProjectCustomizationTemplate" /> with auto generated key value
        /// </summary>
        public ProjectCustomizationTemplate()
        {
            Key = Guid.NewGuid().ToString();
        }

        /// <inheritdoc />
        public ProjectCustomizationTemplate Duplicate() =>
            new ProjectCustomizationTemplate
            {
                Parent = Parent,
                Name = $"{Name}(copy)",
                TransitionModelCustomization = TransitionModelCustomization.Duplicate(),
                EnergyModelCustomization = EnergyModelCustomization.Duplicate()
            };

        /// <inheritdoc />
        object IDuplicable.Duplicate() => Duplicate();

        /// <inheritdoc />
        public override void PushToModel(IModelProject modelProject)
        {
            foreach (var propertyInfo in GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (propertyInfo.GetCustomAttribute<ModelCustomizationRootAttribute>() is null) continue;
                var data = propertyInfo.GetValue(this) as ModelCustomizationData;
                data?.PushToModel(modelProject);
            }
        }

        /// <summary>
        ///     Create a new <see cref="ProjectCustomizationTemplate" /> by pulling all data from the passed
        ///     <see cref="IModelProject" /> and <see cref="ProjectModelData" /> parent
        /// </summary>
        /// <param name="modelProject"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static ProjectCustomizationTemplate Create(IModelProject modelProject, ProjectModelData parent)
        {
            if (modelProject == null) throw new ArgumentNullException(nameof(modelProject));
            if (parent == null) throw new ArgumentNullException(nameof(parent));

            var energySetterProvider = modelProject.Manager<IEnergyManager>().DataAccess.Query(x => x.GetEnergySetterProvider());

            var obj = new ProjectCustomizationTemplate
            {
                Parent = parent.Parent,
                EnergyModelCustomization = EnergyModelCustomizationData.Create(energySetterProvider, parent),
                TransitionModelCustomization = TransitionModelCustomizationData.Create(modelProject, parent)
            };
            obj.Name = $"New Customization [{obj.Parent.ProjectName}]";

            return obj;
        }
    }
}