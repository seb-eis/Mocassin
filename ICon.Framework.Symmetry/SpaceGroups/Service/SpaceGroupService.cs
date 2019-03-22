using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Mocassin.Framework.Collections;
using Mocassin.Framework.Exceptions;
using Mocassin.Framework.Extensions;
using Mocassin.Framework.SQLiteCore;
using Mocassin.Mathematics.Comparers;
using Mocassin.Mathematics.Extensions;
using Mocassin.Mathematics.ValueTypes;
using Mocassin.Symmetry.CrystalSystems;

namespace Mocassin.Symmetry.SpaceGroups
{
    /// <inheritdoc />
    public class SpaceGroupService : ISpaceGroupService
    {
        /// <summary>
        ///     The space group context provider
        /// </summary>
        public ISqLiteContextProvider<SpaceGroupContext> ContextProvider { get; set; }

        /// <summary>
        ///     The currently loaded space group object
        /// </summary>
        public SpaceGroupEntity LoadedGroupEntity { get; set; }

        /// <inheritdoc />
        public ISpaceGroup LoadedGroup => LoadedGroupEntity;

        /// <summary>
        ///     The equality comparator which contains the almost equal information for the double vectors
        /// </summary>
        public VectorComparer3D<Fractional3D> VectorComparer { get; set; }

        /// <inheritdoc />
        public IComparer<Fractional3D> Comparer => VectorComparer;

        /// <summary>
        ///     Creates new space group service using the default space group context, database filepath and double comparer
        /// </summary>
        /// <param name="dbFilepath"></param>
        /// <param name="comparer"></param>
        public SpaceGroupService(string dbFilepath, IComparer<double> comparer)
        {
            ContextProvider = new SpaceGroupContextProvider(dbFilepath);
            VectorComparer = new VectorComparer3D<Fractional3D>(comparer);
        }

        /// <summary>
        ///     Creates new space group service using the default space group context, database path and tolerance value for
        ///     comparisons
        /// </summary>
        /// <param name="dbFilepath"></param>
        /// <param name="tolerance"></param>
        public SpaceGroupService(string dbFilepath, double tolerance)
            : this(dbFilepath, NumericComparer.CreateRanged(tolerance))
        {
            TryLoadGroup(group => group.Index == 1);
        }

        /// <summary>
        ///     Creates new space group service from custom SQLiteCoreContextProvider
        /// </summary>
        /// <param name="contextProvider"></param>
        /// <param name="comparer"></param>
        public SpaceGroupService(ISqLiteContextProvider<SpaceGroupContext> contextProvider, IComparer<double> comparer)
        {
            ContextProvider = contextProvider ?? throw new ArgumentNullException(nameof(contextProvider));
            VectorComparer = new VectorComparer3D<Fractional3D>(comparer);
        }

        /// <inheritdoc />
        public bool TryLoadGroup(Predicate<ISpaceGroup> searchPredicate)
        {
            SpaceGroupEntity newGroup;
            using (var context = ContextProvider.CreateContext())
            {
                newGroup = context.SpaceGroups
                    .Where(a => searchPredicate(a))
                    .Include(g => g.BaseSymmetryOperations)
                    .SingleOrDefault();

                if (LoadedGroupEntity == null) 
                    LoadedGroupEntity = newGroup;

                if (newGroup != null && !newGroup.GetGroupEntry().Equals(LoadedGroupEntity.GetGroupEntry()))
                    LoadedGroupEntity = !newGroup.GetGroupEntry().Equals(LoadedGroupEntity.GetGroupEntry()) 
                        ? newGroup 
                        : LoadedGroupEntity;
            }

            return newGroup != null;
        }

        /// <inheritdoc />
        public bool TryLoadGroup(SpaceGroupEntry groupEntry)
        {
            if (groupEntry == null)
                throw new ArgumentNullException(nameof(groupEntry));

            if (LoadedGroupEntity != null && groupEntry.Equals(LoadedGroupEntity.GetGroupEntry()))
                return true;

            return TryLoadGroup(group => group.Index == groupEntry.Index && group.Specifier == groupEntry.Specifier);
        }

        /// <inheritdoc />
        public CrystalSystem CreateCrystalSystem(ICrystalSystemSource source)
        {
            if (LoadedGroupEntity == null)
                throw new InvalidObjectStateException("No space group loaded, cannot create crystal system");

            return source.Create(LoadedGroupEntity);
        }

        /// <inheritdoc />
        public SetList<ISpaceGroup> GetFullGroupList()
        {
            var setList = new SetList<ISpaceGroup>(Comparer<ISpaceGroup>.Default, 274);
            using (var context = ContextProvider.CreateContext())
            {
                setList.AddMany(context.SpaceGroups.Include(x => x.BaseSymmetryOperations));
            }

            return setList;
        }

