using System.Collections.Generic;
using System.Xml.Serialization;
using Mocassin.Model.ModelProject;
using Mocassin.Model.Translator.Jobs;
using Mocassin.UI.Xml.Base;

namespace Mocassin.UI.Xml.Jobs
{
    /// <summary>
    ///     Serializable data object base for the creation of <see cref="Mocassin.Model.Translator.Jobs.IJobCollection" />
    ///     database creation instructions
    /// </summary>
    [XmlRoot]
    public abstract class JobPackageDescriptionGraph : ProjectObjectGraph
    {
        /// <summary>
        ///     Get or set the key of the <see cref="Mocassin.Model.Simulations.ISimulation" /> that the collection is for
        /// </summary>
        [XmlAttribute("BaseSimulation")]
        public string SimulationKey { get; set; }

        /// <summary>
        ///     Get or set an rng seed string to overwrite the one defined in the affiliated
        ///     <see cref="Mocassin.Model.Simulations.ISimulation" />
        /// </summary>
        [XmlAttribute("RngSeed")]
        public string RngSeed { get; set; }

        /// <summary>
        ///     Get or set the job count multiplier per <see cref="JobDescriptionGraph" /> that is defined in the collection
        /// </summary>
        [XmlAttribute("JobCount")]
        public string JobCountPerConfig { get; set; }

        /// <summary>
        ///     Get or set the list of <see cref="ManualOptimizerGraph" /> objects
        /// </summary>
        [XmlArray("ManualOptimizers")]
        [XmlArrayItem(typeof(SelectionOptimizerGraph), ElementName = "SelectionOptimizer")]
        public List<ManualOptimizerGraph> ManualOptimizers { get; set; }

        /// <summary>
        ///     Creates a <see cref="IJobCollection" /> for simulation database creation in the context of the passed
        ///     <see cref="IModelProject" />
        /// </summary>
        /// <param name="modelProject"></param>
        /// <returns></returns>
        public abstract IJobCollection ToInternal(IModelProject modelProject);

        /// <summary>
        ///     Get the sequence of defined <see cref="JobDescriptionGraph" /> objects of the collection
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerable<JobDescriptionGraph> GetConfigurations();
    }
}