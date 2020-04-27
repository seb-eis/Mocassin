using System;
using System.Collections.Generic;
using System.Linq;
using Mocassin.Framework.Extensions;
using Mocassin.Mathematics.ValueTypes;
using Mocassin.Model.Particles;
using Mocassin.Model.Structures;
using Mocassin.Symmetry.Analysis;
using Mocassin.Tools.Evaluation.Context;
using Mocassin.Tools.Evaluation.Extensions;

namespace Mocassin.Tools.Evaluation.Queries
{
    /// <summary>
    ///     Evaluation query to extract the <see cref="SiteCoordination" /> around single points within the lattice
    /// </summary>
    public class LocalSiteCoordinationEvaluation : SiteEnvironmentEvaluation<IUnitCellProvider<SiteCoordination[]>>
    {
        /// <summary>
        ///     Get or set the <see cref="Func{T,TResult}" /> that defines which positions are included in the result. Defaults to
        ///     stable <see cref="PositionStability" /> only
        /// </summary>
        public Func<Vector4I, FractionalPosition, bool> OriginAcceptor { get; set; }

        /// <inheritdoc />
        public LocalSiteCoordinationEvaluation(IEvaluableJobSet jobSet, double maxDistance)
            : base(jobSet, maxDistance)
        {
            OriginAcceptor = (a, b) => b.Stability == PositionStability.Stable;
        }

        /// <inheritdoc />
        protected override IUnitCellProvider<SiteCoordination[]> GetValue(JobContext jobContext)
        {
            var targets = TargetDictionary[jobContext.JobModel.SimulationJobPackageModel];
            var latticeSizes = jobContext.GetLatticeSizes();
            var particles = jobContext.ModelContext.ModelProject.Manager<IParticleManager>().DataAccess.Query(x => x.GetParticles());
            var sites = jobContext.ModelContext.ModelProject.Manager<IStructureManager>().DataAccess.Query(x => x.GetLinearizedExtendedPositionList());
            var lattice = jobContext.McsReader.ReadLattice();
            var distributions = new SiteCoordination[particles.Max(x => x.Index) + 1];
            var vectorEncoder = jobContext.ModelContext.GetUnitCellVectorEncoder();
            var rawResult = new SiteCoordination[latticeSizes.A, latticeSizes.B, latticeSizes.C][][];
            var dummyCell = new SiteCoordination[latticeSizes.P][].Populate(new SiteCoordination[0]);

            foreach (var origin in Vector4I.LatticeVectorSet(latticeSizes))
            {
                if (!OriginAcceptor.Invoke(origin, sites[origin.P]))
                {
                    rawResult[origin.A, origin.B, origin.C] ??= dummyCell;
                    continue;
                }

                rawResult[origin.A, origin.B, origin.C] ??= new SiteCoordination[latticeSizes.P][];

                var targetData = targets[origin.P];
                PrepareDistributionsForCollection(distributions, particles, targetData);
                foreach (var target in targetData)
                {
                    var targetVector = (origin + target.CrystalVectorToTarget).ToTrimmed(latticeSizes);
                    var latticeIndex = targetVector.ToLinearIndex(latticeSizes);
                    var particleId = lattice[latticeIndex];
                    distributions[particleId].Increment(target.DistanceInFm);
                }

                var value = distributions.Skip(1).Action(x => x.MakeResult(1)).ToArray(distributions.Length - 1);
                rawResult[origin.A, origin.B, origin.C][origin.P] = value;
            }

            return LatticeWrapping.ToSuperCell(rawResult, vectorEncoder);
        }

        /// <summary>
        ///     Initializes the raw <see cref="SiteCoordination" /> instances for all particles
        /// </summary>
        /// <param name="distributions"></param>
        /// <param name="particles"></param>
        /// <param name="targets"></param>
        protected void PrepareDistributionsForCollection(SiteCoordination[] distributions, IList<IParticle> particles, LatticeTarget[] targets)
        {
            foreach (var particle in particles)
            {
                if (particle.IsVoid) continue;
                var distribution = new SiteCoordination(particle);
                distribution.PrepareForCollection(targets);
                distributions[particle.Index] = distribution;
            }
        }
    }
}