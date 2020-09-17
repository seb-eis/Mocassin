using System;
using System.Collections.Generic;
using System.Linq;
using Mocassin.Framework.Extensions;
using Mocassin.Mathematics.Comparer;
using Mocassin.Mathematics.Constraints;
using Mocassin.Mathematics.Permutation;
using Mocassin.Model.Particles;
using Mocassin.Model.Structures;
using Mocassin.Symmetry.Analysis;
using Mocassin.Symmetry.SpaceGroups;

namespace Mocassin.Model.Energies
{
    /// <inheritdoc />
    public class PairInteractionFinder : IPairInteractionFinder
    {
        /// <inheritdoc />
        public IUnitCellProvider<ICellSite> UnitCellProvider { get; protected set; }

        /// <inheritdoc />
        public ISpaceGroupService SpaceGroupService { get; protected set; }

        /// <summary>
        ///     Create new pair interaction finder with the specified unit cell provider and space group service
        /// </summary>
        /// <param name="unitCellProvider"></param>
        /// <param name="spaceGroupService"></param>
        public PairInteractionFinder(IUnitCellProvider<ICellSite> unitCellProvider, ISpaceGroupService spaceGroupService)
        {
            UnitCellProvider = unitCellProvider ?? throw new ArgumentNullException(nameof(unitCellProvider));
            SpaceGroupService = spaceGroupService ?? throw new ArgumentNullException(nameof(spaceGroupService));
        }

        /// <inheritdoc />
        public IEnumerable<StablePairInteraction> SampleUniqueStablePairs(IEnumerable<ICellSite> positions,
            IStableEnvironmentInfo environmentInfo, NumericComparer comparer)
        {
            var radialConstraint = new NumericConstraint(false, 0.0, environmentInfo.MaxInteractionRange, true, comparer);
            var acceptPredicate = GetStableAcceptancePredicate(environmentInfo);
            var unfilteredPairs = FindAllStablePairs(positions, radialConstraint);
            var filteredPairs = FilterAndReindex(unfilteredPairs, acceptPredicate, 0);
            return FindAndAssignChiralPartners(filteredPairs, comparer);
        }

        /// <inheritdoc />
        public IEnumerable<UnstablePairInteraction> SampleUniqueUnstablePairs(IEnumerable<IUnstableEnvironment> unstableEnvironments,
            NumericComparer comparer)
        {
            var unfilteredPairs = new List<UnstablePairInteraction>();

            foreach (var environment in unstableEnvironments)
            {
                var radialConstraint = new NumericConstraint(false, 0.0, environment.MaxInteractionRange, true, comparer);
                var acceptPredicate = GetUnstableAcceptancePredicate(environment);
                var singleResult = FindAllUnstablePairs(new[] {environment.CellSite}, radialConstraint);
                unfilteredPairs.AddRange(singleResult.Where(x => acceptPredicate(x)));
            }

            return FilterAndReindex(unfilteredPairs, x => true, 0);
        }

        /// <summary>
        ///     Fins chiral partner <see cref="StablePairInteraction" /> entries and assigns the affiliated property
        /// </summary>
        /// <param name="source"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public IEnumerable<StablePairInteraction> FindAndAssignChiralPartners(IEnumerable<StablePairInteraction> source, NumericComparer comparer)
        {
            var sourceList = source.AsList();
            for (var i = 0; i < sourceList.Count; i++)
            {
                var first = sourceList[i];
                if (first.ChiralPartner != null) continue;
                for (var j = i; j < sourceList.Count; j++)
                {
                    var second = sourceList[j];
                    var compValue = comparer.Compare(first.Distance, second.Distance);
                    if (compValue < 0) continue;
                    if (compValue > 0) break;
                    if (!CheckIsChiralPair(first, second)) continue;
                    first.SetChiralPartner(second);
                }
            }

            return sourceList;
        }

