using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Mocassin.Framework.Extensions;
using Mocassin.Framework.SQLiteCore;
using Mocassin.Model.DataManagement;
using Mocassin.Model.ModelProject;
using Mocassin.Model.Translator;
using Mocassin.Model.Translator.ModelContext;
using Mocassin.Tools.Evaluation.Queries;
using Mocassin.UI.Data.Base;
using Mocassin.UI.Data.Main;

namespace Mocassin.Tools.Evaluation.Context
{
    /// <summary>
    ///     Context for a evaluation of contents and results provided by <see cref="ISimulationLibrary" /> interfaces. This
    ///     context manages the lifetimes of <see cref="JobContext" /> instances
    ///     and not properly disposing the context or creating <see cref="JobContext" /> instances not known by the this
    ///     context will cause memory leaking
    /// </summary>
    public class MslEvaluationContext : IDisposable
    {
        private readonly object lockObject = new object();

        /// <summary>
        ///     Get a <see cref="Dictionary{TKey,TValue}" /> that caches <see cref="IProjectModelContext" /> instances for each job
        ///     packaged context id
        /// </summary>
        private Dictionary<int, IProjectModelContext> ProjectContextCache { get; }

        /// <summary>
        ///     Get a <see cref="Dictionary{TKey,TValue}" /> that caches <see cref="ISimulationModel" /> getters for each job
        ///     packaged context id
        /// </summary>
        private Dictionary<int, ISimulationModel> SimulationModelCache { get; }

        /// <summary>
        ///     Stores all <see cref="JobContext" /> instances that were provided by the context
        /// </summary>
        private HashSet<JobContext> KnownJobContexts { get; }

        /// <summary>
        ///     Get the provider <see cref="Func{TResult}" /> that supplies <see cref="IModelProject" /> instances
        /// </summary>
        public Func<IModelProject> ModelProjectProvider { get; }

        /// <summary>
        ///     Get the <see cref="ReadOnlyDbContext" /> that supplies read only access to the database contents
        /// </summary>
        public ReadOnlyDbContext DataContext { get; }

        /// <summary>
        ///     Get the <see cref="IMarshalService" /> to handle marshalling of interop objects
        /// </summary>
        public IMarshalService MarshalService { get; }

        /// <summary>
        ///     Creates new <see cref="MslEvaluationContext" /> for a <see cref="SimulationDbContext" /> that used the
        ///     passed <see cref="IModelProject" /> provider
        /// </summary>
        /// <param name="simulationLibrary"></param>
        /// <param name="modelProjectProvider"></param>
        protected MslEvaluationContext(SimulationDbContext simulationLibrary, Func<IModelProject> modelProjectProvider)
        {
            DataContext = simulationLibrary?.AsReadOnly() ?? throw new ArgumentNullException(nameof(simulationLibrary));
            ModelProjectProvider = modelProjectProvider ?? throw new ArgumentNullException(nameof(modelProjectProvider));
            ProjectContextCache = new Dictionary<int, IProjectModelContext>();
            SimulationModelCache = new Dictionary<int, ISimulationModel>();
            MarshalService = new MarshalService();
            KnownJobContexts = new HashSet<JobContext>();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            DataContext.Dispose();
            MarshalService.Dispose();
            KnownJobContexts.DisposeAllAndClear();
        }

        /// <summary>
        ///     A convenience function to quickly combine querying and calling <see cref="MakeEvaluableSet" />
        /// </summary>
        /// <param name="queryMutator"></param>
        /// <param name="targetSecondaryState"></param>
        /// <returns></returns>
        public IEvaluableJobSet LoadJobsAsEvaluable(Func<IQueryable<SimulationJobModel>, IQueryable<SimulationJobModel>> queryMutator,
            bool targetSecondaryState = false)
        {
            var query = queryMutator.Invoke(EvaluationJobSet());
            return MakeEvaluableSet(query, targetSecondaryState);
        }

