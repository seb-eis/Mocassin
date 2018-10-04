using System;
using System.Collections.Generic;
using System.Linq;

using ICon.Framework.Collections;
using ICon.Mathematics.Comparers;
using ICon.Mathematics.Permutation;
using ICon.Mathematics.Constraints;
using ICon.Symmetry.Analysis;
using ICon.Symmetry.SpaceGroups;
using ICon.Model.Structures;
using ICon.Model.Particles;

namespace ICon.Model.Energies
{
    /// <summary>
    /// Basic implementation of a pair interaction searcher within a unit cell provider (Bases upon space group service)
    /// </summary>
    public class PairInteractionFinder : IPairInteractionFinder
    {
        /// <summary>
        /// The unit cell provider used during the search
        /// </summary>
        public IUnitCellProvider<IUnitCellPosition> UnitCellProvider { get; protected set; }

        /// <summary>
        /// The space group service used for symmetry comparison
        /// </summary>
        public ISpaceGroupService SpaceGroupService { get; protected set; }

        /// <summary>
        /// Create new pair interaction finder with the specfified unit cell provider and space group service
        /// </summary>
        /// <param name="unitCellProvider"></param>
        /// <param name="spaceGroupService"></param>
        public PairInteractionFinder(IUnitCellProvider<IUnitCellPosition> unitCellProvider, ISpaceGroupService spaceGroupService)
        {
            UnitCellProvider = unitCellProvider ?? throw new ArgumentNullException(nameof(unitCellProvider));
            SpaceGroupService = spaceGroupService ?? throw new ArgumentNullException(nameof(spaceGroupService));
        }

        /// <summary>
        /// Creates the list of indexed unpolar pair interactions with stable positions restricted by the passed start positions stable environment information.
        /// Double comparer for tolerance comparisons during radial search
        /// </summary>
        /// <param name="positions"></param>
        /// <param name="environmentInfo"></param>
        /// <returns></returns>
        public IEnumerable<SymmetricPairInteraction> CreateUniqueSymmetricPairs(IEnumerable<IUnitCellPosition> positions, IStableEnvironmentInfo environmentInfo, DoubleComparer comparer)
        {
            var radialConstraint = new DoubleConstraint(false, 0.0, environmentInfo.MaxInteractionRange, true, comparer);
            var filterPredicate = GetIgnoredFilterPredicate(environmentInfo);
            var unfilteredPositions = CreateUniqueSymmetricPairs(positions, radialConstraint, value => value.IsValidAndStable());
            return FilterAndReindex(unfilteredPositions, filterPredicate, 0).ToList();
        }

        /// <summary>
        /// Creates the list of unique unpolar pair interaction objects for all passed unit cell positions within the provided radial constraint
        /// and general acceptance predicate
        /// </summary>
        /// <param name="positions"></param>
        /// <param name="radialConstraint"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IEnumerable<SymmetricPairInteraction> CreateUniqueSymmetricPairs(IEnumerable<IUnitCellPosition> positions, DoubleConstraint radialConstraint, Predicate<IUnitCellPosition> predicate)
        {
            var candidateDictionary = CreateUniquePairCandidateDictionary(positions, radialConstraint, predicate, false);
            var values = CreateUniquePairs(candidateDictionary, CreateSymmetricPair);
            return values.Cast<SymmetricPairInteraction>();
        }

        /// <summary>
        /// Takes a seqeunce of unstable environments and a double comparer to create the sequence of all existing asymmetric pair interactions.
        /// Interactions are sorted by start unit cell position and then distance
        /// </summary>
        /// <param name="unstableEnvironments"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public IEnumerable<AsymmetricPairInteraction> CreateUniqueAsymmetricPairs(IEnumerable<IUnstableEnvironment> unstableEnvironments, DoubleComparer comparer)
        {
            foreach (var environment in unstableEnvironments)
            {
                var radialConstraint = new DoubleConstraint(false, 0.0, environment.MaxInteractionRange, true, comparer);
                var predicate = MakeAsymmetricFilteringPredicate(environment, position => position.IsValidAndStable());
                var singleResult = CreateUniqueAsymmetricPairs(new IUnitCellPosition[] { environment.UnitCellPosition }, radialConstraint, predicate);
                foreach (var item in singleResult)
                {
                    yield return item;
                }
            }
        }

