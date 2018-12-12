using System;
using System.Collections.Generic;
using System.Linq;
using Mocassin.Framework.Extensions;
using Mocassin.Model.Simulations;
using Mocassin.Model.Translator.Jobs;
using Mocassin.Model.Translator.ModelContext;

namespace Mocassin.Model.Translator.DbBuilder
{
    /// <inheritdoc />
    public class JobDbModelBuilder : IJobDbModelBuilder
    {
        /// <summary>
        ///     Get or set the current work project model context
        /// </summary>
        private IProjectModelContext ProjectModelContext { get; set; }

        /// <summary>
        ///     Get or set the energy db model builder
        /// </summary>
        private IEnergyDbModelBuilder EnergyDbModelBuilder { get; set; }

        /// <summary>
        ///     Get or set the structure db model builder
        /// </summary>
        private IStructureDbModelBuilder StructureDbModelBuilder { get; set; }

        /// <summary>
        ///     Get or set the transition db model builder
        /// </summary>
        private ITransitionDbModelBuilder TransitionDbModelBuilder { get; set; }

        /// <inheritdoc />
        public IProjectModelContextBuilder ProjectModelContextBuilder { get; set; }

        /// <summary>
        ///     Create new db model builder that uses the passed model context builder
        /// </summary>
        /// <param name="projectModelContextBuilder"></param>
        public JobDbModelBuilder(IProjectModelContextBuilder projectModelContextBuilder)
        {
            ProjectModelContextBuilder = projectModelContextBuilder ?? throw new ArgumentNullException(nameof(projectModelContextBuilder));
        }

        /// <inheritdoc />
        public SimulationJobPackageModel BuildJobPackageModel(IJobCollection jobCollection)
        {
            PrepareBuildComponents();
            var packageModel = CreatePackageModel(jobCollection.GetSimulation());
            packageModel.JobModels = GetJobModels(jobCollection.GetJobConfigurations()).ToList();
            LinkPackageModel(packageModel);
            return packageModel;
        }

        /// <summary>
        /// Creates an indexed set of simulation job models for the passed sequence of job configurations
        /// </summary>
        /// <param name="jobConfigurations"></param>
        /// <returns></returns>
        protected IEnumerable<SimulationJobModel> GetJobModels(IEnumerable<JobConfiguration> jobConfigurations)
        {
            var index = 0;
            foreach (var jobConfiguration in jobConfigurations)
            {
                jobConfiguration.JobId = index++;
                var jobModel = GetJobModel(jobConfiguration);
                yield return jobModel;
            }
        }

        /// <summary>
        ///     Get a new job model database object for the passed job configuration
        /// </summary>
        /// <param name="jobConfiguration"></param>
        /// <returns></returns>
        protected SimulationJobModel GetJobModel(JobConfiguration jobConfiguration)
        {
            var result = new SimulationJobModel
            {
                JobInfo = jobConfiguration.GetInteropJobInfo(),
                JobHeader = jobConfiguration.GetInteropJobHeader(),
                SimulationLatticeModel = GetLatticeModel(jobConfiguration.LatticeConfiguration)
            };

            CompleteSimulationJobModelData(result);
            return result;
        }

        /// <summary>
        /// Calculates missing data and sets it on the passed simulation job model
        /// </summary>
        /// <param name="jobModel"></param>
        protected void CompleteSimulationJobModelData(SimulationJobModel jobModel)
        {
            SetNumberOfMobiles(jobModel);
            SetNumberOfSelectables(jobModel);
            SetStateByteCount(jobModel);
        }

        /// <summary>
        /// Sets the total number of mobile particles on the simulation job model
        /// </summary>
        /// <param name="jobModel"></param>
        protected void SetNumberOfMobiles(SimulationJobModel jobModel)
        {

        }

        /// <summary>
        /// Sets the total number of selectable particles on the simulation job model
        /// </summary>
        /// <param name="jobModel"></param>
        protected void SetNumberOfSelectables(SimulationJobModel jobModel)
        {

        }

        /// <summary>
        /// Sets the total number of required state bytes on the simulation job model
        /// </summary>
        /// <param name="jobModel"></param>
        protected void SetStateByteCount(SimulationJobModel jobModel)
        {

        }

        /// <summary>
        /// Get the lattice model for the passed lattice configuration
        /// </summary>
        /// <param name="latticeConfiguration"></param>
        /// <returns></returns>
        protected SimulationLatticeModel GetLatticeModel(LatticeConfiguration latticeConfiguration)
        {
            var latticeModel = new SimulationLatticeModel();

            return latticeModel;
        }

        /// <summary>
        ///     Creates a new default package model for the passed simulation
        /// </summary>
        /// <returns></returns>
        protected SimulationJobPackageModel CreatePackageModel(ISimulation simulation)
        {
            var simulationModel = ProjectModelContext.SimulationModelContext.FindSimulationModel(simulation);
            if (simulationModel == null)
                throw new ArgumentException("Simulation cannot be found in the project model context");

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
        ///     Creates all db builder components and creates a new project model context
        /// </summary>
        protected void PrepareBuildComponents()
        {
            ProjectModelContext = ProjectModelContextBuilder.BuildNewContext().Result;
            EnergyDbModelBuilder = new EnergyDbModelBuilder(ProjectModelContext);
            StructureDbModelBuilder = new StructureDbModelBuilder(ProjectModelContext);
            TransitionDbModelBuilder = new TransitionDbModelBuilder(ProjectModelContext);
        }
    }
}