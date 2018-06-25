using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

using ICon.Framework.Extensions;
using ICon.Framework.Collections;
using ICon.Framework.SQLiteCore;
using ICon.Framework.Exceptions;
using ICon.Symmetry.CrystalSystems;
using ICon.Mathematics.ValueTypes;
using ICon.Mathematics.Comparers;
using ICon.Mathematics.Extensions;


namespace ICon.Symmetry.SpaceGroups
{
    /// <summary>
    /// ICon symmetry service that handles space group database access and creation of affiliated cyrstal systems
    /// </summary>
    public class SpaceGroupService : ISpaceGroupService
    {
        /// <summary>
        /// The space group context provider
        /// </summary>
        public SQLiteContextProvider<SpaceGroupContext> ContextProvider { get; set; }

        /// <summary>
        /// The currently loaded space group object
        /// </summary>
        public SpaceGroupEntity LoadedGroupEntity { get; set; }

        /// <summary>
        /// Get the currently looded space group as a space group interface
        /// </summary>
        public ISpaceGroup LoadedGroup => LoadedGroupEntity;

        /// <summary>
        /// The equality comparator which contains the almost equal information for the double vectors
        /// </summary>
        public VectorComparer3D<Fractional3D> VectorComparer { get; set; }

        /// <summary>
        /// The internal vector comparer as a comparer interface
        /// </summary>
        public IComparer<Fractional3D> Comparer => VectorComparer;

        /// <summary>
        /// Creates new space group service using the default space group context, database filepath and double comparer
        /// </summary>
        /// <param name="dbFilepath"></param>
        /// <param name="comparer"></param>
        public SpaceGroupService(string dbFilepath, IComparer<double> comparer)
        {
            ContextProvider = new SpaceGroupContextProvider(dbFilepath);
            VectorComparer = new VectorComparer3D<Fractional3D>(comparer);
        }

        /// <summary>
        /// Creates new space group service using the default space group context, database path and tolerance value for comparisons
        /// </summary>
        /// <param name="dbFilepath"></param>
        /// <param name="tolerance"></param>
        public SpaceGroupService(string dbFilepath, double tolerance) : this(dbFilepath, DoubleComparer.CreateRanged(tolerance))
        {
            TryLoadGroup((group) => group.Index == 1);
        }

        /// <summary>
        /// Creates a new space group service from the default databse path
        /// </summary>
        /// <param name="comparer"></param>
        public SpaceGroupService(IComparer<double> comparer)
        {
            ContextProvider = new SpaceGroupContextProvider();
            VectorComparer = new VectorComparer3D<Fractional3D>(comparer);
        }

        /// <summary>
        /// Creates new space group service from custom SQLiteCoreContextProvider
        /// </summary>
        /// <param name="contextProvider"></param>
        /// <param name="comparer"></param>
        public SpaceGroupService(SQLiteContextProvider<SpaceGroupContext> contextProvider, IComparer<double> comparer)
        {
            ContextProvider = contextProvider ?? throw new ArgumentNullException(nameof(contextProvider));
            VectorComparer = new VectorComparer3D<Fractional3D>(comparer);
        }

        /// <summary>
        ///  Tries to load a space group from database into the service that matches the provided predicate (Throws on ambiguous predicate)
        /// </summary>
        /// <param name="searchPredicate"></param>
        /// <returns></returns>
        public bool TryLoadGroup(Predicate<ISpaceGroup> searchPredicate)
        {
            SpaceGroupEntity newGroup = null;
            using (SpaceGroupContext context = ContextProvider.CreateContext())
            {
                newGroup = context.SpaceGroups
                    .Where(a => searchPredicate(a))
                    .Include(g => g.BaseSymmetryOperations)
                    .SingleOrDefault();

                if (LoadedGroupEntity == null)
                {
                    LoadedGroupEntity = newGroup;
                }

                if (newGroup != null && !newGroup.GetGroupEntry().Equals(LoadedGroupEntity.GetGroupEntry()))
                {
                    LoadedGroupEntity = (!newGroup.GetGroupEntry().Equals(LoadedGroupEntity.GetGroupEntry())) ? newGroup : LoadedGroupEntity;
                }
            }
            return (newGroup == null) ? false : true;
        }

