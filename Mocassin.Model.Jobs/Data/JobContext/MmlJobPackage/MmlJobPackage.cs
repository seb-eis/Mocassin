using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Mocassin.Model.Mml
{
    /// <summary>
    ///     A job package that bundles multiple job specifications with global definitions for a full set of simulations, e.g.
    ///     a special energy set
    /// </summary>
    [XmlRoot("JobPackage")]
    public class MmlJobPackage
    {
        /// <summary>
        ///     Get or set the simulation id that the package is based upon
        /// </summary>
        [XmlAttribute("SimulationId")]
        public string SimulationId { get; set; }

        /// <summary>
        ///     Get or set the list of global job property overwrites of this package
        /// </summary>
        [XmlElement("JobProperty")]
        public HashSet<MmlJobProperty> GlobalJobProperties { get; set; }

        /// <summary>
        ///     Get or set the energy model specification that the package should use
        /// </summary>
        [XmlElement("EnergySpecifications")]
        public MmlEnergySpecification MmlEnergySpecification { get; set; }

        /// <summary>
        ///     Get or set the list of job specifications in the job package that are bound to the same set of reference data
        /// </summary>
        [XmlElement("JobSpecification")]
        public List<MmlJobSpecification> JobSpecifications { get; set; }

        /// <summary>
        ///     Create new empty job properties
        /// </summary>
        public MmlJobPackage()
        {
        }

        /// <summary>
        ///     Creates new job package with simulation id and sequences of global job properties and job specifications
        /// </summary>
        /// <param name="simulationId"></param>
        /// <param name="globalJobProperties"></param>
        /// <param name="jobSpecifications"></param>
        public MmlJobPackage(string simulationId, IEnumerable<MmlJobProperty> globalJobProperties,
            IEnumerable<MmlJobSpecification> jobSpecifications)
        {
            SimulationId = simulationId ?? throw new ArgumentNullException(nameof(simulationId));
            JobSpecifications = jobSpecifications?.ToList() ?? new List<MmlJobSpecification>();
            GlobalJobProperties = new HashSet<MmlJobProperty>(globalJobProperties ?? new List<MmlJobProperty>());
        }
    }
}