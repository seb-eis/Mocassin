using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
        public SimulationJobPackageModel BuildJobPackageModel(IJobCollection jobCollection, CancellationToken cancellationToken = default)
        {
            PrepareBuildComponents();
            if (cancellationToken.IsCancellationRequested) throw new TaskCanceledException();
            var simulationModel = ProjectModelContext.SimulationModelContext.FindSimulationModel(jobCollection.GetSimulation());
            if (simulationModel == null)
                throw new ArgumentException("Simulation cannot be found in the project model context");

            var packageModel = CreatePackageModel(simulationModel);
            var jobModelTasks = GetJobModelBuildTasks(simulationModel, jobCollection, cancellationToken);
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
        ///     Creates a set of <see cref="SimulationJobModel"/> from a <see cref="ISimulationModel"/> and <see cref="IJobCollection"/>
        /// </summary>
        /// <param name="simulationModel"></param>
        /// <param name="jobCollection"></param>
        /// <returns></returns>
        protected IEnumerable<SimulationJobModel> GetJobModels(ISimulationModel simulationModel, IJobCollection jobCollection)
        {
            var index = 0;
            foreach (var jobConfiguration in jobCollection)
            {
                var jobModel = GetJobModel(simulationModel, jobConfiguration, jobCollection);
                JobIsBuildEvent.OnNext(++index);
                yield return jobModel;
            }
        }

        /// <summary>
        ///     Creates a set of <see cref="SimulationJobModel"/> build <see cref="Task"/> instances from a <see cref="ISimulationModel"/> and <see cref="IJobCollection"/>
        /// </summary>
        /// <param name="simulationModel"></param>
        /// <param name="jobCollection"></param>
        /// <returns></returns>
        protected IList<Task<SimulationJobModel>> GetJobModelBuildTasks(ISimulationModel simulationModel, IJobCollection jobCollection, CancellationToken cancellationToken = default)
        {
            var result = new List<Task<SimulationJobModel>>();

            var index = 1;
            foreach (var jobConfiguration in jobCollection)
            {
                var jobModelTask = Task.Run(() =>
                {
                    var jobModel = GetJobModel(simulationModel, jobConfiguration, jobCollection);
                    JobIsBuildEvent.OnNext(index++);
                    return jobModel;
                }, cancellationToken);
                result.Add(jobModelTask);
            }

            return result;
        }

        /// <summary>
        ///     Get a new job model database object for the <see cref="ISimulationModel" /> using the provided
        ///     <see cref="JobConfiguration" /> and <see cref="IJobCollection" />
        /// </summary>
        /// <param name="simulationModel"></param>
        /// <param name="jobConfiguration"></param>
        /// <param name="jobCollection"></param>
        /// <returns></returns>
        protected SimulationJobModel GetJobModel(ISimulationModel simulationModel, JobConfiguration jobConfiguration, IJobCollection jobCollection)
        {
            var result = new SimulationJobModel
            {
                JobInfo = jobConfiguration.GetInteropJobInfo(),
                JobHeader = jobConfiguration.GetInteropJobHeader(),
                JobMetaData = GetJobMetaDataEntity(jobConfiguration, jobCollection),
                JobResultData = new JobResultDataEntity(),
                SimulationLatticeModel = LatticeDbEntityBuilder.BuildModel(simulationModel, jobConfiguration.LatticeConfiguration)
            };

            if (jobConfiguration.Instruction == null) result.RoutineData = RoutineDataEntity.CreateEmpty();
            if (jobConfiguration.Instruction != null && RoutineDataEntity.TryParse(jobConfiguration.Instruction, out var routineData))
            {
                result.RoutineData = routineData ?? throw new InvalidOperationException("Failed to parse attached routine instruction.");
            }

            result.JobMetaData.JobModel = result;
            result.JobResultData.JobModel = result;
            SetSimulationJobInfoFlags(result, simulationModel);
            return result;
        }

        /// <summary>
        ///     Get a <see cref="JobMetaDataEntity" /> for the passed <see cref="JobConfiguration" /> 
        /// </summary>
        /// <param name="jobConfiguration"></param>
        /// <param name="jobCollection"></param>
        /// <returns></returns>
        protected JobMetaDataEntity GetJobMetaDataEntity(JobConfiguration jobConfiguration, IJobCollection jobCollection)
        {
            var entity = new JobMetaDataEntity
            {
                CollectionName = jobConfiguration.CollectionName,
                ConfigName = jobConfiguration.ConfigName,
                JobIndex = jobConfiguration.JobIndex,
                ConfigIndex = jobConfiguration.ConfigIndex,
                CollectionIndex = jobCollection.CollectionId,
                Temperature = jobConfiguration.Temperature,
                Mcsp = jobConfiguration.TargetMcsp,
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

            jobMetaData.ElectricFieldModulus = kmcConfig?.ElectricFieldModulus ?? 0;
            jobMetaData.BaseFrequency = kmcConfig?.BaseFrequency ?? 0;
            jobMetaData.NormalizationFactor = kmcConfig?.FixedNormalizationFactor ?? 1.0;
            jobMetaData.PreRunMcsp = kmcConfig?.PreRunMcsp ?? 0;
        }

        /// <summary>
        ///     Set the simulation type flags according to a simulation model in the passed prepared simulation job model
        /// </summary>
        /// <param name="jobModel"></param>
        /// <param name="simulationModel"></param>
        protected void SetSimulationJobInfoFlags(SimulationJobModel jobModel, ISimulationModel simulationModel)
        {
            const long dofFlag = (long) SimulationExecutionFlags.UseDualDofCorrection;
            const long kmcFlag = (long) SimulationExecutionFlags.KmcSimulation;
            const long mmcFlag = (long) SimulationExecutionFlags.MmcSimulation;
            const long preFlag = (long) SimulationExecutionFlags.UsePrerun;

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
        ///     <see cref="IJobCollection" /> and removes invalidated <see cref="SimulationExecutionFlags" />
        /// </summary>
        /// <param name="packageModel"></param>
        /// <param name="jobCollection"></param>
        protected void RunPostBuildOptimizers(SimulationJobPackageModel packageModel, IJobCollection jobCollection)
        {
            var removedFlags = PostBuildOptimizers
                .Concat(jobCollection.GetPostBuildOptimizers())
                .Aggregate(SimulationExecutionFlags.None, (current, optimizer) => current | optimizer.Run(ProjectModelContext, packageModel));

            foreach (var jobModel in packageModel.JobModels)
            {
                jobModel.JobInfo.Structure.JobFlags -= jobModel.JobInfo.Structure.JobFlags & (long) removedFlags;
                jobModel.JobMetaData.FlagString = ((SimulationExecutionFlags) jobModel.JobInfo.Structure.JobFlags).ToString();
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