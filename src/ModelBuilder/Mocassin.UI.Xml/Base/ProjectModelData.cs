using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using Mocassin.UI.Data.EnergyModel;
using Mocassin.UI.Data.LatticeModel;
using Mocassin.UI.Data.Main;
using Mocassin.UI.Data.ParticleModel;
using Mocassin.UI.Data.SimulationModel;
using Mocassin.UI.Data.StructureModel;
using Mocassin.UI.Data.TransitionModel;

namespace Mocassin.UI.Data.Base
{
    /// <summary>
    ///     The main root for mocassin project data that targets <see cref="Mocassin.Model.Basic.IModelManager" /> through the
    ///     <see cref="Mocassin.Model.ModelProject.IProjectInputPipeline" /> interface
    /// </summary>
    [XmlRoot]
    public class ProjectModelData : ProjectChildEntity<MocassinProject>
    {
        /// <summary>
        ///     Get or set a key for the customization
        /// </summary>
        [XmlAttribute, NotMapped]
        public string Key { get; set; }

        /// <summary>
        ///     Get or set the input particle data
        /// </summary>
        [XmlElement, ModelInputRoot(0), NotMapped]
        public ParticleModelData ParticleModelData { get; set; }

        /// <summary>
        ///     Get or set the input structure data
        /// </summary>
        [XmlElement, ModelInputRoot(1), NotMapped]
        public StructureModelData StructureModelData { get; set; }

        /// <summary>
        ///     Get or set the input transition data
        /// </summary>
        [XmlElement, ModelInputRoot(2), NotMapped]
        public TransitionModelData TransitionModelData { get; set; }

        /// <summary>
        ///     Get or set the input energy data
        /// </summary>
        [XmlElement, ModelInputRoot(3), NotMapped]
        public EnergyModelData EnergyModelData { get; set; }

        /// <summary>
        ///     Get or set the input lattice data
        /// </summary>
        [XmlElement, ModelInputRoot(4), NotMapped]
        public LatticeModelData LatticeModelData { get; set; }

        /// <summary>
        ///     Get or set the input simulation data
        /// </summary>
        [XmlElement, ModelInputRoot(5), NotMapped]
        public SimulationModelData SimulationModelData { get; set; }

        /// <summary>
        ///     Get the full input sequence for the <see cref="Mocassin.Model.ModelProject.IModelProject" /> input process pipeline
        /// </summary>
        /// <returns></returns>
        public IEnumerable<object> GetInputSequence()
        {
            var dataRoots = GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                                     .Select(x => (x.GetValue(this) as ModelManagerData, x.GetCustomAttribute<ModelInputRootAttribute>()))
                                     .Where(x => x.Item2 != null && x.Item1 != null)
                                     .OrderBy(x => x.Item2.Order)
                                     .Select(x => x.Item1)
                                     .ToList();

            return dataRoots.SelectMany(x => x.GetInputSequence());
        }

        /// <summary>
        ///     Creates a new empty default <see cref="ProjectModelData" /> without setting the parent
        /// </summary>
        /// <returns></returns>
        public static ProjectModelData CreateNew() =>
            new ProjectModelData
            {
                Key = Guid.NewGuid().ToString(),
                ParticleModelData = new ParticleModelData(),
                StructureModelData = new StructureModelData(),
                EnergyModelData = new EnergyModelData(),
                TransitionModelData = new TransitionModelData(),
                LatticeModelData = new LatticeModelData(),
                SimulationModelData = new SimulationModelData()
            };
    }
}