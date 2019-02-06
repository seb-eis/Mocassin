using System;
using System.Reflection;
using System.Xml.Serialization;
using Mocassin.Model.Energies;
using Mocassin.Model.ModelProject;
using Mocassin.Model.Transitions;

namespace Mocassin.UI.Xml.CustomizationData
{
    /// <summary>
    ///     The main root for storage and serialization of customizable parametrization data of <see cref="IModelProject" />
    /// </summary>
    [XmlRoot("MocassinModelCustomizationGraph")]
    public class XmlProjectCustomization : XmlModelCustomizationData
    {
        /// <summary>
        ///     Get or set a key for the customization
        /// </summary>
        [XmlAttribute("Key")]
        public string Key { get; set; }

        /// <summary>
        ///     Get or set the <see cref="XmlEnergyModelCustomization" /> that stores energy customization data
        /// </summary>
        [XmlElement("EnergyCustomization")]
        [ModelCustomizationRoot]
        public XmlEnergyModelCustomization EnergyModelCustomization { get; set; }

        /// <summary>
        ///     Get or set the <see cref="XmlTransitionModelCustomization" /> that stores transition customization data
        /// </summary>
        [XmlElement("TransitionCustomization")]
        [ModelCustomizationRoot]
        public XmlTransitionModelCustomization TransitionModelCustomization { get; set; }

        /// <summary>
        ///     Creates new <see cref="XmlProjectCustomization" /> with auto generated key value
        /// </summary>
        public XmlProjectCustomization()
        {
            Key = Guid.NewGuid().ToString();
        }

        /// <inheritdoc />
        public override void PushToModel(IModelProject modelProject)
        {
            foreach (var propertyInfo in GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (propertyInfo.GetCustomAttribute<ModelCustomizationRootAttribute>() is null) continue;
                var data = propertyInfo.GetValue(this) as XmlModelCustomizationData;
                data?.PushToModel(modelProject);
            }
        }

        /// <summary>
        ///     Create a new <see cref="XmlProjectCustomization" /> by pulling all data from the passed
        ///     <see cref="IModelProject" />
        /// </summary>
        /// <param name="modelProject"></param>
        /// <returns></returns>
        public static XmlProjectCustomization Create(IModelProject modelProject)
        {
            if (modelProject == null) throw new ArgumentNullException(nameof(modelProject));
            var energySetterProvider = modelProject.GetManager<IEnergyManager>().QueryPort.Query(x => x.GetEnergySetterProvider());
            var ruleSetterProvider = modelProject.GetManager<ITransitionManager>().QueryPort.Query(x => x.GetRuleSetterProvider());

            var obj = new XmlProjectCustomization
            {
                EnergyModelCustomization = XmlEnergyModelCustomization.Create(energySetterProvider),
                TransitionModelCustomization = XmlTransitionModelCustomization.Create(ruleSetterProvider)
            };

            return obj;
        }
    }
}