        /// <inheritdoc />
        public SetList<Fractional3D> GetAllWyckoffPositions(Fractional3D refVector)
        {
            return CreatePositionSetList(refVector);
        }

        /// <inheritdoc />
        public List<SetList<Fractional3D>> GetAllWyckoffPositions(IEnumerable<Fractional3D> refVectors)
        {
            if (refVectors == null)
                throw new ArgumentNullException(nameof(refVectors));

            return refVectors.Select(GetAllWyckoffPositions).ToList();
        }

        /// <inheritdoc />
        public SetList<TSource> GetAllWyckoffPositions<TSource>(TSource refVector)
            where TSource : struct, IFractional3D<TSource>
        {
            return CreatePositionSetList(refVector);
        }

        /// <inheritdoc />
        public List<SetList<TSource>> GetAllWyckoffPositions<TSource>(IEnumerable<TSource> refVectors)
            where TSource : struct, IFractional3D<TSource>
        {
            if (refVectors == null)
                throw new ArgumentNullException(nameof(refVectors));

            return refVectors.Select(GetAllWyckoffPositions).ToList();
        }

        /// <summary>
        ///     Creates the position set list of all equivalent vectors to the passed source
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        protected SetList<Fractional3D> CreatePositionSetList(Fractional3D vector)
        {
            var results = new SetList<Fractional3D>(VectorComparer, LoadedGroupEntity.BaseSymmetryOperations.Count);
            foreach (var operation in LoadedGroupEntity.BaseSymmetryOperations)
                results.Add(operation.ApplyWithTrim(vector));

            results.List.TrimExcess();
            return results;
        }

        /// <summary>
        ///     Creates a basic fractional position set list from double coordinate values
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        protected SetList<Fractional3D> CreatePositionSetList(double a, double b, double c)
        {
            var results = new SetList<Fractional3D>(VectorComparer, LoadedGroupEntity.BaseSymmetryOperations.Count);
            foreach (var operation in LoadedGroupEntity.BaseSymmetryOperations)
                results.Add(operation.ApplyWithTrim(a, b, c));

            results.List.TrimExcess();
            return results;
        }

        /// <summary>
        ///     Creates a source type fractional position set list where all results carry the original information of the source
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="vector"></param>
        /// <returns></returns>
        protected SetList<TSource> CreatePositionSetList<TSource>(TSource vector) where TSource : struct, IFractional3D<TSource>
        {
            var results = new SetList<TSource>(VectorComparer.ToCompatibleComparer<TSource>(),
                LoadedGroupEntity.BaseSymmetryOperations.Count);

            foreach (var operation in LoadedGroupEntity.BaseSymmetryOperations)
                results.Add(operation.ApplyWithTrim(vector));

            results.List.TrimExcess();
            return results;
        }

        /// <summary>
        ///     Get all wyckoff sequences for the provided reference sequence of fractional vectors
        /// </summary>
        /// <param name="refSequence"></param>
        /// <returns></returns>
        public SetList<Fractional3D[]> GetWyckoffSequences(params Fractional3D[] refSequence)
        {
            Fractional3D[] MoveStartToUnitCell(IReadOnlyList<Fractional3D> vectors)
            {
                var coordinateA = vectors[0].A.PeriodicTrim(0.0, 1.0, 1.0e-10) - vectors[0].A;
                var coordinateB = vectors[0].B.PeriodicTrim(0.0, 1.0, 1.0e-10) - vectors[0].B;
                var coordinateC = vectors[0].C.PeriodicTrim(0.0, 1.0, 1.0e-10) - vectors[0].C;
                var shift = new Fractional3D(coordinateA, coordinateB, coordinateC);
                return vectors.Select(a => a + shift).ToArray();
            }

            var comparer = Comparer<Fractional3D[]>.Create((a, b) => a.LexicographicCompare(b, VectorComparer));
            var result = new SetList<Fractional3D[]>(comparer);

            foreach (var operation in LoadedGroupEntity.BaseSymmetryOperations)
                result.Add(MoveStartToUnitCell(refSequence.Select(a => operation.ApplyUntrimmed(a.A, a.B, a.C)).ToArray()));

            return result;
        }

        /// <inheritdoc />
        public IList<Fractional3D[]> GetAllWyckoffSequences(IEnumerable<Fractional3D> refSequence)
        {
            return LoadedGroupEntity.BaseSymmetryOperations
                .Select(operation => refSequence.Select(vector => operation.ApplyUntrimmed(vector)).ToArray()).ToList();
        }

