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
using Mocassin.Tools.Evaluation.Queries.Data;

namespace Mocassin.Tools.Evaluation.Queries
{
    /// <summary>
    ///     Evaluation query to extract the <see cref="SiteCoordination" /> averaged for each <see cref="ICellSite" />
    /// </summary>
    public class CellSiteCoordinationEvaluation : SiteEnvironmentEvaluation<IDictionary<ICellSite, IDictionary<IParticle, SiteCoordination[]>>>
    {
        /// <summary>
        ///     Get or set the <see cref="Func{TResult}" /> that descries for which <see cref="ICellSite" /> that distribution is
        ///     created (Defaults to stable only)
        /// </summary>
        public Func<ICellSite, bool> SiteAcceptor { get; set; }

        /// <inheritdoc />
        public CellSiteCoordinationEvaluation(IEvaluableJobSet jobSet, double maxDistance)
            : base(jobSet, maxDistance)
        {
            SiteAcceptor = x => x.IsValidAndStable();
        }

        /// <inheritdoc />
        protected override IDictionary<ICellSite, IDictionary<IParticle, SiteCoordination[]>> GetValue(JobContext jobContext)
        {
            var targets = TargetDictionary[jobContext.JobModel.SimulationJobPackageModel];
            var sizeVector = jobContext.GetLatticeSizes();
            var particles = jobContext.ModelContext.ModelProject.Manager<IParticleManager>().DataAccess.Query(x => x.GetParticles());
            var cellProvider = jobContext.ModelContext.GetUnitCellProvider();
            var lattice = jobContext.McsReader.ReadLattice();
            var sites = cellProvider.GetUnitCell(0, 0, 0).GetAllEntries().ToList();
            var result = PrepareResultContainer(particles.Max(x => x.Index), particles, targets, sites);
            var distributionAccesses = CreateCoordinationQuickAccess(result);
            var particleCounts = new int[particles.Max(x => x.Index) + 1];

            foreach (var originVector in Vector4I.LatticeVectorSet(sizeVector))
            {
                var originParticleId = lattice[originVector.ToLinearIndex(sizeVector)];
                particleCounts[originParticleId]++;
                var site = sites[originVector.P].Content;
                if (!SiteAcceptor.Invoke(site)) continue;

                var targetData = targets[originVector.P];
                foreach (var target in targetData)
                {
                    var index = (originVector + target.CrystalVectorToTarget).ToTrimmed(sizeVector).ToLinearIndex(sizeVector);
                    var targetParticleId = lattice[index];
                    distributionAccesses[site.Index, originParticleId, targetParticleId - 1].Increment(target.DistanceInFm);
                }
            }

            result.Action(pair => pair.Value.Action(x => x.Value.Action(y => y.MakeResult(particleCounts[x.Key.Index])).Load()).Load()).Load();
            return result;
        }

        /// <summary>
        ///     Creates a 3D access array for the <see cref="SiteCoordination" /> instances by [SiteID, OriginParticleId,
        ///     TargetParticleId]
        /// </summary>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        protected SiteCoordination[,,] CreateCoordinationQuickAccess(IDictionary<ICellSite, IDictionary<IParticle, SiteCoordination[]>> dictionary)
        {
            var length0 = dictionary.Keys.Max(x => x.Index) + 1;
            var length1 = dictionary.Keys.Select(x => x.OccupationSet.Max(y => y.Index)).Max() + 1;
            var length2 = dictionary.Values.Select(x => x.Values.Select(y => y.Length).Max()).Max();
            var result = new SiteCoordination[length0, length1, length2];
            foreach (var pair in dictionary)
            {
                var index0 = pair.Key.Index;
                foreach (var item in pair.Value)
                {
                    var index1 = item.Key.Index;
                    for (var index2 = 0; index2 < item.Value.Length; index2++) result[index0, index1, index2] = item.Value[index2];
                }
            }

            return result;
        }

        /// <summary>
        ///     Prepares the result <see cref="Dictionary{TKey,TValue}" /> that stores the <see cref="SiteCoordination" /> set for
        ///     each requested <see cref="ICellSite" />
        /// </summary>
        /// <param name="length"></param>
        /// <param name="particles"></param>
        /// <param name="targets"></param>
        /// <param name="sites"></param>
        protected Dictionary<ICellSite, IDictionary<IParticle, SiteCoordination[]>> PrepareResultContainer(int length, IList<IParticle> particles,
            IDictionary<int, LatticeTarget[]> targets, IList<LatticePoint<ICellSite>> sites)
        {
            var result = new Dictionary<ICellSite, IDictionary<IParticle, SiteCoordination[]>>();
            var index = -1;
            foreach (var position in sites)
            {
                index++;
                if (result.ContainsKey(position.Content) || !SiteAcceptor(position.Content)) continue;
                var innerResult = new Dictionary<IParticle, SiteCoordination[]>();
                foreach (var originParticle in position.Content.OccupationSet)
                {
                    var distributions = new SiteCoordination[length];
                    foreach (var particle in particles)
                    {
                        if (particle.IsVoid) continue;
                        var distribution = new SiteCoordination(particle);
                        distribution.PrepareForCollection(targets[index]);
                        distributions[particle.Index - 1] = distribution;
                    }

                    innerResult.Add(originParticle, distributions);
                }

                result.Add(position.Content, innerResult);
            }

            return result;
        }
    }
}