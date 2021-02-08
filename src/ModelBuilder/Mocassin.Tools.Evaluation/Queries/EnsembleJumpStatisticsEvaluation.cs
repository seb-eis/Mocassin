using System;
using System.Linq;
using Mocassin.Framework.Collections.Mocassin.Tools.Evaluation.Queries;
using Mocassin.Framework.Extensions;
using Mocassin.Model.Translator.ModelContext;
using Mocassin.Tools.Evaluation.Context;
using Mocassin.Tools.Evaluation.Queries.Data;

namespace Mocassin.Tools.Evaluation.Queries
{
    /// <summary>
    ///     An evaluation that supplies <see cref="JumpStatisticsData" /> for each jump histogram of a simulation
    /// </summary>
    public class EnsembleJumpStatisticsEvaluation : JobEvaluation<ReadOnlyList<JumpStatisticsData>>
    {
        /// <summary>
        ///     Get or set the required <see cref="EnsembleMetaEvaluation" />
        /// </summary>
        public EnsembleMetaEvaluation EnsembleMetaEvaluation { get; set; }

        /// <inheritdoc />
        public EnsembleJumpStatisticsEvaluation(IEvaluableJobSet jobSet)
            : base(jobSet)
        {
        }

        /// <summary>
        ///     Groups the results into their affiliated <see cref="EnsembleJumpStatisticsData"/>
        /// </summary>
        /// <returns></returns>
        public ReadOnlyList<ReadOnlyList<EnsembleJumpStatisticsData>> GetGroupedEnsembles()
        {
            return Result.Select(x => x.GroupBy(y => y.GlobalTrackerModel.TrackedParticle))
                         .Select(x => x.Select(y => new EnsembleJumpStatisticsData(y.ToList().AsReadOnlyList(), y.Key)))
                         .Select(x => x.ToList().AsReadOnlyList())
                         .ToList()
                         .AsReadOnlyList();
        }

        /// <inheritdoc />
        protected override ReadOnlyList<JumpStatisticsData> GetValue(JobContext jobContext)
        {
            var globalTrackerModels = jobContext.SimulationModel.SimulationTrackingModel.GlobalTrackerModels;
            var temperature = jobContext.JobModel.JobMetaData.Temperature;
            var result = globalTrackerModels
                         .Select(trackerModel => new JumpStatisticsData(jobContext.McsReader, trackerModel, temperature, GetEnsembleSize(trackerModel, jobContext)))
                         .ToList();
            return result.AsReadOnlyList();
        }

        /// <summary>
        ///     Gets the ensemble size for a <see cref="IGlobalTrackerModel"/>
        /// </summary>
        /// <param name="trackerModel"></param>
        /// <param name="jobContext"></param>
        /// <returns></returns>
        protected int GetEnsembleSize(IGlobalTrackerModel trackerModel, JobContext jobContext)
        {
            return EnsembleMetaEvaluation.ParticleCountEvaluation[jobContext.DataId][trackerModel.TrackedParticle.Index];
        }

        /// <inheritdoc />
        protected override void PrepareForExecution()
        {
            EnsembleMetaEvaluation ??= new EnsembleMetaEvaluation(JobSet);
            if (!EnsembleMetaEvaluation.JobSet.CompatibleTo(JobSet))
                throw new InvalidOperationException("The ensemble meta evaluation is not compatible");

            EnsembleMetaEvaluation.Run().Wait();
            base.PrepareForExecution();
        }
    }
}