using System;
using Mocassin.Model.Translator;
using Mocassin.Model.Translator.ModelContext;
using Mocassin.Tools.UAccess.Readers;

namespace Mocassin.Tools.Evaluation.Context
{
    /// <summary>
    ///     Provides a context for evaluation of the results of a single <see cref="SimulationJobModel" />
    /// </summary>
    public class JobContext : IEquatable<JobContext>, IDisposable
    {
        /// <summary>
        ///     Get the <see cref="MslEvaluationContext" /> that the job belongs to
        /// </summary>
        public MslEvaluationContext EvaluationContext { get; }

        /// <summary>
        ///     Get the <see cref="SimulationJobModel" /> that is evaluated
        /// </summary>
        public SimulationJobModel JobModel { get; }

        /// <summary>
        ///     Get a <see cref="McsContentReader" /> to read the contents of the binary state
        /// </summary>
        public McsContentReader McsReader { get; private set; }

        /// <summary>
        ///     Get a boolean flag indicating if the context reads the primary or secondary (pre-run) state
        /// </summary>
        public bool IsReadingPrimaryState { get; private set; }

        /// <summary>
        ///     Get the <see cref="IProjectModelContext" /> of this job
        /// </summary>
        public IProjectModelContext ModelContext => EvaluationContext.GetProjectModelContext(JobModel);

        /// <summary>
        ///     Get the <see cref="ISimulationModel" /> of this job
        /// </summary>
        public ISimulationModel SimulationModel => EvaluationContext.GetSimulationModel(JobModel);

        /// <summary>
        ///     Get the id of the context in its data source collection
        /// </summary>
        public int DataId { get; }

        /// <summary>
        ///     Get the auto generated full config name
        /// </summary>
        public string FullConfigName { get; }

        /// <summary>
        ///     Creates new <see cref="JobContext" /> for the passed <see cref="SimulationJobModel" /> and
        ///     <see cref="MslEvaluationContext" />
        /// </summary>
        /// <param name="jobModel"></param>
        /// <param name="evaluationContext"></param>
        /// <param name="dataId"></param>
        private JobContext(SimulationJobModel jobModel, MslEvaluationContext evaluationContext, int dataId)
        {
            JobModel = jobModel ?? throw new ArgumentNullException(nameof(jobModel));
            EvaluationContext = evaluationContext ?? throw new ArgumentNullException(nameof(evaluationContext));
            DataId = dataId;
            FullConfigName = MakeFullConfigName(jobModel);
        }

        /// <summary>
        ///     Creates a new <see cref="JobContext" /> that targets the primary results of the passed
        ///     <see cref="SimulationJobModel" /> within the specified <see cref="MslEvaluationContext" />
        /// </summary>
        /// <param name="jobModel"></param>
        /// <param name="evaluationContext"></param>
        /// <param name="dataId"></param>
        /// <returns></returns>
        public static JobContext CreatePrimary(SimulationJobModel jobModel, MslEvaluationContext evaluationContext, int dataId)
        {
            return CreateInternal(jobModel, evaluationContext, dataId, false);
        }

        /// <summary>
        ///     Creates a new <see cref="JobContext" /> that targets the secondary results of the passed
        ///     <see cref="SimulationJobModel" /> within the specified <see cref="MslEvaluationContext" />
        /// </summary>
        /// <param name="jobModel"></param>
        /// <param name="evaluationContext"></param>
        /// <param name="dataId"></param>
        /// <returns></returns>
        public static JobContext CreateSecondary(SimulationJobModel jobModel, MslEvaluationContext evaluationContext, int dataId)
        {
            return CreateInternal(jobModel, evaluationContext, dataId, true);
        }

        /// <summary>
        ///     Tries to switch the <see cref="McsReader" /> target to the passed binary set
        /// </summary>
        /// <param name="mcsBinary"></param>
        /// <returns></returns>
        private void SwitchMcsTargetInternal(byte[] mcsBinary)
        {
            if (mcsBinary == null || mcsBinary.Length == 0)
                throw new InvalidOperationException("Cannot read NULL or empty state target");

            McsReader = McsContentReader.Create(mcsBinary);
        }

        /// <summary>
        ///     Creates a new <see cref="JobContext" /> for a <see cref="SimulationJobModel" /> with a flag if the
        ///     secondary state should be targeted
        /// </summary>
        /// <param name="jobModel"></param>
        /// <param name="evaluationContext"></param>
        /// <param name="dataId"></param>
        /// <param name="useSecondaryState"></param>
        /// <returns></returns>
        private static JobContext CreateInternal(SimulationJobModel jobModel, MslEvaluationContext evaluationContext, int dataId,
            bool useSecondaryState)
        {
            if (jobModel.JobResultData == null)
                throw new InvalidOperationException("Result data is null on passed job model");
            var evalContext = new JobContext(jobModel, evaluationContext, dataId);

            if (useSecondaryState)
                evalContext.SwitchMcsTargetInternal(jobModel.JobResultData.SimulationPreRunStateBinary);
            else
            {
                evalContext.SwitchMcsTargetInternal(jobModel.JobResultData.SimulationStateBinary);
                evalContext.IsReadingPrimaryState = true;
            }

            return evalContext;
        }

        /// <inheritdoc />
        public bool Equals(JobContext other)
        {
            return JobModel.Equals(other?.JobModel);
        }

        /// <summary>
        ///     Creates a name for a <see cref="SimulationJobModel" /> based on the meta information that can be used to identify
        ///     multiplied jobs
        /// </summary>
        /// <param name="jobModel"></param>
        /// <returns></returns>
        private string MakeFullConfigName(SimulationJobModel jobModel)
        {
            if (JobModel.JobMetaData == null) return null;
            return $"{jobModel.JobMetaData.CollectionName}:{jobModel.JobMetaData.ConfigName}";
        }

        /// <inheritdoc />
        public void Dispose()
        {
            McsReader.Dispose();
        }
    }
}