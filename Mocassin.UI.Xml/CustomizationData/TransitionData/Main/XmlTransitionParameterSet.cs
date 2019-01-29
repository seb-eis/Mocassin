using System.Collections.Generic;
using System.Xml.Serialization;

namespace Mocassin.UI.Xml.CustomizationData
{
    /// <summary>
    ///     Serializable object that carries data for customization of <see cref="Mocassin.Model.Energies.IEnergyManager" />
    ///     interaction settings through the <see cref="IEnergySetterProvider" /> system
    /// </summary>
    [XmlRoot("TransitionModelParametrization")]
    public class XmlTransitionParameterSet
    {
        /// <summary>
        ///     Get or set the list of affiliated kinetic rules
        /// </summary>
        [XmlArray("KineticRules")]
        [XmlArrayItem("KineticRule")]
        public List<XmlKineticRule> KineticRules { get; set; }
    }
}