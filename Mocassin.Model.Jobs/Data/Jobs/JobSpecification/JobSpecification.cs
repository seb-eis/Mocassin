using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Mocassin.Framework.Extensions;

namespace Mocassin.Model.Jobs
{
    /// <summary>
    ///     Data class for user defined job specifications that enables data overwrites for the settings of the reference
    ///     simulation through serialized data
    /// </summary>
    [XmlRoot]
    public class JobSpecification
    {
        /// <summary>
        ///     The set of job properties that should be overwritten
        /// </summary>
        [XmlElement("Property")]
        public HashSet<JobProperty> JobProperties { get; set; }

        /// <summary>
        ///     Create empty job specifications
        /// </summary>
        public JobSpecification()
        {
            JobProperties = new HashSet<JobProperty>();
        }

        /// <summary>
        ///     Creates a new job specification from a sequence of job properties
        /// </summary>
        /// <param name="jobProperties"></param>
        public JobSpecification(IEnumerable<JobProperty> jobProperties)
            : this()
        {
            if (jobProperties == null)
                throw new ArgumentNullException(nameof(jobProperties));

            JobProperties.AddMany(jobProperties);
        }
    }
}