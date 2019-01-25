using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Mocassin.Model.Simulations;
using Mocassin.Model.Transitions;
using Mocassin.UI.Xml.BaseData;
using Mocassin.UI.Xml.TransitionData;

namespace Mocassin.UI.Xml.SimulationData
{
    /// <summary>
    ///     Serializable data object for <see cref="Mocassin.Model.Simulations.IKineticSimulation" /> model object creation
    /// </summary>
    [XmlRoot("KineticSimulation")]
    public class XmlKineticSimulation : XmlSimulationBase
    {
        /// <summary>
        ///     Get or set a custom normalization probability for the simulation
        /// </summary>
        [XmlAttribute("NormalizationProbability")]
        public double NormalizationProbability { get; set; }

        /// <summary>
        ///     Get or set the electric field magnitude in [V/m]
        /// </summary>
        [XmlAttribute("ElectricFieldModulus")]
        public double ElectricFieldMagnitude { get; set; }

        /// <summary>
        ///     Get or set the electric field direction vector in fractional coordinates
        /// </summary>
        [XmlElement("ElectricFieldVector")]
        public XmlVector3D ElectricFieldVector { get; set; }

        /// <summary>
        ///     Get or set the list of active kinetic transitions in this kinetic simulation
        /// </summary>
        [XmlArray("Transitions")]
        [XmlArrayItem("Transition")]
        public List<XmlKineticTransition> Transitions { get; set; }

        /// <inheritdoc />
        protected override SimulationBase GetPreparedSpecifiedSimulation()
        {
            var obj = new KineticSimulation
            {
                NormalizationProbability = NormalizationProbability,
                ElectricFieldMagnitude = Math.Abs(ElectricFieldMagnitude),
                ElectricFieldVector = ElectricFieldVector.AsDataVector3D(),
                Transitions = Transitions.Select(x => x.GetInputObject()).Cast<IKineticTransition>().ToList()
            };
            return obj;
        }
    }
}