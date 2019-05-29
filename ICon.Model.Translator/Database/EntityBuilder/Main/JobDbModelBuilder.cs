using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mocassin.Framework.Events;
using Mocassin.Framework.Extensions;
using Mocassin.Model.Translator.Database.Entities.Other.Meta;
using Mocassin.Model.Translator.Jobs;
using Mocassin.Model.Translator.ModelContext;
using Mocassin.Model.Translator.Optimization;

namespace Mocassin.Model.Translator.EntityBuilder
{
    /// <inheritdoc />
    public class JobDbEntityBuilder : IJobDbEntityBuilder
    {
        /// <summary>
        ///     Get the set of <see cref="IPostBuildOptimizer" /> interfaces registered with the builder
        /// </summary>
        private HashSet<IPostBuildOptimizer> PostBuildOptimizers { get; }

        /// <summary>
        ///     Get the <see cref="ReactiveEvent{TSubject}" /> that is called on finished jobs
        /// </summary>
        private ReactiveEvent<int> JobIsBuildEvent { get; }

        /// <inheritdoc />
        public IObservable<int> WhenJobIsBuild => JobIsBuildEvent.AsObservable();

        /// <inheritdoc />
        public IProjectModelContext ProjectModelContext { get; set; }

        /// <summary>
        ///     Get or set the energy db model builder
        /// </summary>
        public IEnergyDbEntityBuilder EnergyDbEntityBuilder { get; set; }

        /// <summary>
        ///     Get or set the structure db model builder
        /// </summary>
        public IStructureDbEntityBuilder StructureDbEntityBuilder { get; set; }

        /// <summary>
        ///     Get or set the transition db model builder
        /// </summary>
        public ITransitionDbEntityBuilder TransitionDbEntityBuilder { get; set; }

        /// <summary>
        ///     Get or set the lattice db model builder
        /// </summary>
        public ILatticeDbEntityBuilder LatticeDbEntityBuilder { get; set; }

        /// <summary>
        ///     Create new db model builder that uses the passed model context
        /// </summary>
        /// <param name="projectModelContext"></param>
        public JobDbEntityBuilder(IProjectModelContext projectModelContext)
        {
            ProjectModelContext = projectModelContext ?? throw new ArgumentNullException(nameof(projectModelContext));
            PostBuildOptimizers = new HashSet<IPostBuildOptimizer>();
            JobIsBuildEvent = new ReactiveEvent<int>();
        }

        /// <inheritdoc />
        public SimulationJobPackageModel BuildJobPackageModel(IJobCollection jobCollection)
        {
            PrepareBuildComponents();

            var simulationModel = ProjectModelContext.SimulationModelContext.FindSimulationModel(jobCollection.GetSimulation());
            if (simulationModel == null)
                throw new ArgumentException("Simulation cannot be found in the project model context");

            var packageModel = CreatePackageModel(simulationModel);
            var jobModelTasks = GetJobModelBuildTasks(simulationModel, jobCollection);
            Task.WhenAll(jobModelTasks).Wait();

            packageModel.JobModels = new List<SimulationJobModel>(jobModelTasks.Count);
            packageModel.JobModels.AddRange(jobModelTasks.Select(x => x.Result));

            LinkPackageModel(packageModel);
            RunPostBuildOptimizers(packageModel, jobCollection);

            return packageModel;
        }

        /// <inheritdoc />
        public Task<SimulationJobPackageModel> BuildJobPackageModelAsync(IJobCollection jobCollection)
        {
            return Task.Run(() => BuildJobPackageModel(jobCollection));
        }

        /// <inheritdoc />
        public void AddPostBuildOptimizer(IPostBuildOptimizer postBuildOptimizer)
        {
            PostBuildOptimizers.Add(postBuildOptimizer);
        }

        /// <summary>
        ///     Creates an indexed set of simulation job models for the passed simulation model with the passed sequence of job
        ///     configurations
        /// </summary>
        /// <param name="simulationModel"></param>
        /// <param name="jobConfigurations"></param>
        /// <returns></returns>
        protected IEnumerable<SimulationJobModel> GetJobModels(ISimulationModel simulationModel,
            IEnumerable<JobConfiguration> jobConfigurations)
        {
            var index = 0;
            foreach (var jobConfiguration in jobConfigurations)
            {
                jobConfiguration.JobId = index++;
                var jobModel = GetJobModel(simulationModel, jobConfiguration);
                JobIsBuildEvent.OnNext(jobConfiguration.JobId);
                yield return jobModel;
            }
        }

