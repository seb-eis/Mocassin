using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Xml.Serialization;
using Mocassin.Model.ModelProject;
using Mocassin.Model.Translator.Jobs;
using Mocassin.UI.Xml.Base;
using Mocassin.UI.Xml.Main;

namespace Mocassin.UI.Xml.Jobs
{
    /// <summary>
    ///     Main serializable data object to provide multiple <see cref="IJobCollection" /> objects to the simulation database
    ///     creation system
    /// </summary>
    [XmlRoot("DatabaseCreationInstruction")]
    public class ProjectJobTranslationGraph : MocassinProjectChildEntity<MocassinProjectGraph>
    {
        /// <summary>
        ///     Get or set a key for the customization
        /// </summary>
        [XmlAttribute("Key")]
        [NotMapped]
        public string Key { get; set; }

        /// <summary>
        ///     Get or set the list of <see cref="KmcJobPackageDescriptionGraph" /> that defines
        ///     <see cref="Mocassin.Model.Simulations.IKineticSimulation" /> database build instructions
        /// </summary>
        [XmlArray("KmcJobPackages")]
        [XmlArrayItem("JobPackage")]
        [NotMapped]
        public List<KmcJobPackageDescriptionGraph> KmcJobPackageDescriptions { get; set; }

        /// <summary>
        ///     Get or set the list of <see cref="MmcJobPackageDescriptionGraph" /> that defines
        ///     <see cref="Mocassin.Model.Simulations.IMetropolisSimulation" /> database build instructions
        /// </summary>
        [XmlArray("MmcJobPackages")]
        [XmlArrayItem("JobPackage")]
        [NotMapped]
        public List<MmcJobPackageDescriptionGraph> MmcJobPackageDescriptions { get; set; }

        /// <summary>
        ///     Creates new <see cref="ProjectJobTranslationGraph"/> with empty <see cref="JobPackageDescriptionGraph"/> collections
        /// </summary>
        public ProjectJobTranslationGraph()
        {
            KmcJobPackageDescriptions = new List<KmcJobPackageDescriptionGraph>();
            MmcJobPackageDescriptions = new List<MmcJobPackageDescriptionGraph>();
            Key = Guid.NewGuid().ToString();
        }

        /// <summary>
        ///     Get the sequence of <see cref="IJobCollection" /> objects defined in the object
        /// </summary>
        /// <param name="modelProject"></param>
        /// <returns></returns>
        public IEnumerable<IJobCollection> ToInternals(IModelProject modelProject)
        {
            return MmcJobPackageDescriptions.Select(x => x.ToInternal(modelProject))
                .Concat(KmcJobPackageDescriptions.Select(x => x.ToInternal(modelProject)));
        }
    }
}