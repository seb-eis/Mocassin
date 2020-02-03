using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Serialization;
using Mocassin.Framework.Extensions;
using Mocassin.Model.ModelProject;
using Mocassin.Model.Simulations;
using Mocassin.Model.Translator.Jobs;
using Mocassin.UI.Xml.Base;

namespace Mocassin.UI.Xml.Jobs
{
    /// <summary>
    ///     Serializable data object for storage and provision of <see cref="MmcJobCollection" /> objects
    /// </summary>
    [XmlRoot]
    public class MmcJobPackageData : JobPackageData, IDuplicable<MmcJobPackageData>
    {
        private ModelObjectReference<MetropolisSimulation> simulation;
        private MmcJobConfigData jobBaseDescription;
        private ObservableCollection<MmcJobConfigData> jobConfigurations;

        /// <summary>
        ///     Get or set the <see cref="ModelObjectReference{T}" /> to the target <see cref="MetropolisSimulation" />
        /// </summary>
        [XmlElement]
        public ModelObjectReference<MetropolisSimulation> Simulation
        {
            get => simulation;
            set => SetProperty(ref simulation, value);
        }

        /// <summary>
        ///     Get or set the <see cref="MmcJobConfigData" /> that provides the default values for the config sequence
        /// </summary>
        [XmlElement]
        public MmcJobConfigData JobBaseDescription
        {
            get => jobBaseDescription;
            set => SetProperty(ref jobBaseDescription, value);
        }

        /// <summary>
        ///     Get or set the list of <see cref="MmcJobConfigData" /> objects of the collection
        /// </summary>
        [XmlArray]
        public ObservableCollection<MmcJobConfigData> JobConfigurations
        {
            get => jobConfigurations;
            set => SetProperty(ref jobConfigurations, value);
        }

        /// <inheritdoc />
        public MmcJobPackageData()
        {
            JobConfigurations = new ObservableCollection<MmcJobConfigData>();
            JobBaseDescription = new MmcJobConfigData();
        }

        /// <inheritdoc />
        public override IJobCollection ToInternal(IModelProject modelProject, int collectionId)
        {
            if (modelProject == null) throw new ArgumentNullException(nameof(modelProject));
            var result = JobPackageDataProvider.Create(modelProject, this);
            result.CollectionId = collectionId;
            return result;
        }

        /// <inheritdoc />
        public override IEnumerable<JobConfigData> GetConfigurations()
        {
            return JobConfigurations.AsEnumerable();
        }

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
        public int GetJobCountPerConfig(IModelProject modelProject)
        {
            return int.TryParse(JobCountPerConfig, out var count)
                ? count
                : modelProject.DataTracker.FindObjectByKey<IMetropolisSimulation>(Simulation.Key).JobCount;
        }

        /// <inheritdoc />
        public MmcJobPackageData Duplicate()
        {
            var result = new MmcJobPackageData
            {
                Simulation = Simulation?.Duplicate(),
                JobBaseDescription = JobBaseDescription.Duplicate(),
                JobConfigurations = JobConfigurations.Select(x => x.Duplicate()).ToObservableCollection()
            };
            CopyBaseDataTo(result);
            return result;
        }

        /// <inheritdoc />
        object IDuplicable.Duplicate()
        {
            return Duplicate();
        }
    }
}