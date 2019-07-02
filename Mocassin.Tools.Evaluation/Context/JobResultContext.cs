using System;
using Mocassin.Model.Translator;
using Mocassin.Model.Translator.ModelContext;
using Mocassin.Tools.UAccess.Readers;

namespace Mocassin.Tools.Evaluation.Context
{
    /// <summary>
    ///     Provides a context for evaluation of the results of a single <see cref="SimulationJobModel" />
    /// </summary>
    public class JobResultContext : IEquatable<JobResultContext>
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
        public bool IsReadingPrimary { get; private set; }

        /// <summary>
        ///     Get the <see cref="IProjectModelContext"/> of this job
        /// </summary>
        public IProjectModelContext ModelContext => EvaluationContext.GetProjectModelContext(JobModel);

        /// <summary>
        ///     Get the <see cref="ISimulationModel"/> of this job
        /// </summary>
        public ISimulationModel SimulationModel => EvaluationContext.GetSimulationModel(JobModel);

        /// <summary>
        ///     Creates new <see cref="JobResultContext" /> for the passed <see cref="SimulationJobModel" /> and
        ///     <see cref="MslEvaluationContext" />
        /// </summary>
        /// <param name="jobModel"></param>
        /// <param name="evaluationContext"></param>
        private JobResultContext(SimulationJobModel jobModel, MslEvaluationContext evaluationContext)
        {
            JobModel = jobModel ?? throw new ArgumentNullException(nameof(jobModel));
            EvaluationContext = evaluationContext ?? throw new ArgumentNullException(nameof(evaluationContext));
        }

        /// <summary>
        ///     Creates a new <see cref="JobResultContext" /> that targets the primary results of the passed
        ///     <see cref="SimulationJobModel" /> within the specified <see cref="MslEvaluationContext"/>
        /// </summary>
        /// <param name="jobModel"></param>
        /// <param name="evaluationContext"></param>
        /// <returns></returns>
        public static JobResultContext CreatePrimary(SimulationJobModel jobModel, MslEvaluationContext evaluationContext)
        {
            return CreateInternal(jobModel, evaluationContext, false);
        }

        /// <summary>
        ///     Creates a new <see cref="JobResultContext" /> that targets the secondary results of the passed
        ///     <see cref="SimulationJobModel" /> within the specified <see cref="MslEvaluationContext"/>
        /// </summary>
        /// <param name="jobModel"></param>
        /// <param name="evaluationContext"></param>
        /// <returns></returns>
        public static JobResultContext CreateSecondary(SimulationJobModel jobModel, MslEvaluationContext evaluationContext)
        {
            return CreateInternal(jobModel, evaluationContext, true);
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
        ///     Creates a new <see cref="JobResultContext" /> for a <see cref="SimulationJobModel" /> with a flag if the
        ///     secondary state should be targeted
        /// </summary>
        /// <param name="jobModel"></param>
        /// <param name="evaluationContext"></param>
        /// <param name="useSecondaryState"></param>
        /// <returns></returns>
        private static JobResultContext CreateInternal(SimulationJobModel jobModel, MslEvaluationContext evaluationContext, bool useSecondaryState)
        {
            if (jobModel.JobResultData == null)
                throw new InvalidOperationException("Result data is null on passed job model");
            var evalContext = new JobResultContext(jobModel, evaluationContext);

            if (useSecondaryState)
                evalContext.SwitchMcsTargetInternal(jobModel.JobResultData.SimulationPreRunStateBinary);
            else
            {
                evalContext.SwitchMcsTargetInternal(jobModel.JobResultData.SimulationStateBinary);
                evalContext.IsReadingPrimary = true;
            }

            return evalContext;
        }

        /// <inheritdoc />
        public bool Equals(JobResultContext other)
        {
            return JobModel.Equals(other?.JobModel);
        }
    }
}