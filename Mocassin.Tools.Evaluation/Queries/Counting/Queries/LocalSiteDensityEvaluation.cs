using System;
using System.Collections.Generic;
using Mocassin.Mathematics.ValueTypes;
using Mocassin.Model.Particles;
using Mocassin.Symmetry.Analysis;
using Mocassin.Tools.Evaluation.Context;

namespace Mocassin.Tools.Evaluation.Queries
{
    /// <summary>
    ///     Query that extracts local site densities
    /// </summary>
    public class LocalSiteDensityEvaluation : SiteEnvironmentEvaluation<IUnitCellProvider<IDictionary<IParticle, double>>>
    {
        /// <inheritdoc />
        public LocalSiteDensityEvaluation(IEvaluableJobSet jobSet, double maxDistance)
            : base(jobSet, maxDistance)
        {
        }

        /// <inheritdoc />
        protected override IUnitCellProvider<IDictionary<IParticle, double>> GetValue(JobContext jobContext) => throw new NotImplementedException();

        /// <summary>
        ///     Prepares the raw result <see cref="IDictionary{TKey,TValue}" /> array system
        /// </summary>
        /// <param name="particles"></param>
        /// <param name="latticeSizes"></param>
        /// <returns></returns>
        protected IDictionary<IParticle, double>[,,][] PrepareRawResult(IList<IParticle> particles, in Vector4I latticeSizes) =>
            throw new NotImplementedException();
    }
}