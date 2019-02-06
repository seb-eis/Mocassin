using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Mocassin.Model.ModelProject;
using Mocassin.Model.Translator;
using Mocassin.Model.Translator.EntityBuilder;
using Mocassin.Model.Translator.Jobs;
using Mocassin.Model.Translator.ModelContext;

namespace Mocassin.UI.Xml.CreationData
{
    /// <summary>
    ///     Main serializable data object to provide multiple <see cref="IJobCollection" /> objects to the simulation database
    ///     creation system
    /// </summary>
    [XmlRoot("DatabaseCreationInstruction")]
    public class XmlDbCreationInstruction
    {
        /// <summary>
        ///     Get or set the list of <see cref="XmlKmcJobPackageDescription" /> that defines
        ///     <see cref="Mocassin.Model.Simulations.IKineticSimulation" /> database build instructions
        /// </summary>
        [XmlArray("KmcJobPackages")]
        [XmlArrayItem("JobPackage")]
        public List<XmlKmcJobPackageDescription> KmcJobPackageDescriptions { get; set; }

        /// <summary>
        ///     Get or set the list of <see cref="XmlMmcJobPackageDescription" /> that defines
        ///     <see cref="Mocassin.Model.Simulations.IMetropolisSimulation" /> database build instructions
        /// </summary>
        [XmlArray("MmcJobPackages")]
        [XmlArrayItem("JobPackage")]
        public List<XmlMmcJobPackageDescription> MmcJobPackageDescriptions { get; set; }

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