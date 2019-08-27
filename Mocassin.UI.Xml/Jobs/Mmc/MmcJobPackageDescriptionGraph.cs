using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Mocassin.Model.ModelProject;
using Mocassin.Model.Simulations;
using Mocassin.Model.Translator.Jobs;
using Mocassin.UI.Xml.Base;

namespace Mocassin.UI.Xml.Jobs
{
    /// <summary>
    ///     Serializable data object for storage and provision of <see cref="MmcJobCollection" /> objects
    /// </summary>
    [XmlRoot("MmcJobCollection")]
    public class MmcJobPackageDescriptionGraph : JobPackageDescriptionGraph, IDuplicable<MmcJobPackageDescriptionGraph>
    {
        /// <summary>
        ///     Get or set the <see cref="ModelObjectReferenceGraph{T}" /> to the target <see cref="MetropolisSimulation" />
        /// </summary>
        [XmlElement("BaseSimulation")]
        public ModelObjectReferenceGraph<MetropolisSimulation> Simulation { get; set; }

        /// <summary>
        ///     Get or set the <see cref="MmcJobDescriptionGraph" /> that provides the default values for the config sequence
        /// </summary>
        [XmlElement("JobBaseConfiguration")]
        public MmcJobDescriptionGraph JobBaseDescription { get; set; }

        /// <summary>
        ///     Get or set the list of <see cref="MmcJobDescriptionGraph" /> objects of the collection
        /// </summary>
        [XmlArray("JobConfigurations")]
        [XmlArrayItem("JobConfiguration")]
        public List<MmcJobDescriptionGraph> JobConfigurations { get; set; }

        /// <inheritdoc />
        public MmcJobPackageDescriptionGraph()
        {
            JobConfigurations = new List<MmcJobDescriptionGraph>();
            JobBaseDescription = new MmcJobDescriptionGraph();
        }

        /// <inheritdoc />
        public override IJobCollection ToInternal(IModelProject modelProject, int collectionId)
        {
            if (modelProject == null) throw new ArgumentNullException(nameof(modelProject));
            var result = JobCollectionAdapter.Create(modelProject, this);
            result.CollectionId = collectionId;
            return result;
        }

        /// <inheritdoc />
        public override IEnumerable<JobDescriptionGraph> GetConfigurations()
        {
            return JobConfigurations.AsEnumerable();
        }

        /// <inheritdoc />
        public override int GetTotalJobCount(IModelProject modelProject)
        {
            if (modelProject == null) throw new ArgumentNullException(nameof(modelProject));
            return JobConfigurations.Count * GetJobCountPerConfig(modelProject);
        }

        /// <summary>
        ///     Get the actual job count per configuration form the first priority level that defines value
        /// </summary>
        /// <param name="modelProject"></param>
        /// <returns></returns>
        public int GetJobCountPerConfig(IModelProject modelProject)
        {
            return int.TryParse(JobCountPerConfig, out var count)
                ? count
                : modelProject.DataTracker.FindObjectByKey<IMetropolisSimulation>(Simulation.Key).JobCount;
        }

        /// <inheritdoc />
        public MmcJobPackageDescriptionGraph Duplicate()
        {
            var result = new MmcJobPackageDescriptionGraph
            {
                Simulation = Simulation?.Duplicate(),
                JobBaseDescription = JobBaseDescription.Duplicate(),
                JobConfigurations = JobConfigurations.Select(x => x.Duplicate()).ToList()
            };
            CopyBaseDataTo(result);
            return result;
        }

        /// <inheritdoc />
        object IDuplicable.Duplicate()
        {
            return Duplicate();
        }
    }
}