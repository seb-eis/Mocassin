using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Mocassin.Model.Simulations;
using Mocassin.Model.Transitions;
using Mocassin.UI.Xml.Base;
using Mocassin.UI.Xml.TransitionModel;

namespace Mocassin.UI.Xml.SimulationModel
{
    /// <summary>
    ///     Serializable data object for <see cref="Mocassin.Model.Simulations.IKineticSimulation" /> model object creation
    /// </summary>
    [XmlRoot("KineticSimulation")]
    public class KineticSimulationGraph : SimulationBaseGraph
    {
        /// <summary>
        /// Get or set the pre-run mcsp for normalization and relaxation of the lattice
        /// </summary>
        [XmlAttribute("PrerunMcsp")]
        public int PreRunMcsp { get; set; } = 100;

        /// <summary>
        ///     Get or set a custom normalization probability for the simulation
        /// </summary>
        [XmlAttribute("NormalizationProbability")]
        public double NormalizationProbability { get; set; } = 1.0;

        /// <summary>
        ///     Get or set the electric field magnitude in [V/m]
        /// </summary>
        [XmlAttribute("ElectricFieldModulus")]
        public double ElectricFieldMagnitude { get; set; } = 100e6;

        /// <summary>
        ///     Get or set the electric field direction vector in fractional coordinates
        /// </summary>
        [XmlElement("ElectricFieldVector")]
        public VectorGraph3D ElectricFieldVector { get; set; } = new VectorGraph3D {A = 1};

        /// <summary>
        ///     Get or set the list of active kinetic transitions in this kinetic simulation
        /// </summary>
        [XmlArray("Transitions")]
        [XmlArrayItem("Transition")]
        public List<ModelObjectReferenceGraph<KineticTransition>> Transitions { get; set; }

        /// <summary>
        ///     Creates new <see cref="KineticSimulationGraph"/> with empty component lists
        /// </summary>
        public KineticSimulationGraph()
        {
            Transitions = new List<ModelObjectReferenceGraph<KineticTransition>>();
        }

        /// <inheritdoc />
        protected override SimulationBase GetPreparedSpecifiedSimulation()
        {
            var obj = new KineticSimulation
            {
                NormalizationProbability = NormalizationProbability,
                ElectricFieldMagnitude = Math.Abs(ElectricFieldMagnitude),
                ElectricFieldVector = ElectricFieldVector.AsDataVector3D(),
                Transitions = Transitions.Select(x => x.GetInputObject()).Cast<IKineticTransition>().ToList(),
                PreRunMcsp = PreRunMcsp
            };
            return obj;
        }
    }
}