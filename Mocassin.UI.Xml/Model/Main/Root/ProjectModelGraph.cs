using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using Mocassin.UI.Xml.Base;
using Mocassin.UI.Xml.EnergyModel;
using Mocassin.UI.Xml.Main;
using Mocassin.UI.Xml.ParticleModel;
using Mocassin.UI.Xml.SimulationModel;
using Mocassin.UI.Xml.StructureModel;
using Mocassin.UI.Xml.TransitionModel;

namespace Mocassin.UI.Xml.Model
{
    /// <summary>
    ///     The main root for mocassin project data that targets <see cref="Mocassin.Model.Basic.IModelManager" /> through the
    ///     <see cref="Mocassin.Model.ModelProject.IProjectInputPipeline" /> interface
    /// </summary>
    [XmlRoot("MocassinModelGraph")]
    public class ProjectModelGraph : MocassinProjectChildEntity<MocassinProjectGraph>
    {
        /// <summary>
        ///     Get or set a key for the customization
        /// </summary>
        [XmlAttribute("Key")]
        [NotMapped]
        public string Key { get; set; }

        /// <summary>
        ///     Get or set the input particle data
        /// </summary>
        [XmlElement("ParticleModel")]
        [ModelInputRoot(0)]
        [NotMapped]
        public ParticleModelGraph ParticleModelGraph { get; set; }

        /// <summary>
        ///     Get or set the input structure data
        /// </summary>
        [XmlElement("StructureModel")]
        [ModelInputRoot(1)]
        [NotMapped]
        public StructureModelGraph StructureModelGraph { get; set; }

        /// <summary>
        ///     Get or set the input transition data
        /// </summary>
        [XmlElement("TransitionModel")]
        [ModelInputRoot(2)]
        [NotMapped]
        public TransitionModelGraph TransitionModelGraph { get; set; }

        /// <summary>
        ///     Get or set the input energy data
        /// </summary>
        [XmlElement("EnergyModel")]
        [ModelInputRoot(3)]
        [NotMapped]
        public EnergyModelGraph EnergyModelGraph { get; set; }

        /// <summary>
        ///     Get or set the input simulation data
        /// </summary>
        [XmlElement("SimulationModel")]
        [ModelInputRoot(4)]
        [NotMapped]
        public SimulationModelGraph SimulationModelGraph { get; set; }

        /// <summary>
        ///     Get the full input sequence for the <see cref="Mocassin.Model.ModelProject.IModelProject" /> input process pipeline
        /// </summary>
        /// <returns></returns>
        public IEnumerable<object> GetInputSequence()
        {
            var dataRoots = GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Select(x => (x.GetValue(this) as ModelManagerGraph, x.GetCustomAttribute<ModelInputRootAttribute>()))
                .Where(x => x.Item2 != null && x.Item1 != null)
                .OrderBy(x => x.Item2.Order)
                .Select(x => x.Item1)
                .ToList();

            return dataRoots.SelectMany(x => x.GetInputSequence());
        }

        /// <summary>
        ///     Creates a new empty default <see cref="ProjectModelGraph"/>
        /// </summary>
        /// <returns></returns>
        public static ProjectModelGraph CreateNew()
        {
            return new ProjectModelGraph
            {
                Key = Guid.NewGuid().ToString(),
                ParticleModelGraph = new ParticleModelGraph(),
                StructureModelGraph = new StructureModelGraph(),
                EnergyModelGraph = new EnergyModelGraph(),
                TransitionModelGraph = new TransitionModelGraph(),
                SimulationModelGraph = new SimulationModelGraph()
            };
        }
    }
}