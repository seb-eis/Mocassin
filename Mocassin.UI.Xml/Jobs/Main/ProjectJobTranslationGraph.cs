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
    public sealed class ProjectJobTranslationGraph : MocassinProjectChildEntity<MocassinProjectGraph>
    {
        private string key;
        private List<KmcJobPackageDescriptionGraph> kmcJobPackageDescriptions;
        private List<MmcJobPackageDescriptionGraph> mmcJobPackageDescriptions;

        /// <summary>
        ///     Get or set a key for the customization
        /// </summary>
        [XmlAttribute("Key")]
        [NotMapped]
        public string Key
        {
            get => key;
            set => SetProperty(ref key, value);
        }

        /// <summary>
        ///     Get or set the list of <see cref="KmcJobPackageDescriptionGraph" /> that defines
        ///     <see cref="Mocassin.Model.Simulations.IKineticSimulation" /> database build instructions
        /// </summary>
        [XmlArray("KmcJobPackages")]
        [XmlArrayItem("JobPackage")]
        [NotMapped]
        public List<KmcJobPackageDescriptionGraph> KmcJobPackageDescriptions
        {
            get => kmcJobPackageDescriptions;
            set => SetProperty(ref kmcJobPackageDescriptions, value);
        }

        /// <summary>
        ///     Get or set the list of <see cref="MmcJobPackageDescriptionGraph" /> that defines
        ///     <see cref="Mocassin.Model.Simulations.IMetropolisSimulation" /> database build instructions
        /// </summary>
        [XmlArray("MmcJobPackages")]
        [XmlArrayItem("JobPackage")]
        [NotMapped]
        public List<MmcJobPackageDescriptionGraph> MmcJobPackageDescriptions
        {
            get => mmcJobPackageDescriptions;
            set => SetProperty(ref mmcJobPackageDescriptions, value);
        }

        /// <summary>
        ///     Creates new <see cref="ProjectJobTranslationGraph" /> with empty <see cref="JobPackageDescriptionGraph" />
        ///     collections
        /// </summary>
        public ProjectJobTranslationGraph()
        {
            KmcJobPackageDescriptions = new List<KmcJobPackageDescriptionGraph>();
            MmcJobPackageDescriptions = new List<MmcJobPackageDescriptionGraph>();
            Key = Guid.NewGuid().ToString();
            Name = "New translation set";
        }

        /// <summary>
        ///     Get the sequence of <see cref="IJobCollection" /> objects defined in the object
        /// </summary>
        /// <param name="modelProject"></param>
        /// <returns></returns>
        public IEnumerable<IJobCollection> ToInternals(IModelProject modelProject)
        {
            var index = 0;
            foreach (var item in MmcJobPackageDescriptions.Concat(KmcJobPackageDescriptions.Cast<JobPackageDescriptionGraph>()))
                yield return item.ToInternal(modelProject, index++);
        }

        /// <summary>
        ///     Calculates the total number simulation configurations of the object defined within the context of the passed
        ///     <see cref="IModelProject" />
        /// </summary>
        /// <param name="modelProject"></param>
        /// <returns></returns>
        public int GetTotalJobCount(IModelProject modelProject)
        {
            if (modelProject == null) throw new ArgumentNullException(nameof(modelProject));
            return KmcJobPackageDescriptions
                .Cast<JobPackageDescriptionGraph>()
                .Concat(MmcJobPackageDescriptions)
                .Sum(x => x.GetTotalJobCount(modelProject));
        }
    }
}