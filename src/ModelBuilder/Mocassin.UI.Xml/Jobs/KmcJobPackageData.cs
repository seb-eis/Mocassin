using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Serialization;
using Mocassin.Framework.Extensions;
using Mocassin.Model.ModelProject;
using Mocassin.Model.Simulations;
using Mocassin.Model.Translator.Jobs;
using Mocassin.UI.Data.Base;

namespace Mocassin.UI.Data.Jobs
{
    /// <summary>
    ///     Serializable data object for storage and provision of <see cref="KmcJobCollection" /> objects
    /// </summary>
    [XmlRoot]
    public class KmcJobPackageData : JobPackageData, IDuplicable<KmcJobPackageData>
    {
        private KmcJobConfigData jobBaseDescription;
        private ObservableCollection<KmcJobConfigData> jobConfigurations;
        private ModelObjectReference<KineticSimulation> simulation;

        /// <summary>
        ///     Get or set the <see cref="ModelObjectReference{T}" /> to the target <see cref="KineticSimulation" />
        /// </summary>
        [XmlElement]
        public ModelObjectReference<KineticSimulation> Simulation
        {
            get => simulation;
            set => SetProperty(ref simulation, value);
        }

        /// <summary>
        ///     Get or set the <see cref="KmcJobConfigData" /> that provides the default values for the config sequence
        /// </summary>
        [XmlElement]
        public KmcJobConfigData JobBaseDescription
        {
            get => jobBaseDescription;
            set => SetProperty(ref jobBaseDescription, value);
        }

        /// <summary>
        ///     Get or set the list of <see cref="KmcJobConfigData" /> objects of the collection
        /// </summary>
        [XmlArray]
        public ObservableCollection<KmcJobConfigData> JobConfigurations
        {
            get => jobConfigurations;
            set => SetProperty(ref jobConfigurations, value);
        }

        /// <inheritdoc />
        public KmcJobPackageData()
        {
            JobConfigurations = new ObservableCollection<KmcJobConfigData>();
            JobBaseDescription = new KmcJobConfigData();
        }

        /// <inheritdoc />
        public KmcJobPackageData Duplicate()
        {
            var result = new KmcJobPackageData
            {
                Simulation = Simulation?.Duplicate(),
                JobBaseDescription = JobBaseDescription.Duplicate(),
                JobConfigurations = JobConfigurations.Select(x => x.Duplicate()).ToObservableCollection()
            };
            CopyBaseDataTo(result);
            return result;
        }

        /// <inheritdoc />
        object IDuplicable.Duplicate() => Duplicate();

        /// <inheritdoc />
        public override IJobCollection ToInternal(IModelProject modelProject, int collectionId)
        {
            if (modelProject == null) throw new ArgumentNullException(nameof(modelProject));
            var result = JobPackageDataProvider.Create(modelProject, this);
            result.CollectionId = collectionId;
            return result;
        }

        /// <inheritdoc />
        public override IEnumerable<JobConfigData> GetConfigurations() => JobConfigurations.AsEnumerable();

        /// <inheritdoc />
        public override int GetTotalJobCount(IModelProject modelProject)
        {
            if (modelProject == null) throw new ArgumentNullException(nameof(modelProject));
            return JobConfigurations.Count * GetJobCountPerConfig(modelProject);
        }

        /// <summary>
        ///     Get the actual job count per configuration form the first priority level that defines value
        /// </summary>
        /// <param name="modelProject"></param>
        /// <returns></returns>
        public int GetJobCountPerConfig(IModelProject modelProject) =>
            int.TryParse(JobCountPerConfig, out var count)
                ? count
                : modelProject.DataTracker.FindObject<IKineticSimulation>(Simulation.Key).JobCount;
    }
}