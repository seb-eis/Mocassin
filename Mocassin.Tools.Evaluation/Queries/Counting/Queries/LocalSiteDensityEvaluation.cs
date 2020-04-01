using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Mocassin.Framework.Extensions;
using Mocassin.Mathematics.ValueTypes;
using Mocassin.Model.Particles;
using Mocassin.Symmetry.Analysis;
using Mocassin.Tools.Evaluation.Context;
using Mocassin.Tools.Evaluation.Extensions;

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
        protected override IUnitCellProvider<IDictionary<IParticle, double>> GetValue(JobContext jobContext)
        {
            throw new NotImplementedException();
            var particleMap = jobContext.ModelContext.ModelProject.DataTracker.MapObjects<IParticle>();
            var vectorEncoder = jobContext.ModelContext.GetUnitCellVectorEncoder();
            var latticeSizes = jobContext.GetLatticeSizes();
            var lattice = jobContext.McsReader.ReadLattice();
            var rawResult = new IDictionary<IParticle, double>[latticeSizes.A, latticeSizes.B, latticeSizes.C][];

            return LatticeWrapping.ToSuperCell(rawResult, vectorEncoder);
        }

        /// <summary>
        ///     Prepares the raw result <see cref="IDictionary{TKey,TValue}"/> array system
        /// </summary>
        /// <param name="particles"></param>
        /// <param name="latticeSizes"></param>
        /// <returns></returns>
        protected IDictionary<IParticle, double>[,,][] PrepareRawResult(IList<IParticle> particles, in CrystalVector4D latticeSizes)
        {
            var result = new IDictionary<IParticle, double>[latticeSizes.A, latticeSizes.B, latticeSizes.C][];
            return result;
        }
    }
}