        /// <summary>
        /// Loads a group utilizing the provided space group identifier (Returns false if not possible and true on successfull loading or if the group is already loaded)
        /// </summary>
        /// <param name="groupEntry"></param>
        /// <returns></returns>
        public bool TryLoadGroup(SpaceGroupEntry groupEntry)
        {
            if (groupEntry == null)
            {
                throw new ArgumentNullException(nameof(groupEntry));
            }
            if (LoadedGroupEntity != null && groupEntry.Equals(LoadedGroupEntity.GetGroupEntry()))
            {
                return true;
            }
            return TryLoadGroup(group => group.Index == groupEntry.Index && group.Specifier == groupEntry.Specifier);
        }

        /// <summary>
        /// Creates a new cyrstal system matching the currently loaded group using a crystal system provider
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public CrystalSystem CreateCrystalSystem(ICrystalSystemProvider provider)
        {
            if (LoadedGroupEntity == null)
            {
                throw new InvalidObjectStateException("There is currently no group loaded into the service, cannot create crytsal system");
            }
            return provider.Create(LoadedGroupEntity);
        }

        /// <summary>
        /// Creates a sorted set list of all possible space groups as read only interfaces
        /// </summary>
        /// <returns></returns>
        public SetList<ISpaceGroup> GetFullGroupList()
        {
            var setList = new SetList<ISpaceGroup>(Comparer<ISpaceGroup>.Default, 274);
            using (SpaceGroupContext context = ContextProvider.CreateContext())
            {
                foreach (var group in context.SpaceGroups)
                {
                    setList.Add(group);
                }
            }
            return setList;
        }

        /// <summary>
        /// Returns an ordered unique set list that contains all wyckoff positions equivalent to the provided one
        /// </summary>
        /// <param name="refVector"></param>
        /// <returns></returns>
        public SetList<Fractional3D> GetAllWyckoffPositions(Fractional3D refVector)
        {
            return CreatePositionSetList(refVector);
        }

        /// <summary>
        /// Returns the wyckoff extended sorted unique lists for multiple refernce vectors
        /// </summary>
        /// <param name="refVectors"></param>
        /// <returns></returns>
        public List<SetList<Fractional3D>> GetAllWyckoffPositions(IEnumerable<Fractional3D> refVectors)
        {
            if (refVectors == null)
            {
                throw new ArgumentNullException(nameof(refVectors));
            }
            return refVectors.Select((vector) => GetAllWyckoffPositions(vector)).ToList();
        }

        /// <summary>
        /// Returns an ordered unique set list that contains all wyckoff positions equivalent to the provided one (From generic vector)
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="refVector"></param>
        /// <returns></returns>
        public SetList<TSource> GetAllWyckoffPositions<TSource>(TSource refVector) where TSource : struct, IFractional3D<TSource>
        {
            return CreatePositionSetList(refVector);
        }

        /// <summary>
        /// Returns an ordere list of ordered unique extended wyckoff position sets that are equivaelnt in vector type to the original one
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="refVectors"></param>
        /// <returns></returns>
        public List<SetList<TSource>> GetAllWyckoffPositions<TSource>(IEnumerable<TSource> refVectors) where TSource : struct, IFractional3D<TSource>
        {
            if (refVectors == null)
            {
                throw new ArgumentNullException(nameof(refVectors));
            }
            return refVectors.Select((vector) => GetAllWyckoffPositions(vector)).ToList();
        }


        /// <summary>
        /// Creates a basic fractional position set list from a basic fractional vector
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        protected SetList<Fractional3D> CreatePositionSetList(Fractional3D vector)
        {
            var results = new SetList<Fractional3D>(VectorComparer, LoadedGroupEntity.BaseSymmetryOperations.Count);
            foreach (SymmetryOperationEntity operation in LoadedGroupEntity.BaseSymmetryOperations)
            {
                results.Add(operation.ApplyWithTrim(vector));
            }
            results.List.TrimExcess();
            return results;
        }