        /// <summary>
        ///     Get a list interface of job model build tasks for the passed simulation model with the passed job configuration
        ///     sequence
        /// </summary>
        /// <param name="simulationModel"></param>
        /// <param name="jobConfigurations"></param>
        /// <returns></returns>
        protected IList<Task<SimulationJobModel>> GetJobModelBuildTasks(ISimulationModel simulationModel,
            IEnumerable<JobConfiguration> jobConfigurations)
        {
            var result = new List<Task<SimulationJobModel>>();

            var index = 1;
            foreach (var jobConfiguration in jobConfigurations)
            {
                jobConfiguration.JobId = index++;
                var jobModelTask = Task.Run(() =>
                {
                    var jobModel = GetJobModel(simulationModel, jobConfiguration);
                    JobIsBuildEvent.OnNext(jobConfiguration.JobId);
                    return jobModel;
                });
                result.Add(jobModelTask);
            }

            return result;
        }

        /// <summary>
        ///     Get a new job model database object for the passed simulation model and specified job configuration
        /// </summary>
        /// <param name="simulationModel"></param>
        /// <param name="jobConfiguration"></param>
        /// <returns></returns>
        protected SimulationJobModel GetJobModel(ISimulationModel simulationModel, JobConfiguration jobConfiguration)
        {
            var result = new SimulationJobModel
            {
                JobInfo = jobConfiguration.GetInteropJobInfo(),
                JobHeader = jobConfiguration.GetInteropJobHeader(),
                JobMetaData = GetJobMetaDataEntity(jobConfiguration),
                SimulationLatticeModel = LatticeDbEntityBuilder.BuildModel(simulationModel, jobConfiguration.LatticeConfiguration)
            };

            result.JobMetaData.JobModel = result;
            SetSimulationJobInfoFlags(result, simulationModel);
            return result;
        }

        /// <summary>
        ///     Get a <see cref="JobMetaDataEntity" /> for the passed <see cref="JobConfiguration" />
        /// </summary>
        /// <param name="jobConfiguration"></param>
        /// <returns></returns>
        protected JobMetaDataEntity GetJobMetaDataEntity(JobConfiguration jobConfiguration)
        {
            var entity = new JobMetaDataEntity
            {
                JobCollectionName = jobConfiguration.CollectionName,
                JobConfigName = jobConfiguration.ConfigName,
                JobIndex = jobConfiguration.JobId,
                Temperature = jobConfiguration.Temperature,
                MainRunMcsp = jobConfiguration.TargetMcsp,
                TimeLimit = jobConfiguration.TimeLimit,
                DopingInfo = jobConfiguration.LatticeConfiguration.GetDopingString(),
                LatticeInfo = jobConfiguration.LatticeConfiguration.GetSizeString()
            };

            AddKineticMetaData(entity, jobConfiguration);

            return entity;
        }

        /// <summary>
        ///     Adds meta information to the passed <see cref="JobMetaDataEntity" /> specific to <see cref="KmcJobConfiguration" />
        ///     if possible
        /// </summary>
        /// <param name="jobMetaData"></param>
        /// <param name="jobConfiguration"></param>
        protected void AddKineticMetaData(JobMetaDataEntity jobMetaData, JobConfiguration jobConfiguration)
        {
            var kmcConfig = jobConfiguration as KmcJobConfiguration;

            jobMetaData.ElectricFieldModulus = kmcConfig?.ElectricFieldModulus ?? double.NaN;
            jobMetaData.BaseFrequency = kmcConfig?.BaseFrequency ?? double.NaN;
            jobMetaData.NormalizationFactor = kmcConfig?.FixedNormalizationFactor ?? double.NaN;
            jobMetaData.PreRunMcsp = kmcConfig?.PreRunMcsp ?? -1;
        }

