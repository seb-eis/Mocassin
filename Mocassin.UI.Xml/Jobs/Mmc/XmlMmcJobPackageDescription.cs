using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Mocassin.Model.ModelProject;
using Mocassin.Model.Simulations;
using Mocassin.Model.Translator.Jobs;

namespace Mocassin.UI.Xml.Jobs
{
    /// <summary>
    ///     Serializable data object for storage and provision of <see cref="MmcJobCollection" /> objects
    /// </summary>
    [XmlRoot("MmcJobCollection")]
    public class XmlMmcJobPackageDescription : XmlJobPackageDescription
    {
        /// <summary>
        ///     Get or set the <see cref="XmlMmcJobDescription" /> that provides the default values for the config sequence
        /// </summary>
        [XmlElement("JobBaseConfiguration")]
        public XmlMmcJobDescription JobBaseDescription { get; set; }

        /// <summary>
        ///     Get or set the list of <see cref="XmlMmcJobDescription" /> objects of the collection
        /// </summary>
        [XmlArray("JobConfigurations")]
        [XmlArrayItem("JobConfiguration")]
        public List<XmlMmcJobDescription> JobConfigurations { get; set; }

        /// <inheritdoc />
        public override IJobCollection ToInternal(IModelProject modelProject)
        {
            if (modelProject == null) throw new ArgumentNullException(nameof(modelProject));
            return XmlJobCollectionAdapter.Create(modelProject, this);
        }

        /// <inheritdoc />
        public override IEnumerable<XmlJobDescription> GetConfigurations()
        {
            return JobConfigurations.AsEnumerable();
        }
    }
}