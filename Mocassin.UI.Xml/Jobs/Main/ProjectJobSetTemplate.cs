using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Xml.Serialization;
using Mocassin.Framework.Extensions;
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
    [XmlRoot]
    public sealed class ProjectJobSetTemplate : ProjectChildEntity<MocassinProject>, IDuplicable<ProjectJobSetTemplate>
    {
        private string key;
        private ObservableCollection<KmcJobPackageData> kmcJobPackageDescriptions;
        private ObservableCollection<MmcJobPackageData> mmcJobPackageDescriptions;

        /// <summary>
        ///     Get or set a key for the customization
        /// </summary>
        [XmlAttribute]
        [NotMapped]
        public string Key
        {
            get => key;
            set => SetProperty(ref key, value);
        }

        /// <summary>
        ///     Get or set the list of <see cref="KmcJobPackageData" /> that defines
        ///     <see cref="Mocassin.Model.Simulations.IKineticSimulation" /> database build instructions
        /// </summary>
        [XmlArray]
        [NotMapped]
        public ObservableCollection<KmcJobPackageData> KmcJobPackageDescriptions
        {
            get => kmcJobPackageDescriptions;
            set => SetProperty(ref kmcJobPackageDescriptions, value);
        }

        /// <summary>
        ///     Get or set the list of <see cref="MmcJobPackageData" /> that defines
        ///     <see cref="Mocassin.Model.Simulations.IMetropolisSimulation" /> database build instructions
        /// </summary>
        [XmlArray]
        [NotMapped]
        public ObservableCollection<MmcJobPackageData> MmcJobPackageDescriptions
        {
            get => mmcJobPackageDescriptions;
            set => SetProperty(ref mmcJobPackageDescriptions, value);
        }

        /// <summary>
        ///     Creates new <see cref="ProjectJobSetTemplate" /> with empty <see cref="JobPackageData" />
        ///     collections
        /// </summary>
        public ProjectJobSetTemplate()
        {
            KmcJobPackageDescriptions = new ObservableCollection<KmcJobPackageData>();
            MmcJobPackageDescriptions = new ObservableCollection<MmcJobPackageData>();
            Key = Guid.NewGuid().ToString();
        }

        /// <inheritdoc />
        public ProjectJobSetTemplate Duplicate()
        {
            var duplicate = new ProjectJobSetTemplate
            {
                Parent = Parent,
                Name = $"{Name}(copy)",
                KmcJobPackageDescriptions = KmcJobPackageDescriptions.Select(x => x.Duplicate()).ToObservableCollection(),
                MmcJobPackageDescriptions = MmcJobPackageDescriptions.Select(x => x.Duplicate()).ToObservableCollection()
            };
            return duplicate;
        }

        /// <inheritdoc />
        object IDuplicable.Duplicate()
        {
            return Duplicate();
        }

        /// <summary>
        ///     Get the sequence of <see cref="IJobCollection" /> objects defined in the object
        /// </summary>
        /// <param name="modelProject"></param>
        /// <returns></returns>
        public IEnumerable<IJobCollection> ToInternals(IModelProject modelProject)
        {
            var index = 0;
            foreach (var item in MmcJobPackageDescriptions.Concat(KmcJobPackageDescriptions.Cast<JobPackageData>()))
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
                .Cast<JobPackageData>()
                .Concat(MmcJobPackageDescriptions)
                .Sum(x => x.GetTotalJobCount(modelProject));
        }

        /// <summary>
        ///     Creates  anew <see cref="ProjectJobSetTemplate" /> that belongs to the passed parent <see cref="MocassinProject" />
        /// </summary>
        /// <param name="parentProject"></param>
        /// <returns></returns>
        public static ProjectJobSetTemplate Create(MocassinProject parentProject)
        {
            return new ProjectJobSetTemplate
            {
                Parent = parentProject,
                Name = $"Job translation [{parentProject.ProjectName}]"
            };
        }
    }
}