        /// <summary>
        /// Creates the list of unique polar pair interaction objects for all passed unit cell positions within the provided radial constraint
        /// and general acceptance predicate
        /// </summary>
        /// <param name="positions"></param>
        /// <param name="radialConstraint"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IEnumerable<AsymmetricPairInteraction> CreateUniqueAsymmetricPairs(IEnumerable<IUnitCellPosition> positions, DoubleConstraint radialConstraint, Predicate<IUnitCellPosition> predicate)
        {
            var candidateDictionary = CreateUniquePairCandidateDictionary(positions, radialConstraint, predicate, true);
            var values = CreateUniquePairs(candidateDictionary, CreateAsymmetricPair);
            return values.Cast<AsymmetricPairInteraction>();
        }

        /// <summary>
        /// Filters a seqeunce of pair interactions by the provided selector predicate and reindexes the remainder starting at the provided index
        /// </summary>
        /// <typeparam name="TInteraction"></typeparam>
        /// <param name="unfiltered"></param>
        /// <param name="selector"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        public IEnumerable<TInteraction> FilterAndReindex<TInteraction>(IEnumerable<TInteraction> unfiltered, Predicate<TInteraction> selector, int startIndex) where TInteraction : PairInteraction
        {
            int index = startIndex - 1;
            foreach (var interaction in unfiltered)
            {
                if (selector(interaction))
                {
                    interaction.Index = ++index;
                    yield return interaction;
                }
            }
        }

        /// <summary>
        /// Takes a pair candidate dictionary and a selector function to a seqeunce of unique pair interactions
        /// </summary>
        /// <param name="candidateDictionary"></param>
        /// <param name="pairMaker"></param>
        /// <returns></returns>
        protected IEnumerable<PairInteraction> CreateUniquePairs(IDictionary<double, List<PairCandidate>> candidateDictionary, Func<PairCandidate, SlotMachinePermuter<IParticle>, PairInteraction> pairMaker)
        {
            var permuters = new Dictionary<(IUnitCellPosition, IUnitCellPosition), SlotMachinePermuter<IParticle>>();
            foreach (var candidate in candidateDictionary.Values.SelectMany(list => list))
            {
                if (!permuters.TryGetValue((candidate.Position0, candidate.Position1), out var permuter))
                {
                    permuter = MakeParticlePermuter(candidate.Position0, candidate.Position1);
                    permuters[(candidate.Position0, candidate.Position1)] = permuter;
                }
                yield return pairMaker(candidate, permuter);
            }
        }

        /// <summary>
        /// Creates new unpolar pair interaction from pair candidate and affiliated particle permuter
        /// </summary>
        /// <param name="candidate"></param>
        /// <param name="particlePermuter"></param>
        /// <returns></returns>
        protected SymmetricPairInteraction CreateSymmetricPair(PairCandidate candidate, SlotMachinePermuter<IParticle> particlePermuter)
        {
            var energyDictionary = new Dictionary<SymmetricParticlePair, double>((int)particlePermuter.PermutationCount / 2);
            foreach (var key in particlePermuter.Select(value => new SymmetricParticlePair() { Particle0 = value[0], Particle1 = value[1] }))
            {
                energyDictionary[key] = 0.0;
            }
            return new SymmetricPairInteraction(candidate, energyDictionary);
        }

        /// <summary>
        /// Creates new polar pair interaction from pair candidate and affiliated particle permuter
        /// </summary>
        /// <param name="candidate"></param>
        /// <param name="particlePermuter"></param>
        /// <returns></returns>
        protected AsymmetricPairInteraction CreateAsymmetricPair(PairCandidate candidate, SlotMachinePermuter<IParticle> particlePermuter)
        {
            var energyDictionary = new Dictionary<AsymmetricParticlePair, double>((int)particlePermuter.PermutationCount / 2);
            foreach (var key in particlePermuter.Select(value => new AsymmetricParticlePair() { Particle0 = value[0], Particle1 = value[1] }))
            {
                energyDictionary[key] = 0.0;
            }
            return new AsymmetricPairInteraction(candidate, energyDictionary);
        }

        /// <summary>
        /// Crates and indices the list of unique pair interaction candidates for the provided position list, radial constraint and basic acceptance predicate
        /// that is sorted by their distance
        /// </summary>
        /// <param name="positions"></param>
        /// <param name="radialConstraint"></param>
        /// <param name="predicate"></param>
        /// <param name="skipInversionFiltering"></param>
        /// <returns></returns>
        protected IDictionary<double, List<PairCandidate>> CreateUniquePairCandidateDictionary(IEnumerable<IUnitCellPosition> positions, DoubleConstraint radialConstraint, Predicate<IUnitCellPosition> predicate, bool skipInversionFiltering)
        {
            var baseCandidateDictionary = CreateCandidateDictionary(positions, radialConstraint, predicate, skipInversionFiltering);
            var uniqueCandidateDictionary = CreateUniqueCandidateDictionary(baseCandidateDictionary, positions);
            return uniqueCandidateDictionary;
        }

