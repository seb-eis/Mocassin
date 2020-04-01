using System;
using System.Collections.Generic;
using Mocassin.Model.Structures;
using Mocassin.Model.Translator;
using Mocassin.Symmetry.Analysis;

namespace Mocassin.Tools.Evaluation.Queries
{
    /// <summary>
    ///     Base class for <see cref="JobEvaluation{T}"/> that perform site environment related analysis with shared <see cref="LatticeTarget"/> information
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class SiteEnvironmentEvaluation<T> : JobEvaluation<T>
    {
        /// <summary>
        ///     Get the <see cref="Dictionary{TKey,TValue}"/> of <see cref="LatticeTarget"/> lists for the search
        /// </summary>
        protected Dictionary<SimulationJobPackageModel, IDictionary<int, LatticeTarget[]>> TargetDictionary { get; }

        /// <summary>
        ///     Get the cutoff distance in [Ang]
        /// </summary>
        public double MaxDistance { get; }

        /// <summary>
        ///     Creates a <see cref="SiteEnvironmentEvaluation{T}"/> from <see cref="IEvaluableJobSet"/> and with a max sampling distance
        /// </summary>
        /// <param name="jobSet"></param>
        /// <param name="maxDistance"></param>
        protected SiteEnvironmentEvaluation(IEvaluableJobSet jobSet, double maxDistance)
            : base(jobSet)
        {
            ExecuteParallel = true;
            MaxDistance = Math.Abs(maxDistance);
            TargetDictionary = new Dictionary<SimulationJobPackageModel, IDictionary<int, LatticeTarget[]>>();
        }

        /// <inheritdoc />
        protected override void PrepareForExecution()
        {
            foreach (var jobContext in JobSet)
            {
                if (TargetDictionary.ContainsKey(jobContext.JobModel.SimulationJobPackageModel)) continue;
                var manager = jobContext.ModelContext.ModelProject.Manager<IStructureManager>();
                var targets = manager.DataAccess.Query(x => x.FindUnitCellLatticeTargets(MaxDistance, y => true, y => y.IsValidAndStable()));
                TargetDictionary.Add(jobContext.JobModel.SimulationJobPackageModel, targets);
            }
            base.PrepareForExecution();
        }
    }
}