        /// <summary>
        /// Creates a basic fractional position set list from double coordinate values
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        protected SetList<Fractional3D> CreatePositionSetList(double a, double b, double c)
        {
            var results = new SetList<Fractional3D>(VectorComparer, LoadedGroupEntity.BaseSymmetryOperations.Count);
            foreach (SymmetryOperationEntity operation in LoadedGroupEntity.BaseSymmetryOperations)
            {
                results.Add(operation.ApplyWithTrim(a, b, c));
            }
            results.List.TrimExcess();
            return results;
        }

        /// <summary>
        /// Creates a source type fractional position set list where all results carry the original information of the source
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="vector"></param>
        /// <returns></returns>
        protected SetList<TSource> CreatePositionSetList<TSource>(TSource vector) where TSource : struct, IFractional3D<TSource>
        {
            var results = new SetList<TSource>(VectorComparer.MakeCompatibleComparer<TSource>(), LoadedGroupEntity.BaseSymmetryOperations.Count);
            foreach (SymmetryOperationEntity operation in LoadedGroupEntity.BaseSymmetryOperations)
            {
                results.Add(operation.ApplyWithTrim(vector));
            }
            results.List.TrimExcess();
            return results;
        }

        /// <summary>
        /// Get all wyckoff sequences for the provided refernce seqeunce of fractional vectors
        /// </summary>
        /// <param name="refSequence"></param>
        /// <returns></returns>
        public SetList<Fractional3D[]> GetWyckoffSequences(params Fractional3D[] refSequence)
        {
            Fractional3D[] MoveStartToUnitCell(Fractional3D[] vectors)
            {
                double coorA = vectors[0].A.PeriodicTrim(0.0, 1.0, 1.0e-10) - vectors[0].A;
                double coorB = vectors[0].B.PeriodicTrim(0.0, 1.0, 1.0e-10) - vectors[0].B;
                double coorC = vectors[0].C.PeriodicTrim(0.0, 1.0, 1.0e-10) - vectors[0].C;
                var shift = new Fractional3D(coorA, coorB, coorC);
                return vectors.Select(a => a + shift).ToArray();
            }

            var comparer = Comparer<Fractional3D[]>.Create((a, b) => a.LexicographicCompare(b, VectorComparer));
            var result = new SetList<Fractional3D[]>(comparer);

            foreach (var operation in LoadedGroupEntity.BaseSymmetryOperations)
            {
                result.Add(MoveStartToUnitCell(refSequence.Select(a => operation.ApplyUntrimmed(a.A, a.B, a.C)).ToArray()));
            }

            return result;
        }

        /// <summary>
        /// Gets the unfiltered and untrimmed list of all wyckoff extended sequences symmetry equivalent to the input sequence
        /// </summary>
        /// <param name="refSequence"></param>
        /// <returns></returns>
        public IList<Fractional3D[]> GetAllWyckoffSequences(IEnumerable<Fractional3D> refSequence)
        {
            return LoadedGroupEntity.BaseSymmetryOperations.Select(operation => refSequence.Select(vector => operation.ApplyUntrimmed(vector)).ToArray()).ToList();
        }

        /// <summary>
        /// Gets the filtered and trimmed list of all wyckoff extended seqeunces that begin in the origin unit cell (0,0,0)
        /// </summary>
        /// <param name="refSequence"></param>
        /// <returns></returns>
        public SetList<Fractional3D[]> GetAllWyckoffOriginSequences(IEnumerable<Fractional3D> refSequence)
        {
            var comparer = Comparer<Fractional3D[]>.Create((a, b) => a.LexicographicCompare(b, VectorComparer));
            return new SetList<Fractional3D[]>(comparer)
            {
                GetAllWyckoffSequences(refSequence).Select(sequence => ShiftFirstToOrigin(sequence, 1.0e-10).ToArray())
            };
        }

        /// <summary>
        /// Shifts a seuqnce of fractional vectors in a manner that the first vector in the sequuence is in the (0,0,0) origin cell
        /// </summary>
        /// <param name="unshifted"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public IEnumerable<Fractional3D> ShiftFirstToOrigin(IEnumerable<Fractional3D> unshifted, double tolerance)
        {
            var start = unshifted.ElementAt(0);
            var shift = start.TrimToUnitCell(tolerance) - start;
            return unshifted.Select(value => value + shift);
        }

