using System.Collections.Generic;
using System.Linq;
using Mocassin.Model.Translator;
using Mocassin.Tools.Evaluation.Context;

namespace Mocassin.Tools.Evaluation.Extensions
{
    /// <summary>
    ///     Provides extension methods for <see cref="IQueryable{T}" /> of <see cref="SimulationJobModel" />
    /// </summary>
    public static class JobResultQueryableExtensions
    {
        /// <summary>
        ///     Invokes the passed query of <see cref="SimulationJobModel" /> and builds a list of <see cref="JobResultContext" />
        ///     that target the main state
        /// </summary>
        /// <param name="jobModels"></param>
        /// <param name="evaluationContext"></param>
        /// <returns></returns>
        public static IList<JobResultContext> LoadResultContexts(this IQueryable<SimulationJobModel> jobModels,
            MslEvaluationContext evaluationContext)
        {
            return jobModels.Select(x => JobResultContext.CreatePrimary(x, evaluationContext)).ToList();
        }

        /// <summary>
        ///     Invokes the passed query of <see cref="SimulationJobModel" /> and builds a list of <see cref="JobResultContext" />
        ///     that target the pre-run state
        /// </summary>
        /// <param name="jobModels"></param>
        /// <param name="evaluationContext"></param>
        /// <returns></returns>
        public static IList<JobResultContext> LoadSecondaryResultContexts(this IQueryable<SimulationJobModel> jobModels,
            MslEvaluationContext evaluationContext)
        {
            return jobModels.Select(x => JobResultContext.CreateSecondary(x, evaluationContext)).ToList();
        }
    }
}