using System.Collections.Generic;
using System.Linq;
using Mocassin.Model.Particles;
using Mocassin.Symmetry.Analysis;
using Mocassin.Tools.Evaluation.PlotData;

namespace Mocassin.Tools.Evaluation.Queries.Data
{
    /// <summary>
    ///     Describes a radial distribution around a position in the lattice as a set of <see cref="CoordinationShell" /> items
    /// </summary>
    public class SiteCoordination : IDatFileWritable<CoordinationShell>
    {
        /// <summary>
        ///     Get the temporary <see cref="Dictionary{TKey,TValue}" /> that stores the particle count information for each
        ///     distance in [fm]
        /// </summary>
        private Dictionary<int, int> TempCounters { get; set; }

        /// <summary>
        ///     The array of resulting <see cref="CoordinationShell" />. Gets set by calling <see cref="MakeResult" />
        /// </summary>
        public CoordinationShell[] Shells { get; private set; }

        /// <summary>
        ///     Get the array of <see cref="LatticeTarget" /> objects that was used for collection
        /// </summary>
        public LatticeTarget[] Targets { get; private set; }

        /// <summary>
        ///     Get the <see cref="IParticle" /> that the distribution belongs to
        /// </summary>
        public IParticle Particle { get; }

        /// <summary>
        ///     Get the number of samples sites used for averaging
        /// </summary>
        public int SiteCount { get; private set; }

        /// <summary>
        ///     Creates a new empty <see cref="SiteCoordination" /> for a <see cref="IParticle" />
        /// </summary>
        public SiteCoordination(IParticle particle)
        {
            Particle = particle;
        }

        /// <inheritdoc />
        public int GetDatEntriesPerLine() => 2;

        /// <inheritdoc />
        public string GetDatHeader(string format) => string.Format(format, "distance[Ang]", "CN");

        /// <inheritdoc />
        public IEnumerable<CoordinationShell> GetDatLines() => Shells;

        /// <summary>
        ///     Increments the counter for the provided distance without checking for existent key
        /// </summary>
        /// <param name="distanceInFm"></param>
        public void Increment(int distanceInFm)
        {
            TempCounters[distanceInFm]++;
        }

        /// <summary>
        ///     Prepare for collection for a set of <see cref="LatticeTarget" />
        /// </summary>
        /// <param name="targets"></param>
        public void PrepareForCollection(LatticeTarget[] targets)
        {
            Shells = null;
            TempCounters = new Dictionary<int, int>(targets.Length / 2);
            foreach (var target in targets)
            {
                var distance = target.DistanceInFm;
                if (TempCounters.ContainsKey(distance)) continue;
                TempCounters.Add(distance, 0);
            }

            Targets = targets;
        }

        /// <summary>
        ///     Converts the temporary counter system and sets the result using the provided site count
        /// </summary>
        public void MakeResult(int siteCount)
        {
            SiteCount = siteCount;

            var count = TempCounters.Count(pair => pair.Value != 0);
            var itemCount = (double) TempCounters.Sum(x => x.Value);

            var result = new CoordinationShell[count];
            foreach (var (pair, index) in TempCounters.Where(x => x.Value != 0).Select((x, i) => (x, i)))
            {
                var value = new CoordinationShell(pair.Key / 1e5, pair.Value / (double) siteCount);
                result[index] = value;
            }

            TempCounters = null;
            Shells = result;
        }
    }
}