        /// <summary>
        ///     Checks if the two provided <see cref="StablePairInteraction" /> entries meet the conditions to be chiral pairs
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        protected bool CheckIsChiralPair(StablePairInteraction first, StablePairInteraction second)
        {
            if (first.Position0 != first.Position1 || second.Position0 != second.Position1 || first.Position0 != second.Position1) return false;
            return SpaceGroupService.CheckInteractionGeometryIsChiralPair(first.Position0.Vector, first.SecondPositionVector,
                second.Position0.Vector, second.SecondPositionVector);
        }

        /// <summary>
        ///     Builds the <see cref="IEnumerable{T}" /> of <see cref="StablePairInteraction" /> instances within the provided
        ///     radial <see cref="NumericConstraint" />
        /// </summary>
        /// <param name="positions"></param>
        /// <param name="radialConstraint"></param>
        /// <returns></returns>
        protected IEnumerable<StablePairInteraction> FindAllStablePairs(IEnumerable<ICellSite> positions,
            NumericConstraint radialConstraint)
        {
            static bool Predicate(ICellSite position) => position.IsValidAndStable();

            var candidateDictionary = CreateUniquePairCandidateDictionary(positions, radialConstraint, Predicate, false);
            var values = CreateUniquePairs(candidateDictionary, CreateStablePairInteraction);
            return values.Cast<StablePairInteraction>();
        }

        /// <summary>
        ///     Builds the <see cref="IEnumerable{T}" /> of <see cref="UnstablePairInteraction" /> instances within the provided
        ///     radial <see cref="NumericConstraint" />
        /// </summary>
        /// <param name="positions"></param>
        /// <param name="radialConstraint"></param>
        /// <returns></returns>
        public IEnumerable<UnstablePairInteraction> FindAllUnstablePairs(IEnumerable<ICellSite> positions,
            NumericConstraint radialConstraint)
        {
            static bool Predicate(ICellSite position) => position.IsValidAndStable();

            var candidateDictionary = CreateUniquePairCandidateDictionary(positions, radialConstraint, Predicate, true);
            var values = CreateUniquePairs(candidateDictionary, CreateUnstablePairInteraction);
            return values.Cast<UnstablePairInteraction>();
        }

        /// <summary>
        ///     Filters a sequence of pair interactions by the provided selector predicate and reindex the remainder starting at
        ///     the provided index
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="unfiltered"></param>
        /// <param name="selector"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        public IEnumerable<T> FilterAndReindex<T>(IEnumerable<T> unfiltered, Predicate<T> selector, int startIndex) where T : PairInteraction
        {
            var index = startIndex - 1;
            foreach (var interaction in unfiltered)
            {
                if (!selector(interaction)) continue;
                interaction.Index = ++index;
                yield return interaction;
            }
        }

        /// <summary>
        ///     Takes a pair candidate dictionary and a selector function to create the unique set of
        ///     <see cref="PairInteraction" /> instances
        /// </summary>
        /// <param name="candidateDictionary"></param>
        /// <param name="pairMaker"></param>
        /// <returns></returns>
        protected IEnumerable<PairInteraction> CreateUniquePairs(IDictionary<double, List<PairCandidate>> candidateDictionary,
            Func<PairCandidate, IPermutationSource<IParticle>, PairInteraction> pairMaker)
        {
            var permutationSources = new Dictionary<(ICellSite, ICellSite), IPermutationSource<IParticle>>();
            foreach (var candidate in candidateDictionary.Values.SelectMany(list => list))
            {
                if (!permutationSources.TryGetValue((candidate.Position0, candidate.Position1), out var permutationSource))
                {
                    permutationSource = CreateParticlePermutationSource(candidate.Position0, candidate.Position1);
                    permutationSources[(candidate.Position0, candidate.Position1)] = permutationSource;
                }

                yield return pairMaker(candidate, permutationSource);
            }
        }

