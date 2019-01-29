using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using Mocassin.UI.Xml.BaseData;
using Mocassin.UI.Xml.EnergyData;
using Mocassin.UI.Xml.ParticleData;
using Mocassin.UI.Xml.SimulationData;
using Mocassin.UI.Xml.StructureData;
using Mocassin.UI.Xml.TransitionData;

namespace Mocassin.UI.Xml.ProjectData
{
    /// <summary>
    ///     The main root for mocassin project data input as a serialized information
    /// </summary>
    [XmlRoot("MocassinModelGraph")]
    public class XmlMocassinProjectData
    {
        /// <summary>
        ///     Get or set the input particle data
        /// </summary>
        [XmlElement("ParticleModel")]
        [ModelInputRoot(0)]
        public XmlParticleModelData ParticleModelData { get; set; }

        /// <summary>
        ///     Get or set the input structure data
        /// </summary>
        [XmlElement("StructureModel")]
        [ModelInputRoot(1)]
        public XmlStructureModelData StructureModelData { get; set; }

        /// <summary>
        ///     Get or set the input transition data
        /// </summary>
        [XmlElement("TransitionModel")]
        [ModelInputRoot(2)]
        public XmlTransitionModelData TransitionModelData { get; set; }

        /// <summary>
        ///     Get or set the input energy data
        /// </summary>
        [XmlElement("EnergyModel")]
        [ModelInputRoot(3)]
        public XmlEnergyModelData EnergyModelData { get; set; }

        /// <summary>
        ///     Get or set the input simulation data
        /// </summary>
        [XmlElement("SimulationModel")]
        [ModelInputRoot(4)]
        public XmlSimulationModelData SimulationModelData { get; set; }

        /// <summary>
        ///     Get the full input sequence for the <see cref="Mocassin.Model.ModelProject.IModelProject" /> input process pipeline
        /// </summary>
        /// <returns></returns>
        public IEnumerable<object> GetInputSequence()
        {
            var dataRoots = GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Select(x => ((XmlProjectManagerModelData) x.GetValue(this), x.GetCustomAttribute<ModelInputRootAttribute>()))
                .Where(x => x.Item2 != null && x.Item1 != null)
                .OrderBy(x => x.Item2.Order)
                .Select(x => x.Item1)
                .ToList();

            return dataRoots.SelectMany(x => x.GetInputSequence());
        }
    }
}