        /// <inheritdoc />
        public SetList<Fractional3D[]> GetAllWyckoffOriginSequences(IEnumerable<Fractional3D> refSequence)
        {
            var comparer = Comparer<Fractional3D[]>.Create((a, b) => a.LexicographicCompare(b, VectorComparer));
            return new SetList<Fractional3D[]>(comparer)
            {
                GetAllWyckoffSequences(refSequence).Select(sequence => ShiftFirstToOrigin(sequence, 1.0e-10).ToArray())
            };
        }

        /// <inheritdoc />
        public IEnumerable<Fractional3D> ShiftFirstToOrigin(IEnumerable<Fractional3D> source, double tolerance)
        {
            if (!(source is IList<Fractional3D> sourceList))
                sourceList = source.ToList();

            var start = sourceList.ElementAt(0);
            var shift = start.TrimToUnitCell(tolerance) - start;
            return sourceList.Select(value => value + shift);
        }

        /// <inheritdoc />
        public ISymmetryOperation CreateOperationToTarget(in Fractional3D source, in Fractional3D target)
        {
            foreach (var operation in LoadedGroupEntity.BaseSymmetryOperations)
            {
                var vector = operation.ApplyWithTrim(source, out var trimVector);
                if (Comparer.Compare(vector, target) == 0) 
                    return GetTranslationShiftedOperation(operation, trimVector);
            }

            return null;
        }

        /// <summary>
        ///     Shifts all positions so that the resulting sequence first position is on the new position
        /// </summary>
        /// <param name="source"></param>
        /// <param name="newFirst"></param>
        /// <returns></returns>
        public IEnumerable<Fractional3D> ShiftFirstToPosition(IEnumerable<Fractional3D> source, in Fractional3D newFirst)
        {
            if (!(source is IList<Fractional3D> sourceList))
                sourceList = source.ToList();

            var shift = newFirst - sourceList.ElementAt(0);
            return sourceList.Select(value => value + shift);
        }

        /// <inheritdoc />
        public IWyckoffOperationDictionary GetOperationDictionary(in Fractional3D sourceVector)
        {
            var operationComparer = Comparer<ISymmetryOperation>.Create((a, b) => string.Compare(a.Literal, b.Literal, StringComparison.Ordinal));
            var dictionary = new SortedDictionary<Fractional3D, SetList<ISymmetryOperation>>(VectorComparer);

            foreach (var operation in LoadedGroupEntity.BaseSymmetryOperations)
            {
                var vector = operation.ApplyWithTrim(sourceVector);
                if (!dictionary.ContainsKey(vector))
                    dictionary[vector] = new SetList<ISymmetryOperation>(operationComparer);

                dictionary[vector].Add(operation);
            }

            return new WyckoffOperationDictionary(sourceVector, LoadedGroupEntity, dictionary);
        }

        /// <inheritdoc />
        public IList<ISymmetryOperation> GetMultiplicityOperations(in Fractional3D sourceVector, bool shiftCorrection)
        {
            var result = new List<ISymmetryOperation>(LoadedGroupEntity.BaseSymmetryOperations.Count);
            foreach (var operation in LoadedGroupEntity.BaseSymmetryOperations)
            {
                var untrimmedVector = operation.ApplyUntrimmed(sourceVector);
                if (Comparer.Compare(untrimmedVector.TrimToUnitCell(operation.TrimTolerance), sourceVector) != 0)
                    continue;

                var shift = shiftCorrection ? sourceVector - untrimmedVector : Fractional3D.NullVector;
                var newOperation = GetTranslationShiftedOperation(operation, shift);
                result.Add(newOperation);
            }

            result.TrimExcess();
            return result;
        }

        /// <inheritdoc />
        public IPointOperationGroup GetPointOperationGroup(in Fractional3D originPoint, IEnumerable<Fractional3D> pointSequence)
        {
            if (!(pointSequence is IList<Fractional3D> pointList))
                pointList = pointSequence.ToList();

            var operationGroup = new PointOperationGroup
            {
                SpaceGroupEntry = LoadedGroup.GetGroupEntry(),
                OriginPoint = new DataVector3D(originPoint),
                PointSequence = pointList.Select(value => new DataVector3D(value)).ToList(),
                SelfProjectionOperations = new List<SymmetryOperation>()
            };

            var multiplicityOperations = GetMultiplicityOperations(originPoint, true);
            var resultSequences = multiplicityOperations
                .Select(operation => operation.ApplyUntrimmed(pointList).ToList()).ToList();

            operationGroup.PointOperations = multiplicityOperations.Cast<SymmetryOperation>().ToList();

            var equalityComparer = MakeVectorSequenceProjectionComparer();

            foreach (var index in resultSequences.IndexOfMany(value => equalityComparer.Equals(value, resultSequences.First())))
                operationGroup.SelfProjectionOperations.Add((SymmetryOperation) multiplicityOperations[index]);

            foreach (var index in resultSequences.RemoveDuplicatesAndGetRemovedIndices(MakeVectorSequenceEquivalenceComparer()))
                multiplicityOperations.RemoveAt(index);

            operationGroup.UniqueSequenceOperations = multiplicityOperations.Cast<SymmetryOperation>().ToList();
            operationGroup.UniqueSelfProjectionOrders = MakeProjectionMatrix(pointList, operationGroup.SelfProjectionOperations);
            return operationGroup;
        }