        /// <summary>
        ///     Creates new <see cref="StablePairInteraction" /> from <see cref="PairCandidate" /> and a
        ///     <see cref="IPermutationSource{T1}" /> for the occupation sets
        /// </summary>
        /// <param name="candidate"></param>
        /// <param name="particleSource"></param>
        /// <returns></returns>
        protected StablePairInteraction CreateStablePairInteraction(PairCandidate candidate, IPermutationSource<IParticle> particleSource)
        {
            var energyDictionary = new Dictionary<ParticleInteractionPair, double>((int) particleSource.PermutationCount / 2);
            // ToDo: Remove the always symmetric workaround as soon as the system supports asymmetric stable interactions
            foreach (var key in particleSource.Select(value => ParticleInteractionPair.MakePair(value[0], value[1], true)))
                energyDictionary[key] = 0.0;

            return new StablePairInteraction(candidate, energyDictionary);
        }

        /// <summary>
        ///     Creates new <see cref="UnstablePairInteraction" /> from <see cref="PairCandidate" /> and a
        ///     <see cref="IPermutationSource{T1}" /> for the occupation sets
        /// </summary>
        /// <param name="candidate"></param>
        /// <param name="particleSource"></param>
        /// <returns></returns>
        protected UnstablePairInteraction CreateUnstablePairInteraction(PairCandidate candidate, IPermutationSource<IParticle> particleSource)
        {
            var energyDictionary = new Dictionary<ParticleInteractionPair, double>((int) particleSource.PermutationCount / 2);
            foreach (var key in particleSource.Select(value => ParticleInteractionPair.MakePair(value[0], value[1], candidate.IsSymmetric)))
                energyDictionary[key] = 0.0;

            return new UnstablePairInteraction(candidate, energyDictionary);
        }

        /// <summary>
        ///     Crates and indices the list of unique pair interaction candidates for the provided position list, radial constraint
        ///     and basic acceptance predicate that is sorted by their distance
        /// </summary>
        /// <param name="positions"></param>
        /// <param name="radialConstraint"></param>
        /// <param name="predicate"></param>
        /// <param name="skipInversionFiltering"></param>
        /// <returns></returns>
        protected IDictionary<double, List<PairCandidate>> CreateUniquePairCandidateDictionary(IEnumerable<ICellSite> positions,
            NumericConstraint radialConstraint, Func<ICellSite, bool> predicate, bool skipInversionFiltering)
        {
            if (!(positions is ICollection<ICellSite> positionCollection))
                positionCollection = positions.ToList();

            var baseCandidateDictionary = CreateCandidateDictionary(positionCollection, radialConstraint, predicate, skipInversionFiltering);
            var uniqueCandidateDictionary = CreateUniqueCandidateDictionary(baseCandidateDictionary, positionCollection);
            return uniqueCandidateDictionary;
        }

        /// <summary>
        ///     Creates a particle permutation source for the two provided unit cell positions
        /// </summary>
        /// <param name="position0"></param>
        /// <param name="position1"></param>
        /// <returns></returns>
        public IPermutationSource<IParticle> CreateParticlePermutationSource(ICellSite position0, ICellSite position1) =>
            new PermutationSlotMachine<IParticle>(position0.OccupationSet.GetParticles(), position1.OccupationSet.GetParticles());

        /// <summary>
        ///     Searches the set of passed positions for pair interaction within the rules of the constraint and predicate and
        ///     returns a sorted sequence
        ///     of the found cell entries (With optional inversion filtering)
        /// </summary>
        /// <param name="positions"></param>
        /// <param name="radialConstraint"></param>
        /// <param name="predicate"></param>
        /// <param name="skipInversionFiltering"></param>
        /// <returns></returns>
        protected SortedDictionary<double, List<PairCandidate>> CreateCandidateDictionary(IEnumerable<ICellSite> positions,
            NumericConstraint radialConstraint, Func<ICellSite, bool> predicate, bool skipInversionFiltering)
        {
            var resultDictionary = new SortedDictionary<double, List<PairCandidate>>(UnitCellProvider.VectorEncoder.Transformer.FractionalSystem.Comparer);
            var searchQueries = GetSearchQueries(positions, radialConstraint, predicate, skipInversionFiltering);

            foreach (var item in searchQueries) item.Start();

            foreach (var query in searchQueries) InsertPairCandidates(resultDictionary, MakePairCandidates(query.OriginLatticePoint.Content, query.Result));

            return resultDictionary;
        }

