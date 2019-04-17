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
    public class MmcJobPackageDescriptionGraph : JobPackageDescriptionGraph
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
        public override IJobCollection ToInternal(IModelProject modelProject)
        {
            if (modelProject == null) throw new ArgumentNullException(nameof(modelProject));
            return JobCollectionAdapter.Create(modelProject, this);
        }

        /// <inheritdoc />
        public override IEnumerable<JobDescriptionGraph> GetConfigurations()
        {
            return JobConfigurations.AsEnumerable();
        }
    }
}