﻿using System;
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
    ///     Serializable data object for storage and provision of <see cref="KmcJobCollection" /> objects
    /// </summary>
    [XmlRoot("KmcJobCollection")]
    public class KmcJobPackageDescriptionGraph : JobPackageDescriptionGraph
    {
        /// <summary>
        ///     Get or set the <see cref="ModelObjectReferenceGraph{T}" /> to the target <see cref="KineticSimulation" />
        /// </summary>
        [XmlElement("BaseSimulation")]
        public ModelObjectReferenceGraph<KineticSimulation> Simulation { get; set; }

        /// <summary>
        ///     Get or set the <see cref="KmcJobDescriptionGraph" /> that provides the default values for the config sequence
        /// </summary>
        [XmlElement("JobBaseConfiguration")]
        public KmcJobDescriptionGraph JobBaseDescription { get; set; }

        /// <summary>
        ///     Get or set the list of <see cref="KmcJobDescriptionGraph" /> objects of the collection
        /// </summary>
        [XmlArray("JobConfigurations")]
        [XmlArrayItem("JobConfiguration")]
        public List<KmcJobDescriptionGraph> JobConfigurations { get; set; }

        /// <inheritdoc />
        public KmcJobPackageDescriptionGraph()
        {
            JobConfigurations = new List<KmcJobDescriptionGraph>();
            JobBaseDescription = new KmcJobDescriptionGraph();
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
    }
}