        /// <summary>
        /// Creates a particle permuter for the two provided unit cell positions
        /// </summary>
        /// <param name="position0"></param>
        /// <param name="position1"></param>
        /// <returns></returns>
        public SlotMachinePermuter<IParticle> MakeParticlePermuter(IUnitCellPosition position0, IUnitCellPosition position1)
        {
            return new SlotMachinePermuter<IParticle>(position0.OccupationSet.GetParticles(), position1.OccupationSet.GetParticles());
        }

        /// <summary>
        /// Searches the set of passed positions for pair interaction within the rules of the contraint and predicate and returns a sorted seqeunce
        /// of the found cell entries (With optional inversion filtering)
        /// </summary>
        /// <param name="positions"></param>
        /// <param name="radialConstraint"></param>
        /// <param name="predicate"></param>
        /// <param name="skipInversionFiltering"></param>
        /// <returns></returns>
        protected SortedDictionary<double, List<PairCandidate>> CreateCandidateDictionary(IEnumerable<IUnitCellPosition> positions, DoubleConstraint radialConstraint, Predicate<IUnitCellPosition> predicate, bool skipInversionFiltering)
        {
            var resultDictionary = new SortedDictionary<double, List<PairCandidate>>(UnitCellProvider.VectorEncoder.Transformer.FractionalSystem.Comparer);
            var searchQueries = GetSearchQueries(positions, predicate, radialConstraint, skipInversionFiltering);

            foreach (var item in searchQueries)
            {
                item.Start();
            }
            foreach (var query in searchQueries)
            {
                InsertPairCandidates(resultDictionary, MakePairCandidates(query.StartCellEntry.Entry, query.Result));
            }
            return resultDictionary;
        }

        /// <summary>
        /// Filters the raw candidate dictionary for unqiue candidates and creates a new candidate dictiomnray that contains only these unique entries
        /// </summary>
        /// <param name="rawDictionary"></param>
        /// <returns></returns>
        protected SortedDictionary<double, List<PairCandidate>> CreateUniqueCandidateDictionary(SortedDictionary<double, List<PairCandidate>> rawDictionary, IEnumerable<IUnitCellPosition> positions)
        {
            var result = new SortedDictionary<double, List<PairCandidate>>();
            var operationDictionary = GetMultiplicityDictionary(positions);

            int baseIndex = -1;
            foreach (var item in rawDictionary)
            {
                var filtered = new List<PairCandidate>(10);
                foreach (var candidate in FilterCandidatesAndAssignIndices(item.Value, operationDictionary, baseIndex))
                {
                    baseIndex++;
                    filtered.Add(candidate);
                }
                result[item.Key] = filtered;
            }
            return result;
        }

        /// <summary>
        /// Filtres a raw candidtae list and assigns each new unique canidate a new interaction index
        /// </summary>
        /// <param name="rawCandidates"></param>
        /// <param name="operationDictionary"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        protected IEnumerable<PairCandidate> FilterCandidatesAndAssignIndices(List<PairCandidate> rawCandidates, IDictionary<IUnitCellPosition, IList<ISymmetryOperation>> operationDictionary, int startIndex)
        {
            var remaining = rawCandidates.Count;
            while (remaining > 0)
            {
                int orgRemaining = remaining;
                var uniqueCandidate = rawCandidates.First(value => value.Index == -1);

                var possibleVectors = operationDictionary[uniqueCandidate.Position0]
                    .Select(operation => (operation.ApplyUntrimmed(uniqueCandidate.PositionVector)));

                foreach (var vector in possibleVectors)
                {
                    int index = rawCandidates.FindIndex(value => SpaceGroupService.Comparer.Compare(vector, value.PositionVector) == 0);
                    if (index >= 0)
                    {
                        if (rawCandidates[index].Index == -1)
                        {
                            rawCandidates[index] = rawCandidates[index].CopyWithNewIndex(0);
                            remaining--;
                        }
                    }
                }
                yield return uniqueCandidate.CopyWithNewIndex(++startIndex);
            }
        }

        /// <summary>
        /// Creates the operation dictionary that contains the multiplicity operations for each unit cell position (With shift correction)
        /// </summary>
        /// <param name="positions"></param>
        /// <returns></returns>
        protected IDictionary<IUnitCellPosition, IList<ISymmetryOperation>> GetMultiplicityDictionary(IEnumerable<IUnitCellPosition> positions)
        {
            var result = new Dictionary<IUnitCellPosition, IList<ISymmetryOperation>>();
            foreach (var position in positions)
            {
                result[position] = SpaceGroupService.GetMultiplicityOperations(position.Vector, true);
            }
            return result;
        }

