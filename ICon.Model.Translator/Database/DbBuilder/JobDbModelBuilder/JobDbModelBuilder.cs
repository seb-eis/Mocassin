using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mocassin.Framework.Extensions;
using Mocassin.Model.Translator.Jobs;
using Mocassin.Model.Translator.ModelContext;

namespace Mocassin.Model.Translator.DbBuilder
{
    /// <inheritdoc />
    public class JobDbModelBuilder : IJobDbModelBuilder
    {
        /// <inheritdoc />
        public IProjectModelContext ProjectModelContext { get; set; }

        /// <summary>
        ///     Get or set the energy db model builder
        /// </summary>
        public IEnergyDbModelBuilder EnergyDbModelBuilder { get; set; }

        /// <summary>
        ///     Get or set the structure db model builder
        /// </summary>
        public IStructureDbModelBuilder StructureDbModelBuilder { get; set; }

        /// <summary>
        ///     Get or set the transition db model builder
        /// </summary>
        public ITransitionDbModelBuilder TransitionDbModelBuilder { get; set; }

        /// <summary>
        ///     Get or set the lattice db model builder
        /// </summary>
        public ILatticeDbModelBuilder LatticeDbModelBuilder { get; set; }

        /// <summary>
        ///     Create new db model builder that uses the passed model context
        /// </summary>
        /// <param name="projectModelContext"></param>
        public JobDbModelBuilder(IProjectModelContext projectModelContext)
        {
            ProjectModelContext = projectModelContext ?? throw new ArgumentNullException(nameof(projectModelContext));
        }

        /// <inheritdoc />
        public SimulationJobPackageModel BuildJobPackageModel(IJobCollection jobCollection)
        {
            PrepareBuildComponents();

            var simulationModel = ProjectModelContext.SimulationModelContext.FindSimulationModel(jobCollection.GetSimulation());
            if (simulationModel == null)
                throw new ArgumentException("Simulation cannot be found in the project model context");

            var packageModel = CreatePackageModel(simulationModel);
            var jobModelTasks = GetJobModelBuildTasks(simulationModel, jobCollection.GetJobConfigurations());
            Task.WhenAll(jobModelTasks).Wait();

            packageModel.JobModels = jobModelTasks.Select(x => x.Result).ToList();
            LinkPackageModel(packageModel);
            return packageModel;
        }

        /// <inheritdoc />
        public Task<SimulationJobPackageModel> BuildJobPackageModelAsync(IJobCollection jobCollection)
        {
            return Task.Run(() => BuildJobPackageModel(jobCollection));
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

            var index = 0;
            foreach (var jobConfiguration in jobConfigurations)
            {
                jobConfiguration.JobId = index++;
                var jobModelTask = Task.Run(() => GetJobModel(simulationModel, jobConfiguration));
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
                SimulationLatticeModel = LatticeDbModelBuilder.BuildModel(simulationModel, jobConfiguration.LatticeConfiguration)
            };

            return result;
        }

        /// <summary>
        ///     Creates a new default package model for the passed simulation model
        /// </summary>
        /// <returns></returns>
        protected SimulationJobPackageModel CreatePackageModel(ISimulationModel simulationModel)
        {
            var model = new SimulationJobPackageModel
            {
                SimulationEnergyModel = EnergyDbModelBuilder.BuildModel(simulationModel),
                SimulationStructureModel = StructureDbModelBuilder.BuildModel(simulationModel),
                SimulationTransitionModel = TransitionDbModelBuilder.BuildModel(simulationModel)
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
        ///     Sets all null db builder components to use the default build system
        /// </summary>
        protected virtual void PrepareBuildComponents()
        {
            EnergyDbModelBuilder = EnergyDbModelBuilder ?? new EnergyDbModelBuilder(ProjectModelContext);
            StructureDbModelBuilder = StructureDbModelBuilder ?? new StructureDbModelBuilder(ProjectModelContext);
            TransitionDbModelBuilder = TransitionDbModelBuilder ?? new TransitionDbModelBuilder(ProjectModelContext);
        }
    }
}