        /// <summary>
        ///     Set the simulation type flags according to a simulation model in the passed prepared simulation job model
        /// </summary>
        /// <param name="jobModel"></param>
        /// <param name="simulationModel"></param>
        protected void SetSimulationJobInfoFlags(SimulationJobModel jobModel, ISimulationModel simulationModel)
        {
            const long dofFlag = (long) SimulationJobInfoFlags.UseDualDofCorrection;
            const long kmcFlag = (long) SimulationJobInfoFlags.KmcSimulation;
            const long mmcFlag = (long) SimulationJobInfoFlags.MmcSimulation;
            const long preFlag = (long) SimulationJobInfoFlags.UsePrerun;

            switch (simulationModel)
            {
                case IKineticSimulationModel _:
                    var header = ((InteropObject<CKmcJobHeader>) jobModel.JobHeader).Structure;
                    jobModel.JobInfo.Structure.JobFlags |= dofFlag;
                    jobModel.JobInfo.Structure.JobFlags |= kmcFlag;
                    jobModel.JobInfo.Structure.JobFlags -= jobModel.JobInfo.Structure.JobFlags & mmcFlag;
                    jobModel.JobInfo.Structure.JobFlags |= header.PreRunMcsp == 0 ? 0 : preFlag;
                    return;

                case IMetropolisSimulationModel _:
                    jobModel.JobInfo.Structure.JobFlags |= mmcFlag;
                    jobModel.JobInfo.Structure.JobFlags -= jobModel.JobInfo.Structure.JobFlags & kmcFlag;
                    return;

                default:
                    throw new ArgumentException("Type of simulation model is not supported", nameof(simulationModel));
            }
        }

        /// <summary>
        ///     Creates a new default package model for the passed simulation model
        /// </summary>
        /// <returns></returns>
        protected SimulationJobPackageModel CreatePackageModel(ISimulationModel simulationModel)
        {
            var model = new SimulationJobPackageModel
            {
                SimulationEnergyModel = EnergyDbEntityBuilder.BuildModel(simulationModel),
                SimulationStructureModel = StructureDbEntityBuilder.BuildModel(simulationModel),
                SimulationTransitionModel = TransitionDbEntityBuilder.BuildModel(simulationModel)
            };
            return model;
        }

        /// <summary>
        ///     Performs the required internal link operations on the passed simulation job package model
        /// </summary>
        /// <param name="packageModel"></param>
        protected void LinkPackageModel(SimulationJobPackageModel packageModel)
        {
            foreach (var jobModel in packageModel.JobModels)
            {
                jobModel.SimulationEnergyModel = packageModel.SimulationEnergyModel;
                jobModel.SimulationTransitionModel = packageModel.SimulationTransitionModel;
                jobModel.SimulationStructureModel = packageModel.SimulationStructureModel;
                jobModel.SimulationJobPackageModel = packageModel;
            }

            packageModel.LatticeModels = packageModel.JobModels
                .Select(x => x.SimulationLatticeModel)
                .Action(x => x.SimulationJobPackageModel = packageModel)
                .ToList();
        }

        /// <summary>
        ///     Calls all attached <see cref="IPostBuildOptimizer" /> and additionally defined ones of
        ///     <see cref="IJobCollection" /> and removes invalidated <see cref="SimulationJobInfoFlags" />
        /// </summary>
        /// <param name="packageModel"></param>
        /// <param name="jobCollection"></param>
        protected void RunPostBuildOptimizers(SimulationJobPackageModel packageModel, IJobCollection jobCollection)
        {
            var removedFlags = PostBuildOptimizers
                .Concat(jobCollection.GetPostBuildOptimizers())
                .Aggregate(SimulationJobInfoFlags.None, (current, optimizer) => current | optimizer.Run(ProjectModelContext, packageModel));

            foreach (var jobModel in packageModel.JobModels)
            {
                jobModel.JobInfo.Structure.JobFlags -= jobModel.JobInfo.Structure.JobFlags & (long) removedFlags;
                jobModel.JobMetaData.FlagValues = ((SimulationJobInfoFlags) jobModel.JobInfo.Structure.JobFlags).ToString();
            }
        }

        /// <summary>
        ///     Sets all null db builder components to use the default build system
        /// </summary>
        protected virtual void PrepareBuildComponents()
        {
            EnergyDbEntityBuilder = EnergyDbEntityBuilder ?? new EnergyDbEntityBuilder(ProjectModelContext);
            StructureDbEntityBuilder = StructureDbEntityBuilder ?? new StructureDbEntityBuilder(ProjectModelContext);
            TransitionDbEntityBuilder = TransitionDbEntityBuilder ?? new TransitionDbEntityBuilder(ProjectModelContext);
            LatticeDbEntityBuilder = LatticeDbEntityBuilder ?? new LatticeDbEntityBuilder(ProjectModelContext);
        }
    }
}