        /// <summary>
        /// Sorts the pair candidates into their affiliated distance category in the candidate dictionary
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="pairCandidates"></param>
        protected void InsertPairCandidates(SortedDictionary<double, List<PairCandidate>> dictionary, IEnumerable<PairCandidate> pairCandidates)
        {
            foreach (var candidate in pairCandidates)
            {
                if (!dictionary.ContainsKey(candidate.Distance))
                {
                    dictionary[candidate.Distance] = new List<PairCandidate>(50) { candidate };
                    continue;
                }
                dictionary[candidate.Distance].Add(candidate);
            }
        }

        /// <summary>
        /// Takes aseqeunce of found entries around a start position and creates the sqeunce of possible pair interaction candidates
        /// </summary>
        /// <param name="start"></param>
        /// <param name="positions"></param>
        /// <returns></returns>
        protected IEnumerable<PairCandidate> MakePairCandidates(IUnitCellPosition start, IEnumerable<CellEntry<IUnitCellPosition>> positions)
        {
            foreach (var position in positions)
            {
                double distance = UnitCellProvider.VectorEncoder.Transformer.ToCartesian(position.AbsoluteVector - start.Vector).GetLength();
                yield return new PairCandidate(start, position.Entry, position.AbsoluteVector, distance);
            }
        }

        /// <summary>
        /// Takes a set of start positions, radial search constraint and basic acceptance prediacte and create a sequnce of radial search queries to locate all
        /// cell entries just once (Inverse dubplicates are filtered if not specfified otherwise)
        /// </summary>
        /// <param name="starts"></param>
        /// <param name="predicate"></param>
        /// <param name="constraint"></param>
        /// <param name="skipInversionFiltering"></param>
        /// <returns></returns>
        protected RadialSearchQuery<IUnitCellPosition>[] GetSearchQueries(IEnumerable<IUnitCellPosition> starts, Predicate<IUnitCellPosition> predicate, DoubleConstraint constraint, bool skipInversionFiltering)
        {
            var positionSet = starts.ToArray();
            var result = new RadialSearchQuery<IUnitCellPosition>[positionSet.Length];
            for (int i = 0; i < positionSet.Length; i++)
            {
                result[i] = new RadialSearchQuery<IUnitCellPosition>()
                {
                    StartCellEntry = new CellEntry<IUnitCellPosition>(positionSet[i].Vector, positionSet[i]),
                    AcceptancePredicate =  (skipInversionFiltering) ? predicate : MakeInversionFilteringPredicate(positionSet, predicate, i),
                    UnitCellProvider = UnitCellProvider,
                    RadialConstraint = constraint
                };
            }
            return result;
        }

        /// <summary>
        /// Combines the orginal prediacte and the already searches unit cell positions into a combined acceptance predicate that follows the symmetric
        /// stable environment acceptance criteria
        /// </summary>
        /// <param name="positions"></param>
        /// <param name="orgPrediacte"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        protected Predicate<IUnitCellPosition> MakeInversionFilteringPredicate(IUnitCellPosition[] positions, Predicate<IUnitCellPosition> orgPrediacte, int index)
        {
            bool AcceptPredicate(IUnitCellPosition value)
            {
                for (int i = 0; i < index; i++)
                {
                    if (value.Index == positions[i].Index)
                    {
                        return false;
                    }
                }
                return orgPrediacte(value);
            }
            return AcceptPredicate;
        }

        /// <summary>
        /// Takes an unstable environment and the original capptance prediacte and create a combined predicate that follows the unstable environment asymmetric
        /// pair interaction acceptance criteria
        /// </summary>
        /// <param name="unstableEnvironment"></param>
        /// <param name="orgPredicate"></param>
        /// <returns></returns>
        protected Predicate<IUnitCellPosition> MakeAsymmetricFilteringPredicate(IUnstableEnvironment unstableEnvironment, Predicate<IUnitCellPosition> orgPredicate)
        {
            bool AcceptPredicate(IUnitCellPosition position)
            {
                if (unstableEnvironment.GetIgnoredPositions().Contains(position))
                {
                    return false;
                }
                return orgPredicate(position);
            }
            return AcceptPredicate;
        }

        /// <summary>
        /// Get the filter prediacte for ignored pair interactions defined in the stable environment info
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        protected Predicate<SymmetricPairInteraction> GetIgnoredFilterPredicate(IStableEnvironmentInfo info)
        {
            bool filterPredicate(SymmetricPairInteraction interaction)
            {
                return !info.GetIgnoredPairs().Any(pair => interaction.EnergyDictionary.Keys.Contains(pair));
            }
            return filterPredicate;
        }
    }
}