        /// <summary>
        ///     Filters the raw candidate dictionary for unique candidates and creates a new candidate dictionary that contains
        ///     only these unique entries
        /// </summary>
        /// <param name="rawDictionary"></param>
        /// <param name="positions"></param>
        /// <returns></returns>
        protected SortedDictionary<double, List<PairCandidate>> CreateUniqueCandidateDictionary(
            SortedDictionary<double, List<PairCandidate>> rawDictionary, IEnumerable<ICellSite> positions)
        {
            var result = new SortedDictionary<double, List<PairCandidate>>();
            var operationDictionary = GetMultiplicityDictionary(positions);

            var baseIndex = -1;
            foreach (var item in rawDictionary)
            {
                var filtered = new List<PairCandidate>(10);
                var candidates = FilterCandidatesAndAssignIndices(item.Value, operationDictionary, baseIndex).ToList();
                foreach (var candidate in candidates)
                {
                    baseIndex++;
                    filtered.Add(candidate);
                }

                result[item.Key] = filtered;
            }

            return result;
        }

        /// <summary>
        ///     Filters a raw candidate list and assigns each new unique candidate a new interaction index
        /// </summary>
        /// <param name="rawCandidates"></param>
        /// <param name="operationDictionary"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        protected IEnumerable<PairCandidate> FilterCandidatesAndAssignIndices(List<PairCandidate> rawCandidates,
            IDictionary<ICellSite, IList<ISymmetryOperation>> operationDictionary, int startIndex)
        {
            var remaining = rawCandidates.Count;
            while (remaining > 0)
            {
                var uniqueCandidate = rawCandidates.First(value => value.Index == -1);

                var possibleVectors = operationDictionary[uniqueCandidate.Position0]
                    .Select(operation => operation.Transform(uniqueCandidate.PositionVector));

                foreach (var vector in possibleVectors)
                {
                    bool IsCandidateMatch(PairCandidate value) =>
                        value.Position0 == uniqueCandidate.Position0
                        && SpaceGroupService.Comparer.Compare(vector, value.PositionVector) == 0;

                    var index = rawCandidates.FindIndex(IsCandidateMatch);
                    if (index < 0)
                        continue;

                    if (rawCandidates[index].Index != -1)
                        continue;

                    rawCandidates[index] = rawCandidates[index].CopyWithNewIndex(0);
                    remaining--;
                }

                yield return uniqueCandidate.CopyWithNewIndex(++startIndex);
            }
        }

        /// <summary>
        ///     Creates the operation dictionary that contains the multiplicity operations for each unit cell position (With shift
        ///     correction)
        /// </summary>
        /// <param name="positions"></param>
        /// <returns></returns>
        protected IDictionary<ICellSite, IList<ISymmetryOperation>> GetMultiplicityDictionary(
            IEnumerable<ICellSite> positions)
        {
            var result = new Dictionary<ICellSite, IList<ISymmetryOperation>>();
            foreach (var position in positions)
                result[position] = SpaceGroupService.GetOperationsNotShiftingOrigin(position.Vector, true);

            return result;
        }

        /// <summary>
        ///     Sorts the pair candidates into their affiliated distance category in the candidate dictionary
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="pairCandidates"></param>
        protected void InsertPairCandidates(SortedDictionary<double, List<PairCandidate>> dictionary,
            IEnumerable<PairCandidate> pairCandidates)
        {
            foreach (var candidate in pairCandidates)
            {
                if (!dictionary.ContainsKey(candidate.Distance))
                {
                    dictionary[candidate.Distance] = new List<PairCandidate>(50) {candidate};
                    continue;
                }

                dictionary[candidate.Distance].Add(candidate);
            }
        }

