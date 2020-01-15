using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using System.Xml.Serialization;
using Mocassin.Model.Energies;
using Mocassin.Model.ModelProject;
using Mocassin.Model.Transitions;
using Mocassin.UI.Xml.Base;
using Mocassin.UI.Xml.Model;

namespace Mocassin.UI.Xml.Customization
{
    /// <summary>
    ///     The main root for storage and serialization of customizable parametrization data of <see cref="IModelProject" />
    /// </summary>
    [XmlRoot("MocassinModelCustomizationGraph")]
    public class ProjectCustomizationGraph : ModelCustomizationEntity, IDuplicable<ProjectCustomizationGraph>
    {
        private string key;
        private EnergyModelCustomizationGraph energyModelCustomization;
        private TransitionModelCustomizationGraph transitionModelCustomization;

        /// <summary>
        ///     Get the <see cref="ProjectCustomizationGraph"/> that represents an empty customization
        /// </summary>
        public static ProjectCustomizationGraph Empty { get; } = new ProjectCustomizationGraph {Name = "[Empty]"};

        /// <summary>
        ///     Get or set a key for the customization
        /// </summary>
        [XmlAttribute("Key")]
        [NotMapped]
        public string Key
        {
            get => key;
            set => SetProperty(ref key, value);
        }

        /// <summary>
        ///     Get or set the <see cref="EnergyModelCustomizationGraph" /> that stores energy customization data
        /// </summary>
        [XmlElement("EnergyCustomization")]
        [ModelCustomizationRoot]
        [NotMapped]
        public EnergyModelCustomizationGraph EnergyModelCustomization
        {
            get => energyModelCustomization;
            set => SetProperty(ref energyModelCustomization, value);
        }

        /// <summary>
        ///     Get or set the <see cref="TransitionModelCustomizationGraph" /> that stores transition customization data
        /// </summary>
        [XmlElement("TransitionCustomization")]
        [ModelCustomizationRoot]
        [NotMapped]
        public TransitionModelCustomizationGraph TransitionModelCustomization
        {
            get => transitionModelCustomization;
            set => SetProperty(ref transitionModelCustomization, value);
        }

        /// <summary>
        ///     Creates new <see cref="ProjectCustomizationGraph" /> with auto generated key value
        /// </summary>
        public ProjectCustomizationGraph()
        {
            Key = Guid.NewGuid().ToString();
        }

        /// <inheritdoc />
        public override void PushToModel(IModelProject modelProject)
        {
            foreach (var propertyInfo in GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (propertyInfo.GetCustomAttribute<ModelCustomizationRootAttribute>() is null) continue;
                var data = propertyInfo.GetValue(this) as ModelCustomizationEntity;
                data?.PushToModel(modelProject);
            }
        }

        /// <summary>
        ///     Create a new <see cref="ProjectCustomizationGraph" /> by pulling all data from the passed
        ///     <see cref="IModelProject" /> and <see cref="ProjectModelGraph" /> parent
        /// </summary>
        /// <param name="modelProject"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static ProjectCustomizationGraph Create(IModelProject modelProject, ProjectModelGraph parent)
        {
            if (modelProject == null) throw new ArgumentNullException(nameof(modelProject));
            if (parent == null) throw new ArgumentNullException(nameof(parent));

            var energySetterProvider = modelProject.GetManager<IEnergyManager>().QueryPort.Query(x => x.GetEnergySetterProvider());
            var ruleSetterProvider = modelProject.GetManager<ITransitionManager>().QueryPort.Query(x => x.GetRuleSetterProvider());

            var obj = new ProjectCustomizationGraph
            {
                Parent = parent.Parent,
                EnergyModelCustomization = EnergyModelCustomizationGraph.Create(energySetterProvider, parent),
                TransitionModelCustomization = TransitionModelCustomizationGraph.Create(ruleSetterProvider, parent)
            };
            obj.Name = $"New Customization [{obj.Parent.ProjectName}]";

            return obj;
        }

        /// <inheritdoc />
        public ProjectCustomizationGraph Duplicate()
        {
            return new ProjectCustomizationGraph
            {
                Parent = Parent,
                Name = $"{Name}(copy)",
                TransitionModelCustomization = TransitionModelCustomization.Duplicate(),
                EnergyModelCustomization = EnergyModelCustomization.Duplicate()
            };
        }

        /// <inheritdoc />
        object IDuplicable.Duplicate()
        {
            return Duplicate();
        }
    }
}