using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Mocassin.Model.ModelProject;
using Mocassin.Model.Translator.Jobs;

namespace Mocassin.UI.Xml.CreationData
{
    /// <summary>
    ///     Serializable data object for storage and provision of <see cref="KmcJobCollection" /> objects
    /// </summary>
    [XmlRoot("KmcJobCollection")]
    public class XmlKmcJobPackageDescription : XmlJobPackageDescription
    {
        /// <summary>
        ///     Get or set the <see cref="XmlKmcJobDescription" /> that provides the default values for the config sequence
        /// </summary>
        [XmlElement("JobBaseConfiguration")]
        public XmlKmcJobDescription JobBaseDescription { get; set; }

        /// <summary>
        ///     Get or set the list of <see cref="XmlKmcJobDescription" /> objects of the collection
        /// </summary>
        [XmlArray("JobConfigurations")]
        [XmlArrayItem("JobConfiguration")]
        public List<XmlKmcJobDescription> JobConfigurations { get; set; }

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