        /// <summary>
        ///     Get a <see cref="IQueryable{T}" /> for queries against the <see cref="SimulationJobModel" /> set of the context
        ///     without any includes
        /// </summary>
        /// <returns></returns>
        public IQueryable<SimulationJobModel> BasicJobSet() => DataContext.Set<SimulationJobModel>();

        /// <summary>
        ///     Get a <see cref="IQueryable{T}" /> for queries against the <see cref="SimulationJobModel" /> set of the context
        ///     with package, meta data and result entities included where at least the run state binary exists
        /// </summary>
        /// <returns></returns>
        public IQueryable<SimulationJobModel> EvaluationJobSet()
        {
            return BasicJobSet().Include(x => x.SimulationJobPackageModel)
                                .Include(x => x.JobMetaData)
                                .Include(x => x.JobResultData)
                                .Where(x => x.JobResultData.SimulationStateBinary != null);
        }

        /// <summary>
        ///     Loads the passed <see cref="SimulationJobModel" /> instances and prepares a <see cref="IEvaluableJobSet" /> for
        ///     usage with the query system
        /// </summary>
        /// <param name="jobModels"></param>
        /// <param name="targetSecondaryState"></param>
        /// <returns></returns>
        public IEvaluableJobSet MakeEvaluableSet(IQueryable<SimulationJobModel> jobModels, bool targetSecondaryState = false)
        {
            var index = 0;
            var contextSet = targetSecondaryState
                ? jobModels.AsEnumerable().Select(x => JobContext.CreateSecondary(x, this, index++)).ToList()
                : jobModels.AsEnumerable().Select(x => JobContext.CreatePrimary(x, this, index++)).ToList();

            var evaluableSet = new EvaluableJobSet(contextSet);
            foreach (var jobContext in evaluableSet)
            {
                EnsureModelContextCreated(jobContext);
                KnownJobContexts.Add(jobContext);
            }

            return evaluableSet;
        }

        /// <summary>
        ///     Ensures that the <see cref="IProjectModelContext" /> for the passed <see cref="JobContext" /> is loaded into the
        ///     caching system
        /// </summary>
        /// <param name="jobContext"></param>
        public void EnsureModelContextCreated(JobContext jobContext)
        {
            GetProjectModelContext(jobContext.JobModel.SimulationPackageId);
            GetSimulationModel(jobContext.JobModel);
        }

        /// <summary>
        ///     Restores the <see cref="IProjectModelContext" /> from a passed project xml <see cref="string" />
        /// </summary>
        /// <param name="projectXml"></param>
        /// <returns></returns>
        public IProjectModelContext RestoreProjectModelContext(string projectXml)
        {
            var dbBuildTemplate = ProjectDataObject.CreateFromXml<SimulationDbBuildTemplate>(projectXml);
            var modelProject = ModelProjectProvider.Invoke();
            modelProject.InputPipeline.PushToProject(dbBuildTemplate.ProjectModelData.GetInputSequence());
            dbBuildTemplate.ProjectCustomizationTemplate.PushToModel(modelProject);
            var builder = new ProjectModelContextBuilder(modelProject);
            return builder.BuildContextAsync().Result;
        }

        /// <summary>
        ///     Takes an <see cref="IQueryable{T}" /> of <see cref="SimulationJobPackageModel" /> and builds the sequence of
        ///     <see cref="IProjectModelContext" /> instances
        /// </summary>
        /// <param name="jobPackageModels"></param>
        /// <returns></returns>
        public IQueryable<IProjectModelContext> RestoreProjectModelContext(IQueryable<SimulationJobPackageModel> jobPackageModels)
        {
            return jobPackageModels
                   .Include(x => x.ProjectXml)
                   .Select(x => RestoreProjectModelContext(x.ProjectXml));
        }