        /// <summary>
        /// Shifts all positions so that the resulting sequence first position is on the new position
        /// </summary>
        /// <param name="unshifted"></param>
        /// <param name="newFirst"></param>
        /// <returns></returns>
        public IEnumerable<Fractional3D> ShiftFirstToPosition(IEnumerable<Fractional3D> unshifted, in Fractional3D newFirst)
        {
            var shift = newFirst - unshifted.ElementAt(0);
            return unshifted.Select(value => value + shift);
        }

        /// <summary>
        /// Creates the wyckoff position dictionray for the provided source vector
        /// </summary>
        /// <param name="sourceVector"></param>
        /// <returns></returns>
        public IWyckoffOperationDictionary GetOperationDictionary(in Fractional3D sourceVector)
        {
            var operationComparer = Comparer<ISymmetryOperation>.Create((a, b) => a.Literal.CompareTo(b.Literal));
            var dictionary = new SortedDictionary<Fractional3D, SetList<ISymmetryOperation>>(VectorComparer);

            foreach (var operation in LoadedGroupEntity.BaseSymmetryOperations)
            {
                var vector = operation.ApplyWithTrim(sourceVector);
                if (!dictionary.TryGetValue(vector, out var operationList))
                {
                    dictionary[vector] = new SetList<ISymmetryOperation>(operationComparer);
                }
                dictionary[vector].Add(operation);
            }
            return new WyckoffOperationDictionary(sourceVector, LoadedGroupEntity, dictionary);
        }

        /// <summary>
        /// Get a list interface of all symmetry operations that do not change the input vector
        /// </summary>
        /// <param name="sourceVector"></param>
        /// <param name="shiftCorrection"></param>
        /// <returns></returns>
        public IList<ISymmetryOperation> GetMultiplicityOperations(in Fractional3D sourceVector, bool shiftCorrection)
        {
            var result = new List<ISymmetryOperation>(LoadedGroupEntity.BaseSymmetryOperations.Count);
            foreach (var operation in LoadedGroupEntity.BaseSymmetryOperations)
            {
                var untrimmedVector = operation.ApplyUntrimmed(sourceVector);
                if (Comparer.Compare(untrimmedVector.TrimToUnitCell(operation.TrimTolerance), sourceVector) == 0)
                {
                    var shift = (shiftCorrection) ? sourceVector - untrimmedVector : Fractional3D.NullVector;
                    var newOperation = GetTranslationShiftedOperation(operation, shift);
                    result.Add(newOperation);
                }
            }
            result.TrimExcess();
            return result;
        }

        /// <summary>
        /// Get the point operation group for the provided origin point and point sequence based upon the currently loaded space group
        /// </summary>
        /// <param name="originPoint"></param>
        /// <param name="pointSequence"></param>
        /// <returns></returns>
        public IPointOperationGroup GetPointOperationGroup(in Fractional3D originPoint, IEnumerable<Fractional3D> pointSequence)
        {
            var operationGroup = new PointOperationGroup()
            {
                SpaceGroupEntry = LoadedGroup.GetGroupEntry(),
                OriginPoint = new DataVector3D(originPoint),
                PointSequence = pointSequence.Select(value => new DataVector3D(value)).ToList(),
                SelfProjectionOperations = new List<SymmetryOperation>()
            };

            var multiplicityOperations = GetMultiplicityOperations(originPoint, true);
            var resultSequences = multiplicityOperations.Select(operation => operation.ApplyUntrimmed(pointSequence).ToList()).ToList();

            operationGroup.PointOperations = multiplicityOperations.Cast<SymmetryOperation>().ToList();

            var equalityComparer = MakeVectorSequenceProjectionComparer();
            foreach (var index in resultSequences.IndexOfMany(value => equalityComparer.Equals(value, resultSequences.First())))
            {
                operationGroup.SelfProjectionOperations.Add((SymmetryOperation)multiplicityOperations[index]);
            }

            foreach (var index in resultSequences.RemoveDuplicates(MakeVectorSequenceEquivalenceComparer()))
            {
                multiplicityOperations.RemoveAt(index);
            }

            operationGroup.UniqueSequenceOperations = multiplicityOperations.Cast<SymmetryOperation>().ToList();
            operationGroup.PositionEquivalencyIndexing = MakeProjectionIndexing(pointSequence, operationGroup.SelfProjectionOperations);
            return operationGroup;
        }

