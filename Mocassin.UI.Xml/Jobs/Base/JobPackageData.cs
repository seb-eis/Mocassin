using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Serialization;
using Mocassin.Framework.Extensions;
using Mocassin.Model.ModelProject;
using Mocassin.Model.Translator.Jobs;
using Mocassin.UI.Xml.Base;

namespace Mocassin.UI.Xml.Jobs
{
    /// <summary>
    ///     Serializable data object base for the creation of <see cref="Mocassin.Model.Translator.Jobs.IJobCollection" />
    ///     database creation instructions
    /// </summary>
    [XmlRoot]
    public abstract class JobPackageData : ProjectDataObject
    {
        private string rngSeed;
        private string jobCountPerConfig;
        private ObservableCollection<SelectionOptimizerData> selectionOptimizers;

        /// <summary>
        ///     Get or set an rng seed string to overwrite the one defined in the affiliated
        ///     <see cref="Mocassin.Model.Simulations.ISimulation" />
        /// </summary>
        [XmlAttribute]
        public string RngSeed
        {
            get => rngSeed;
            set => SetProperty(ref rngSeed, value);
        }

        /// <summary>
        ///     Get or set the job count multiplier per <see cref="JobConfigData" /> that is defined in the collection
        /// </summary>
        [XmlAttribute]
        public string JobCountPerConfig
        {
            get => jobCountPerConfig;
            set => SetProperty(ref jobCountPerConfig, value);
        }

        /// <summary>
        ///     Get or set the list of <see cref="SelectionOptimizerData" /> objects
        /// </summary>
        [XmlArray]
        public ObservableCollection<SelectionOptimizerData> SelectionOptimizers
        {
            get => selectionOptimizers;
            set => SetProperty(ref selectionOptimizers, value);
        }

        /// <inheritdoc />
        protected JobPackageData()
        {
            SelectionOptimizers = new ObservableCollection<SelectionOptimizerData>();
            RngSeed = Guid.NewGuid().ToString();
        }

        /// <summary>
        ///     Creates a <see cref="IJobCollection" /> for simulation database creation in the context of the passed
        ///     <see cref="IModelProject" /> and assigns the passed collection id
        /// </summary>
        /// <param name="modelProject"></param>
        /// <param name="collectionId"></param>
        /// <returns></returns>
        public abstract IJobCollection ToInternal(IModelProject modelProject, int collectionId);

        /// <summary>
        ///     Get the sequence of defined <see cref="ManualOptimizerData" /> objects pof the package
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<ManualOptimizerData> GetManualOptimizers()
        {
            return SelectionOptimizers;
        }

        /// <summary>
        ///     Get the sequence of defined <see cref="JobConfigData" /> objects of the package
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerable<JobConfigData> GetConfigurations();

        /// <summary>
        ///     Calculate the total number of executable simulations in the context of the passed <see cref="IModelProject" />
        ///     defined by the package
        /// </summary>
        /// <returns></returns>
        public abstract int GetTotalJobCount(IModelProject modelProject);

        /// <summary>
        ///     Copies internal data to the passed <see cref="JobPackageData" />
        /// </summary>
        /// <param name="other"></param>
        protected void CopyBaseDataTo(JobPackageData other)
        {
            other.Name = Name;
            other.JobCountPerConfig = JobCountPerConfig;
            other.SelectionOptimizers = SelectionOptimizers.Select(x => x.Duplicate()).ToObservableCollection();
        }
    }
}