        /// <inheritdoc />
        public SortedDictionary<Fractional3D, List<Fractional3D>> CreateEnvironmentDictionary(IEnumerable<Fractional3D> refSequence)
        {
            if (!(refSequence is IList<Fractional3D> refList))
                refList = refSequence.ToList();

            var result = new SortedDictionary<Fractional3D, List<Fractional3D>>(VectorComparer);
            var wyckoffDictionary = GetOperationDictionary(refList[0]);

            foreach (var entry in wyckoffDictionary)
            {
                var operation = entry.Value.ElementAt(0);
                var sequence = ShiftFirstToPosition(refList.Select(value => operation.ApplyUntrimmed(value)), entry.Key);
                result.Add(entry.Key, sequence.ToList());
            }

            return result;
        }

        /// <summary>
        ///     Add a translation shift to a symmetry operation and returns a new symmetry operation that contains the new
        ///     operations array and literal description
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

            return new SymmetryOperation
            {
                Literal = $"({operation.Literal}) + ({shift.A} ,{shift.B}, {shift.C})",
                TrimTolerance = operation.TrimTolerance,
                Operations = operationsArray
            };
        }

        /// <summary>
        ///     Determines the unique possible projection orders of the positions within the passed vector sequence and operation
        ///     set
        /// </summary>
        /// <param name="vectors"></param>
        /// <param name="symmetryOperations"></param>
        /// <returns> Operations have to be self projection operations for this function to yield meaningful results </returns>
        protected List<List<int>> MakeProjectionMatrix(IEnumerable<Fractional3D> vectors,
            IEnumerable<ISymmetryOperation> symmetryOperations)
        {
            var options = symmetryOperations.Select(operation => operation.ApplyUntrimmed(vectors).ToList()).ToList();
            var uniqueIndexSets = new HashSet<List<int>>(new EqualityCompareAdapter<List<int>>((a, b) => a.SequenceEqual(b), a => a.Sum()));

            foreach (var option in options)
            {
                var currentIndexing = new List<int>(8);
                foreach (var vector in option)
                    currentIndexing.Add(options[0].FindIndex(0, value => VectorComparer.Equals(value, vector)));

                uniqueIndexSets.Add(currentIndexing);
            }

            return uniqueIndexSets.ToList();
        }

        /// <summary>
        ///     Creates a geometry sequence comparer that returns true if two sequences are equivalent or reversing one sequence
        ///     makes them equivalent
        /// </summary>
        /// <returns></returns>
        protected IEqualityComparer<IList<Fractional3D>> MakeVectorSequenceEquivalenceComparer()
        {
            bool AreEquivalent(IList<Fractional3D> lhs, IList<Fractional3D> rhs)
            {
                return lhs.GetSequenceEqualityDirectionTo(rhs, VectorComparer) != 0;
            }

            return new EqualityCompareAdapter<IList<Fractional3D>>(AreEquivalent);
        }

        /// <summary>
        ///     Creates a geometry sequence comparer that returns true if two sequences contain the same set of vectors in any
        ///     order
        /// </summary>
        /// <returns></returns>
        protected IEqualityComparer<IList<Fractional3D>> MakeVectorSequenceProjectionComparer()
        {
            bool AreEquivalent(IList<Fractional3D> lhs, IList<Fractional3D> rhs)
            {
                return lhs.Count == rhs.Count && lhs.Select(value => rhs.Contains(value, VectorComparer)).All(value => value);
            }

            return new EqualityCompareAdapter<IList<Fractional3D>>(AreEquivalent);
        }

        /// <inheritdoc />
        public IComparer<T1> GetSpecialVectorComparer<T1>() where T1 : IVector3D
        {
            return Comparer<T1>.Create((a, b) => ((IComparer<IVector3D>) VectorComparer).Compare(a, b));
        }
    }
}