        /// <summary>
        /// Translates an environment seqeunce onto each possible wyckoff 1 position and retunrs the results as a sorted dictionay for lookup
        /// </summary>
        /// <param name="refSequence"></param>
        /// <returns></returns>
        public SortedDictionary<Fractional3D, List<Fractional3D>> CreateEnvironmentDictionary(IEnumerable<Fractional3D> refSequence)
        {
            var result = new SortedDictionary<Fractional3D, List<Fractional3D>>(VectorComparer);
            var wyckoffDictionary = GetOperationDictionary(refSequence.ElementAt(0));

            foreach (var entry in wyckoffDictionary)
            {
                var operation = entry.Value.ElementAt(0);
                var sequence = ShiftFirstToPosition(refSequence.Select(value => operation.ApplyUntrimmed(value)), entry.Key);
                result.Add(entry.Key, sequence.ToList());
            }

            return result;
        }

        /// <summary>
        /// Add a translation shift to a symmetry operation and returns a new symmetry operation that contains the new operations array and literal description
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="shift"></param>
        /// <returns></returns>
        public ISymmetryOperation GetTranslationShiftedOperation(ISymmetryOperation operation, in Fractional3D shift)
        {
            var operationsArray = operation.GetOperationsArray();
            operationsArray[3] += shift.A;
            operationsArray[7] += shift.B;
            operationsArray[11] += shift.C;

            return new SymmetryOperation()
            {
                Literal = $"({operation.Literal}) + ({shift.A} ,{shift.B}, {shift.C})",
                TrimTolerance = operation.TrimTolerance,
                Operations = operationsArray
            };
        }

        /// <summary>
        /// Determines which positions can be projected onto each other within the passed set of vectors using the provided set of operations and assigns
        /// indices based upon the original sequence
        /// </summary>
        /// <param name="vectors"></param>
        /// <param name="operations"></param>
        /// <returns> Operations have to be self projection operations for this function to yield meaningfull result </returns>
        protected List<int> MakeProjectionIndexing(IEnumerable<Fractional3D> vectors, IEnumerable<ISymmetryOperation> symmetryOperations)
        {
            var options = symmetryOperations.Select(operation => operation.ApplyUntrimmed(vectors).ToList()).ToList();
            var indexing = new List<int>(vectors.Count()).Populate(int.MaxValue, vectors.Count());

            for (int i = 0; i < options.Count; i++)
            {
                for (int j = 0; j < options[i].Count; j++)
                {
                    int indexInFirst = options[0].FindIndex(0, value => VectorComparer.Equals(value, options[i][j]));
                    indexing[j] = (indexInFirst < indexing[j]) ? indexInFirst : indexing[j];
                }
            }

            return indexing;
        }

        /// <summary>
        /// Creates a geometry seqeunce comparer that returns true if two sequences are equivalent or reversing one sequence makes them equivalent
        /// </summary>
        /// <returns></returns>
        protected IEqualityComparer<IList<Fractional3D>> MakeVectorSequenceEquivalenceComparer()
        {
            int AreEquivalent(IList<Fractional3D> lhs, IList<Fractional3D> rhs)
            {
                if (lhs.GetSequenceEqualityDirectionTo(rhs, VectorComparer) != 0)
                {
                    return 0;
                }
                return -1;
            }
            return new CompareAdapter<IList<Fractional3D>>(AreEquivalent);
        }

        /// <summary>
        /// Creates a geoemtry sequence comparer that returns true if two sequences contain the same set of vectors in any order
        /// </summary>
        /// <returns></returns>
        protected IEqualityComparer<IList<Fractional3D>> MakeVectorSequenceProjectionComparer()
        {
            int AreEquivalent(IList<Fractional3D> lhs, IList<Fractional3D> rhs)
            {
                if (lhs.Count != rhs.Count)
                {
                    return (lhs.Count < rhs.Count) ? -1 : 1;
                }
                if (lhs.Select(value => rhs.Contains(value, VectorComparer)).All(value => value == true))
                {
                    return 0;
                }
                return -1;
            }
            return new CompareAdapter<IList<Fractional3D>>(AreEquivalent);
        }
    }
}
