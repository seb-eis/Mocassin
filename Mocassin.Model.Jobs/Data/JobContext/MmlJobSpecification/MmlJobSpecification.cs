using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Mocassin.Framework.Extensions;

namespace Mocassin.Model.Mml
{
    /// <summary>
    ///     Data class for user defined job specifications that enables data overwrites for the settings of the reference
    ///     simulation through serialized data
    /// </summary>
    [XmlRoot]
    public class MmlJobSpecification
    {
        /// <summary>
        ///     The set of job properties that should be overwritten
        /// </summary>
        [XmlElement("Property")]
        public HashSet<MmlJobProperty> JobProperties { get; set; }

        /// <summary>
        ///     Create empty job specifications
        /// </summary>
        public MmlJobSpecification()
        {
            JobProperties = new HashSet<MmlJobProperty>();
        }

        /// <summary>
        ///     Creates a new job specification from a sequence of job properties
        /// </summary>
        /// <param name="jobProperties"></param>
        public MmlJobSpecification(IEnumerable<MmlJobProperty> jobProperties)
            : this()
        {
            if (jobProperties == null)
                throw new ArgumentNullException(nameof(jobProperties));

            JobProperties.AddMany(jobProperties);
        }
    }
}