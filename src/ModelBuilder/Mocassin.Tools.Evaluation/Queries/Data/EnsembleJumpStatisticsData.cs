using System;
using System.Linq;
using Mocassin.Framework.Collections.Mocassin.Tools.Evaluation.Queries;
using Mocassin.Model.Particles;

namespace Mocassin.Tools.Evaluation.Queries.Data
{
    /// <summary>
    /// A data class that stores the <see cref="JumpStatisticsData"/> for an entire ensemble
    /// </summary>
    public class EnsembleJumpStatisticsData
    {
        /// <summary>
        ///     Get all sub <see cref="JumpStatisticsData"/>
        /// </summary>
        public ReadOnlyList<JumpStatisticsData> StatisticsList { get; }

        /// <summary>
        ///     Get the <see cref="IParticle"/> of the ensemble
        /// </summary>
        public IParticle Particle { get; }

        /// <summary>
        ///     Get the number of particles in the ensemble
        /// </summary>
        public int EnsembleSize { get; }

        /// <summary>
        ///     Get the total number of jump attempts
        /// </summary>
        public long TotalAttemptCount { get; }

        /// <summary>
        ///     Get the total number of jump successes
        /// </summary>
        public double TotalJumpCount { get; }

        /// <summary>
        ///     Get the simulated time in [s]
        /// </summary>
        public double SimulatedTime { get; }

        /// <summary>
        ///     Get the average migration rate in [Hz] of each particle
        /// </summary>
        public double AverageMigrationRate { get; }

        /// <summary>
        ///     Creates a new <see cref="EnsembleJumpStatisticsData"/> from a set of <see cref="JumpStatisticsData"/>
        /// </summary>
        /// <param name="statisticsList"></param>
        /// <param name="particle"></param>
        public EnsembleJumpStatisticsData(ReadOnlyList<JumpStatisticsData> statisticsList, IParticle particle)
        {
            ThrowIfInvalidStatisticsList(statisticsList, particle);
            StatisticsList = statisticsList;
            Particle = particle;

            EnsembleSize = statisticsList[0].EnsembleSize;
            SimulatedTime = statisticsList[0].SimulatedTime;
            TotalAttemptCount = statisticsList.Sum(x => x.AttemptCount);
            TotalJumpCount = statisticsList.Sum(x => x.EstimateJumpCount());
            AverageMigrationRate = TotalJumpCount / (EnsembleSize * SimulatedTime);
        }

        /// <summary>
        ///     Throws if the provided set of <see cref="JumpStatisticsData"/> is inconsistent
        /// </summary>
        /// <param name="statisticsList"></param>
        /// <param name="particle"></param>
        private static void ThrowIfInvalidStatisticsList(ReadOnlyList<JumpStatisticsData> statisticsList, IParticle particle)
        {
            if (statisticsList.Count == 0) 
                throw new ArgumentException("Value cannot be an empty collection.", nameof(statisticsList));

            if (statisticsList.Any(x => !x.GlobalTrackerModel.TrackedParticle.Equals(particle)))
                throw new InvalidOperationException("All particles must be identical.");

            if (statisticsList.Any(x => x.EnsembleSize != statisticsList[0].EnsembleSize))
                throw new InvalidOperationException("All ensembles must have the same size.");
        }
    }
}