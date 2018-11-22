using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Mocassin.Model.Jobs
{
    /// <summary>
    ///     A job package that bundles multiple job specifications with global definitions for a full set of simulations, e.g.
    ///     a special energy set
    /// </summary>
    [XmlRoot("JobPackage")]
    public class JobPackage
    {
        /// <summary>
        ///     The simulation id that the package is based upon
        /// </summary>
        [XmlAttribute("SimulationId")]
        public string SimulationId { get; set; }

        /// <summary>
        ///     The list of global job property overwrites of this package
        /// </summary>
        [XmlElement("JobProperty", Namespace = "PackageNamespace")]
        public HashSet<JobProperty> GlobalJobProperties { get; set; }

        /// <summary>
        ///     The list of job specifications in the job package that are bound to the same set of reference data
        /// </summary>
        [XmlElement("JobSpecification")]
        public List<JobSpecification> JobSpecifications { get; set; }

        /// <summary>
        ///     Create new empty job properties
        /// </summary>
        public JobPackage()
        {
        }

        /// <summary>
        ///     Creates new job package with simulation id and sequences of global job properties and job specifications
        /// </summary>
        /// <param name="simulationId"></param>
        /// <param name="globalJobProperties"></param>
        /// <param name="jobSpecifications"></param>
        public JobPackage(string simulationId, IEnumerable<JobProperty> globalJobProperties,
            IEnumerable<JobSpecification> jobSpecifications)
        {
            SimulationId = simulationId ?? throw new ArgumentNullException(nameof(simulationId));
            JobSpecifications = jobSpecifications?.ToList() ?? new List<JobSpecification>();
            GlobalJobProperties = new HashSet<JobProperty>(globalJobProperties ?? new List<JobProperty>());
        }
    }
}