        /// <summary>
        ///     Takes a sequence of found entries around a start position and creates the sequence of possible pair interaction
        ///     candidates
        /// </summary>
        /// <param name="start"></param>
        /// <param name="positions"></param>
        /// <returns></returns>
        protected IEnumerable<PairCandidate> MakePairCandidates(ICellSite start, IEnumerable<LatticePoint<ICellSite>> positions)
        {
            foreach (var position in positions)
            {
                var distance = UnitCellProvider.VectorEncoder.Transformer
                                               .ToCartesian(position.Fractional - start.Vector)
                                               .GetLength();

                yield return new PairCandidate(start, position.Content, position.Fractional, distance);
            }
        }

        /// <summary>
        ///     Takes a set of start positions, radial search constraint and acceptance predicate and create a sequence of
        ///     radial search queries to locate all cell entries just once (Inverse duplicates are filtered if not specified
        ///     otherwise)
        /// </summary>
        /// <param name="starts"></param>
        /// <param name="constraint"></param>
        /// <param name="predicate"></param>
        /// <param name="skipInversionFiltering"></param>
        /// <returns></returns>
        protected RadialLatticePointQuery<ICellSite>[] GetSearchQueries(IEnumerable<ICellSite> starts, NumericConstraint constraint,
            Func<ICellSite, bool> predicate, bool skipInversionFiltering)
        {
            var positionSet = starts.ToList();
            var result = new RadialLatticePointQuery<ICellSite>[positionSet.Count];
            for (var i = 0; i < positionSet.Count; i++)
            {
                result[i] = new RadialLatticePointQuery<ICellSite>
                {
                    OriginLatticePoint = new LatticePoint<ICellSite>(positionSet[i].Vector, positionSet[i]),
                    AcceptancePredicate = skipInversionFiltering
                        ? predicate
                        : MakeSymmetricInversionAcceptancePredicate(positionSet, predicate, i),

                    UnitCellProvider = UnitCellProvider,
                    RadialConstraint = constraint
                };
            }

            return result;
        }

        /// <summary>
        ///     Combines the original predicate and the already searched unit cell positions into a combined acceptance predicate
        ///     that follows the symmetric stable environment acceptance criteria
        /// </summary>
        /// <param name="positions"></param>
        /// <param name="orgPredicate"></param>
        /// <param name="currentSearchIndex"></param>
        /// <returns></returns>
        protected Func<ICellSite, bool> MakeSymmetricInversionAcceptancePredicate(IList<ICellSite> positions,
            Func<ICellSite, bool> orgPredicate, int currentSearchIndex)
        {
            bool AcceptPredicate(ICellSite position)
            {
                return positions.Take(currentSearchIndex).All(x => x != position) && orgPredicate(position);
            }

            return AcceptPredicate;
        }

        /// <summary>
        ///     Creates the acceptance <see cref="Predicate{T}" /> for an <see cref="UnstablePairInteraction" /> from its
        ///     <see cref="IUnstableEnvironment" /> definition
        /// </summary>
        /// <param name="environment"></param>
        /// <returns></returns>
        protected Predicate<UnstablePairInteraction> GetUnstableAcceptancePredicate(IUnstableEnvironment environment)
        {
            bool AcceptPredicate(UnstablePairInteraction interaction)
            {
                return !environment.GetInteractionFilters().Any(x => x.IsApplicable(interaction));
            }

            return AcceptPredicate;
        }

        /// <summary>
        ///     Creates the acceptance <see cref="Predicate{T}" /> for an <see cref="StablePairInteraction" /> from the
        ///     <see cref="IStableEnvironmentInfo" />
        /// </summary>
        /// <param name="environment"></param>
        /// <returns></returns>
        protected Predicate<StablePairInteraction> GetStableAcceptancePredicate(IStableEnvironmentInfo environment)
        {
            bool AcceptPredicate(StablePairInteraction interaction)
            {
                return !environment.GetInteractionFilters().Any(x => x.IsApplicable(interaction));
            }

            return AcceptPredicate;
        }
    }
}