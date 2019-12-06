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
        public ISqLiteContextProvider<SpaceGroupContext> ContextProvider { get; }

        /// <inheritdoc />
        public bool HasDbConnection => ContextProvider != null;

        /// <inheritdoc />
        public ISpaceGroup LoadedGroup { get; protected set; }

        /// <summary>
        ///     The equality comparator which contains the almost equal information for the double vectors
        /// </summary>
        public VectorComparer3D<Fractional3D> VectorComparer { get; }

        /// <inheritdoc />
        public IComparer<Fractional3D> Comparer => VectorComparer;

        /// <summary>
        ///     Get the <see cref="IComparer{T}"/> for double values used by the vector comparer
        /// </summary>
        public IComparer<double> DoubleComparer => VectorComparer.ValueComparer;

        /// <summary>
        ///     Creates new <see cref="SpaceGroupService" /> that uses the provided comparer and database file path
        /// </summary>
        /// <param name="dbFilepath"></param>
        /// <param name="comparer"></param>
        public SpaceGroupService(string dbFilepath, IComparer<double> comparer)
            : this(comparer)
        {
            if (dbFilepath == null) throw new ArgumentNullException(nameof(dbFilepath));
            ContextProvider = new SpaceGroupContextProvider(dbFilepath);
        }

        /// <summary>
        ///     Creates a new <see cref="SpaceGroupService" /> with the passed comparer that does not have a database connection
        /// </summary>
        /// <param name="comparer"></param>
        public SpaceGroupService(IComparer<double> comparer)
        {
            if (comparer == null) throw new ArgumentNullException(nameof(comparer));
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
            if (!HasDbConnection) return false;

            SpaceGroupEntity newGroup;
            using (var context = ContextProvider.CreateContext())
            {
                newGroup = context.SpaceGroups
                    .Where(a => searchPredicate(a))
                    .Include(g => g.BaseSymmetryOperations)
                    .SingleOrDefault();

                if (LoadedGroup == null)
                    LoadedGroup = newGroup;

                if (newGroup != null && !newGroup.GetGroupEntry().Equals(LoadedGroup.GetGroupEntry()))
                {
                    LoadedGroup = !newGroup.GetGroupEntry().Equals(LoadedGroup.GetGroupEntry())
                        ? newGroup
                        : LoadedGroup;
                }
            }

            return newGroup != null;
        }

        /// <inheritdoc />
        public bool TryLoadGroup(SpaceGroupEntry groupEntry)
        {
            if (!HasDbConnection) return false;

            if (groupEntry == null) throw new ArgumentNullException(nameof(groupEntry));

            if (LoadedGroup != null && groupEntry.Equals(LoadedGroup.GetGroupEntry())) return true;

            return TryLoadGroup(group => group.Index == groupEntry.Index && group.Specifier == groupEntry.Specifier);
        }

        /// <inheritdoc />
        public void LoadGroup(ISpaceGroup spaceGroup)
        {
            if (spaceGroup == null) throw new ArgumentNullException(nameof(spaceGroup));
            if (LoadedGroup != spaceGroup) LoadedGroup = spaceGroup;
        }

        /// <inheritdoc />
        public CrystalSystem CreateCrystalSystem(ICrystalSystemSource source)
        {
            if (LoadedGroup == null)
                throw new InvalidObjectStateException("No space group loaded, cannot create crystal system");

            return source.Create(LoadedGroup);
        }

        /// <inheritdoc />
        public SetList<ISpaceGroup> GetFullGroupList()
        {
            var setList = new SetList<ISpaceGroup>(Comparer<ISpaceGroup>.Default, 274);
            using (var context = ContextProvider.CreateContext())
            {
                setList.AddRange(context.SpaceGroups.Include(x => x.BaseSymmetryOperations));
            }

            return setList;
        }

        /// <inheritdoc />
        public SetList<Fractional3D> GetUnitCellP1PositionExtension(in Fractional3D refVector)
        {
            return CreatePositionSetList(refVector);
        }

        /// <inheritdoc />
        public List<SetList<Fractional3D>> GetUnitCellP1PositionExtensions(IEnumerable<Fractional3D> refVectors)
        {
            if (refVectors == null)
                throw new ArgumentNullException(nameof(refVectors));

            return refVectors.Select(GetUnitCellP1PositionExtension).ToList();
        }

        /// <inheritdoc />
        public SetList<Fractional3D> GetUnitCellP1PositionExtension<TSource>(TSource refVector) where TSource : IFractional3D
        {
            return CreatePositionSetList(new Fractional3D(refVector.Coordinates));
        }

        /// <inheritdoc />
        public List<SetList<Fractional3D>> GetUnitCellP1PositionExtensions<TSource>(IEnumerable<TSource> refVectors)
            where TSource : IFractional3D
        {
            if (refVectors == null)
                throw new ArgumentNullException(nameof(refVectors));

            return refVectors.Select(GetUnitCellP1PositionExtension).ToList();
        }

        /// <summary>
        ///     Creates the position set list of all equivalent vectors to the passed source
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        protected SetList<Fractional3D> CreatePositionSetList(Fractional3D vector)
        {
            var results = new SetList<Fractional3D>(VectorComparer, LoadedGroup.Operations.Count);
            foreach (var operation in LoadedGroup.Operations)
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
            var results = new SetList<Fractional3D>(VectorComparer, LoadedGroup.Operations.Count);
            foreach (var operation in LoadedGroup.Operations)
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
        protected SetList<Fractional3D> CreatePositionSetList<TSource>(TSource vector) where TSource : IFractional3D
        {
            var results = new SetList<Fractional3D>(VectorComparer, LoadedGroup.Operations.Count);

            foreach (var operation in LoadedGroup.Operations)
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
            Fractional3D[] MoveStartToUnitCell(IList<Fractional3D> vectors)
            {
                var coordinateA = vectors[0].A.PeriodicTrim(0.0, 1.0, DoubleComparer) - vectors[0].A;
                var coordinateB = vectors[0].B.PeriodicTrim(0.0, 1.0, DoubleComparer) - vectors[0].B;
                var coordinateC = vectors[0].C.PeriodicTrim(0.0, 1.0, DoubleComparer) - vectors[0].C;
                var shift = new Fractional3D(coordinateA, coordinateB, coordinateC);
                return vectors.Select(a => a + shift).ToArray(vectors.Count);
            }

            var comparer = Comparer<Fractional3D[]>.Create((a, b) => a.LexicographicCompare(b, VectorComparer));
            var result = new SetList<Fractional3D[]>(comparer);

            foreach (var operation in LoadedGroup.Operations)
                result.Add(MoveStartToUnitCell(refSequence.Select(a => operation.ApplyUntrimmed(a.A, a.B, a.C)).ToList(refSequence.Length)));

            return result;
        }

        /// <inheritdoc />
        public IList<Fractional3D[]> GetFullP1PathExtension(IEnumerable<Fractional3D> refSequence)
        {
            var refCollection = refSequence.AsCollection();
            return LoadedGroup.Operations
                .Select(operation => refCollection.Select(vector => operation.ApplyUntrimmed(vector)).ToArray(refCollection.Count))
                .ToList(LoadedGroup.Operations.Count);
        }

        /// <inheritdoc />
        public SetList<Fractional3D[]> GetUnitCellP1PathExtension(IEnumerable<Fractional3D> refSequence)
        {
            var refCollection = refSequence.AsCollection();

            var comparer = Comparer<Fractional3D[]>.Create((a, b) => a.LexicographicCompare(b, VectorComparer));
            var list = new SetList<Fractional3D[]>(comparer)
            {
                GetFullP1PathExtension(refCollection).Select(sequence => ShiftFirstToOriginCell(sequence, DoubleComparer).ToArray(sequence.Length))
            };

            return list;
        }

        /// <inheritdoc />
        public IList<ISymmetryOperation> GetMinimalUnitCellP1PathExtensionOperations(IEnumerable<Fractional3D> refSequence,
            bool filterInverses = false)
        {
            var refVectors = refSequence.AsList();

            var operations = new List<ISymmetryOperation>(LoadedGroup.Operations.Count);
            operations.AddRange(LoadedGroup.Operations.Select(x =>
                GetOriginCellShiftedOperations(x.ApplyUntrimmed(refVectors[0]), x, DoubleComparer)));

            var sequences = new SetList<IList<Fractional3D>>(
                Comparer<IList<Fractional3D>>.Create((a, b) => a.LexicographicCompare(b, VectorComparer)),
                operations.Count);
            var filteredOperations = new List<ISymmetryOperation>(LoadedGroup.Operations.Count / 2);

            var transformedVectors = new List<Fractional3D>(refVectors.Count);
            foreach (var operation in operations)
            {
                transformedVectors.Clear();
                transformedVectors.AddRange(operation.ApplyUntrimmed(refVectors));
                if (sequences.Contains(transformedVectors)) continue;
                if (filterInverses)
                {
                    transformedVectors.Reverse();
                    if (sequences.Contains(transformedVectors)) continue;
                }

                var transformCopy = transformedVectors.ToList(transformedVectors.Count);
                sequences.Add(transformCopy);
                filteredOperations.Add(operation);
            }

            return filteredOperations;
        }

        /// <inheritdoc />
        public IEnumerable<Fractional3D> ShiftFirstToOriginCell(IEnumerable<Fractional3D> source, double tolerance)
        {
            var sourceList = source.AsList();

            var start = sourceList[0];
            var shift = start.TrimToUnitCell(tolerance) - start;
            return sourceList.Select(value => value + shift);
        }

        /// <inheritdoc />
        public IEnumerable<Fractional3D> ShiftFirstToOriginCell(IEnumerable<Fractional3D> source, IComparer<double> comparer)
        {
            var sourceList = source.AsList();

            var start = sourceList[0];
            var shift = start.TrimToUnitCell(comparer) - start;
            return sourceList.Select(value => value + shift);
        }

        /// <inheritdoc />
        public ISymmetryOperation GetOriginCellShiftedOperations(in Fractional3D start, ISymmetryOperation operation, double tolerance)
        {
            var shift = start.TrimToUnitCell(tolerance) - start;
            return GetTranslationShiftedOperation(operation, shift);
        }

        /// <inheritdoc />
        public ISymmetryOperation GetOriginCellShiftedOperations(in Fractional3D start, ISymmetryOperation operation, IComparer<double> comparer)
        {
            var shift = start.TrimToUnitCell(comparer) - start;
            return GetTranslationShiftedOperation(operation, shift);
        }

        /// <inheritdoc />
        public ISymmetryOperation GetOperationToTarget(in Fractional3D source, in Fractional3D target)
        {
            foreach (var operation in LoadedGroup.Operations)
            {
                var vector = operation.ApplyWithTrim(source, out var trimVector);
                if (Comparer.Compare(vector, target) == 0)
                    return GetTranslationShiftedOperation(operation, trimVector);
            }

            return null;
        }

        /// <inheritdoc />
        public IList<Fractional3D> GetPositionsInCuboid(in Fractional3D source, in Fractional3D start, in Fractional3D end)
        {
            var equalityComparer = VectorComparer.ValueComparer.ToEqualityComparer();
            var (aMin, bMin, cMin) = (MocassinMath.FloorToInt(start.A, equalityComparer),
                MocassinMath.FloorToInt(start.B, equalityComparer),
                MocassinMath.FloorToInt(start.C, equalityComparer));
            var (aMax, bMax, cMax) = (MocassinMath.FloorToInt(end.A, equalityComparer), MocassinMath.FloorToInt(end.B, equalityComparer),
                MocassinMath.FloorToInt(end.C, equalityComparer));

            var basePositions = GetUnitCellP1PositionExtension(source);
            var capacity = Math.Abs(aMax - aMin) * Math.Abs(bMax - bMin) * Math.Abs(cMax - cMin);
            var result = new List<Fractional3D>(capacity * capacity);
            for (var a = aMin; a <= aMax; a++)
            {
                for (var b = bMin; b <= bMax; b++)
                {
                    for (var c = cMin; c <= cMax; c++)
                    {
                        foreach (var entry in basePositions)
                        {
                            var vector = entry + new Fractional3D(a, b, c);
                            if (VectorComparer.ValueComparer.Compare(vector.A, start.A) < 0) continue;
                            if (VectorComparer.ValueComparer.Compare(vector.B, start.B) < 0) continue;
                            if (VectorComparer.ValueComparer.Compare(vector.C, start.C) < 0) continue;
                            if (VectorComparer.ValueComparer.Compare(vector.A, end.A) > 0) continue;
                            if (VectorComparer.ValueComparer.Compare(vector.B, end.B) > 0) continue;
                            if (VectorComparer.ValueComparer.Compare(vector.C, end.C) > 0) continue;
                            result.Add(vector);
                        }
                    }
                }
            }

            return result;
        }

        /// <inheritdoc />
        public bool CheckInteractionGeometryIsChiralPair(in Fractional3D left0, in Fractional3D right0, in Fractional3D left1, in Fractional3D right1)
        {
            var leftSet = GetWyckoffSequences(left0, right0);
            var rightSet = GetWyckoffSequences(right0, left0);
            var testArray = new[] {left1, right1};
            return !leftSet.Contains(testArray) && rightSet.Contains(testArray);
        }

        /// <inheritdoc />
        public bool CheckInteractionGeometryIsChiral(in Fractional3D left, in Fractional3D right)
        {
            return CheckInteractionGeometryIsChiralPair(left, right, right, left);
        }

        /// <summary>
        ///     Shifts all positions so that the resulting sequence first position is on the new position
        /// </summary>
        /// <param name="source"></param>
        /// <param name="newFirst"></param>
        /// <returns></returns>
        public IEnumerable<Fractional3D> ShiftFirstToPosition(IEnumerable<Fractional3D> source, in Fractional3D newFirst)
        {
            var sourceList = source.AsList();

            var shift = newFirst - sourceList[0];
            return sourceList.Select(value => value + shift);
        }

        /// <inheritdoc />
        public IWyckoffOperationDictionary GetOperationDictionary(in Fractional3D sourceVector)
        {
            var operationComparer =
                Comparer<ISymmetryOperation>.Create((a, b) => string.Compare(a.Literal, b.Literal, StringComparison.Ordinal));
            var dictionary = new SortedDictionary<Fractional3D, SetList<ISymmetryOperation>>(VectorComparer);

            foreach (var operation in LoadedGroup.Operations)
            {
                var vector = operation.ApplyWithTrim(sourceVector);
                if (!dictionary.ContainsKey(vector))
                    dictionary[vector] = new SetList<ISymmetryOperation>(operationComparer);

                dictionary[vector].Add(operation);
            }

            return new WyckoffOperationDictionary(sourceVector, LoadedGroup, dictionary);
        }

        /// <inheritdoc />
        public IList<ISymmetryOperation> GetMultiplicityOperations(in Fractional3D sourceVector, bool shiftCorrection)
        {
            var result = new List<ISymmetryOperation>(LoadedGroup.Operations.Count);
            foreach (var operation in LoadedGroup.Operations)
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
            var pointList = pointSequence.AsList();

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
        public SortedDictionary<Fractional3D, List<Fractional3D>> GetEnvironmentDictionary(IEnumerable<Fractional3D> refSequence)
        {
            var refList = refSequence.AsList();

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
            return new VectorComparer3D<T1>(DoubleComparer);
        }
    }
}