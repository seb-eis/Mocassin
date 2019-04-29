using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Mocassin.Model.ModelProject;
using Mocassin.Model.Translator.Jobs;
using Mocassin.UI.Xml.Base;
using Mocassin.UI.Xml.SimulationModel;
using Newtonsoft.Json;

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

        /// <inheritdoc />
        protected JobPackageDescriptionGraph()
        {
            ManualOptimizers = new List<ManualOptimizerGraph>();
            RngSeed = Guid.NewGuid().ToString();
        }

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

        /// <summary>
        ///     Calculate the total number of executable simulations in the context of the passed <see cref="IModelProject"/> defined by the package
        /// </summary>
        /// <returns></returns>
        public abstract int GetTotalJobCount(IModelProject modelProject);
    }
}