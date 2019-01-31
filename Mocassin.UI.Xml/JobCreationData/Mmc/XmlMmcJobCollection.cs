using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Mocassin.Model.ModelProject;
using Mocassin.Model.Translator.Jobs;

namespace Mocassin.UI.Xml.JobCreationData.Kmc
{
    /// <summary>
    ///     Serializable data object for storage and provision of <see cref="MmcJobCollection" /> objects
    /// </summary>
    [XmlRoot("MmcJobCollection")]
    public class XmlMmcJobCollection : XmlJobCollection
    {
        /// <summary>
        ///     Get or set the <see cref="XmlMmcJobConfiguration" /> that provides the default values for the config sequence
        /// </summary>
        [XmlElement("JobBaseConfig")]
        public XmlMmcJobConfiguration JobBaseConfiguration { get; set; }

        /// <summary>
        ///     Get or set the list of <see cref="XmlMmcJobConfiguration" /> objects of the collection
        /// </summary>
        [XmlArray("JobConfigurations")]
        [XmlArrayItem("JobConfig")]
        public List<XmlMmcJobConfiguration> JobConfigurations { get; set; }

        /// <inheritdoc />
        public override IJobCollection ToInternal(IModelProject modelProject)
        {
            throw new NotImplementedException();
        }
    }
}