        /// <summary>
        ///     Get the <see cref="IProjectModelContext" /> that belongs to the passed <see cref="SimulationJobPackageModel" />
        ///     context id with an optional boolean flag to enforce recreation of the cached context
        /// </summary>
        /// <param name="contextId"></param>
        /// <returns></returns>
        public IProjectModelContext GetProjectModelContext(int contextId)
        {
            if (contextId < 1) throw new ArgumentException("Context id cannot be smaller than 1");
            if (ProjectContextCache.TryGetValue(contextId, out var context)) return context;

            var packageModel = LoadJobPackageModel(contextId);
            var context2 = RestoreProjectModelContext(packageModel.ProjectXml);
            lock (lockObject)
            {
                ProjectContextCache[contextId] = context2;
            }

            return context2;
        }

        /// <summary>
        ///     Get a <see cref="SimulationJobPackageModel" /> by context id from the database
        /// </summary>
        /// <param name="contextId"></param>
        /// <returns></returns>
        public SimulationJobPackageModel LoadJobPackageModel(int contextId)
        {
            lock (lockObject)
            {
                return DataContext.Set<SimulationJobPackageModel>().Single(x => x.Id == contextId);
            }
        }

        /// <summary>
        ///     Get the <see cref="IProjectModelContext" /> that belongs to the passed <see cref="SimulationJobPackageModel" />
        /// </summary>
        /// <param name="packageModel"></param>
        /// <returns></returns>
        public IProjectModelContext GetProjectModelContext(SimulationJobPackageModel packageModel)
        {
            if (packageModel == null) throw new ArgumentNullException(nameof(packageModel));
            return GetProjectModelContext(packageModel.Id);
        }

        /// <summary>
        ///     Get the <see cref="IProjectModelContext" /> that belongs to the passed <see cref="SimulationJobModel" />
        /// </summary>
        /// <param name="jobModel"></param>
        /// <returns></returns>
        public IProjectModelContext GetProjectModelContext(SimulationJobModel jobModel)
        {
            if (jobModel == null) throw new ArgumentNullException(nameof(jobModel));
            return GetProjectModelContext(jobModel.SimulationPackageId);
        }

        /// <summary>
        ///     Get the <see cref="ISimulationModel" /> that belongs to the passed <see cref="SimulationJobModel" />
        ///     from the matching <see cref="IProjectModelContext" />
        /// </summary>
        /// <param name="jobModel"></param>
        /// <returns></returns>
        public ISimulationModel GetSimulationModel(SimulationJobModel jobModel)
        {
            var modelContext = GetProjectModelContext(jobModel);
            if (SimulationModelCache.TryGetValue(jobModel.SimulationPackageId, out var result)) return result;

            var buildGraph = ProjectDataObject.CreateFromXml<SimulationDbBuildTemplate>(jobModel.SimulationJobPackageModel.ProjectXml);
            var simulation = buildGraph.ProjectJobSetTemplate.ToInternals(modelContext.ModelProject).First().GetSimulation();
            var simulationModel = modelContext.SimulationModelContext.FindSimulationModel(simulation);

            lock (lockObject)
            {
                SimulationModelCache[jobModel.SimulationPackageId] = simulationModel;
            }

            return simulationModel;
        }

        /// <summary>
        ///     Creates a new <see cref="MslEvaluationContext" /> for the passed simulation database filename and
        ///     <see cref="IModelProject" /> provider function
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="modelProjectProvider"></param>
        /// <returns></returns>
        public static MslEvaluationContext Create(string filename, Func<IModelProject> modelProjectProvider)
        {
            if (filename == null) throw new ArgumentNullException(nameof(filename));
            if (modelProjectProvider == null) throw new ArgumentNullException(nameof(modelProjectProvider));

            var context = SqLiteContext.OpenDatabase<SimulationDbContext>(filename);
            return new MslEvaluationContext(context, modelProjectProvider);
        }

        /// <summary>
        ///     Creates a new <see cref="MslEvaluationContext" /> for the passed simulation database filename that
        ///     uses the default <see cref="IModelProject" /> provider function
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static MslEvaluationContext Create(string filename) => Create(filename, ModelProjectFactory.CreateDefault);
    }
}