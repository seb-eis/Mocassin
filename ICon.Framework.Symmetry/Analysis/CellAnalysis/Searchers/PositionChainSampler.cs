using System;
using System.Collections.Generic;
using System.Linq;

using ICon.Mathematics.Constraints;
using ICon.Mathematics.Comparers;
using ICon.Mathematics.ValueTypes;

namespace ICon.Symmetry.Analysis
{
    /// <summary>
    /// Searches unit cell providers for cell entry chains that potentially match the input geometry. The class does not perform the actual symmetry comparsions
    /// </summary>
    public class PositionChainSampler<T1>
    {
        /// <summary>
        /// Unit cell provider that is searched
        /// </summary>
        public IUnitCellProvider<T1> UnitCellProvider { get; set; }

        /// <summary>
        /// The double comparer for the tolerance comparison of the distances
        /// </summary>
        public DoubleComparer DoubleComparer { get; set; }

        /// <summary>
        /// Creates new chain cell searcher from a unit cell provider and double comparer for radial search with tolerance
        /// </summary>
        /// <param name="unitCellProvider"></param>
        /// <param name="doubleComparer"></param>
        public PositionChainSampler(IUnitCellProvider<T1> unitCellProvider, DoubleComparer doubleComparer)
        {
            UnitCellProvider = unitCellProvider ?? throw new ArgumentNullException(nameof(unitCellProvider));
            DoubleComparer = doubleComparer ?? throw new ArgumentNullException(nameof(doubleComparer));
        }

        /// <summary>
        /// Get an enumerbale that contains the chain searches around all provided start vectors for the specified refernce geoemtry
        /// </summary>
        /// <param name="vectors"></param>
        /// <param name="refGeoemtry"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public IEnumerable<CellEntry<T1>[]> MultiPointSearch(IEnumerable<Fractional3D> vectors, IEnumerable<CellEntry<T1>> refGeoemtry, IEqualityComparer<T1> comparer)
        {
            foreach (var start in vectors)
            {
                foreach (var item in PointSearch(start, refGeoemtry, comparer))
                {
                    yield return item;
                }
            }
        }

        /// <summary>
        /// Get a default chain search enumerable from the provided start vector that contains all found geometry sequneces that potentially match the provided refernce
        /// </summary>
        /// <param name="startVector"></param>
        /// <param name="refGeoemtry"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public IEnumerable<CellEntry<T1>[]> PointSearch(Fractional3D startVector, IEnumerable<CellEntry<T1>> refGeoemtry, IEqualityComparer<T1> comparer)
        {
            if (refGeoemtry == null)
            {
                throw new ArgumentNullException(nameof(refGeoemtry));
            }
            if (comparer == null)
            {
                throw new ArgumentNullException(nameof(comparer));
            }
            var geoemtry = refGeoemtry.ToArray();
            return MakeSearchEnumerable(GetStartSequence(geoemtry[0].Entry, startVector), GetDefaultConstraints(geoemtry), GetDefaultPredicates(geoemtry, comparer));
        }

        /// <summary>
        /// Gets a custom chain search enumerable for the provided start sequence. The search is limited by the provided set of radial range constraints and predicates 
        /// </summary>
        /// <param name="startSequence"></param>
        /// <param name="constraints"></param>
        /// <param name="predicates"></param>
        /// <returns></returns>
        protected IEnumerable<CellEntry<T1>[]> MakeSearchEnumerable(IEnumerable<CellEntry<T1>[]> startSequence, DoubleConstraint[] constraints, Predicate<T1>[] predicates)
        {
            if (constraints.Length != predicates.Length)
            {
                throw new ArgumentException("Incompatible size of the predicate array", nameof(predicates));
            }
            for (int i = 0; i < constraints.Length; i++)
            {
                startSequence = ExtendSearchSequence(startSequence, constraints[i], predicates[i]);
            }
            foreach (var item in startSequence)
            {
                yield return item;
            }
        }

        /// <summary>
        /// Get an enumerable for all cell entries around a specififed start that fulfill the requrements of the distant constraint and provided prediacte
        /// </summary>
        /// <param name="start"></param>
        /// <param name="constraint"></param>
        /// <param name="targetValue"></param>
        /// <returns></returns>
        protected IEnumerable<CellEntry<T1>> ShellSearch(Fractional3D start, DoubleConstraint constraint, Predicate<T1> predicate)
        {
            foreach (var item in new RadialPositionSampler().Search(UnitCellProvider, start, constraint, predicate))
            {
                yield return item;
            }
        }

        /// <summary>
        /// Extends the chain search enumerable by the next cell search iterator
        /// </summary>
        /// <param name="sequences"></param>
        /// <param name="radialConstraint"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        protected IEnumerable<CellEntry<T1>[]> ExtendSearchSequence(IEnumerable<CellEntry<T1>[]> sequences, DoubleConstraint radialConstraint, Predicate<T1> predicate)
        {
            foreach (var sequence in sequences)
            {
                foreach (var item in ShellSearch(sequence[sequence.Length - 1].AbsoluteVector, radialConstraint, predicate))
                {
                    var extended = new CellEntry<T1>[sequence.Length + 1];
                    extended[sequence.Length] = item;
                    sequence.CopyTo(extended, 0);
                    yield return extended;
                }
            }
        }

        /// <summary>
        /// Creates the search starting sequence from a single cell entry information
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="vector"></param>
        /// <returns></returns>
        protected IEnumerable<CellEntry<T1>[]> GetStartSequence(T1 entry, in Fractional3D vector)
        {
            return new List<CellEntry<T1>[]> { new CellEntry<T1>[] { new CellEntry<T1>(vector, entry) } };
        }

        /// <summary>
        /// Takes a refernce geometry and creates a sequence of radial search constraints to lookup possible symmetry equivalents. Length is geoemtry -1
        /// </summary>
        /// <param name="geometry"></param>
        /// <returns></returns>
        public DoubleConstraint[] GetDefaultConstraints(CellEntry<T1>[] geometry)
        {
            var cartesianGeometry = geometry.Select(entry => UnitCellProvider.VectorEncoder.Transformer.ToCartesian(entry.AbsoluteVector)).ToArray();
            var result = new DoubleConstraint[cartesianGeometry.Length - 1];
            for (int i = 0; i < cartesianGeometry.Length - 1;)
            {
                result[i] = GetSearchConstraint(cartesianGeometry[i], cartesianGeometry[++i]);
            }
            return result;
        }

        /// <summary>
        /// Creates the default search predicates that compare the cell entries for equality using the provided comparer. Length is geometry - 1
        /// </summary>
        /// <param name="geometry"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public Predicate<T1>[] GetDefaultPredicates(CellEntry<T1>[] geometry, IEqualityComparer<T1> comparer)
        {
            var result = new Predicate<T1>[geometry.Length - 1];
            for (int i = 0; i < geometry.Length - 1; i++)
            {
                result[i] = GetDefaultEntryPredicate(geometry[i+1].Entry, comparer);
            }
            return result;
        }

        /// <summary>
        /// Takes two cartesian vectors and creates a radial search constraint for the distance between the vector points
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        protected DoubleConstraint GetSearchConstraint(in Cartesian3D first, in Cartesian3D second)
        {
            double length = (second - first).GetLength();
            return new DoubleConstraint(true, length, length, true, DoubleComparer);
        }

        /// <summary>
        /// Creates the default entry comparer predicate that compares the occuring entries to the spceififed expected one
        /// </summary>
        /// <param name="expectedEntry"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        protected Predicate<T1> GetDefaultEntryPredicate(T1 expectedEntry, IEqualityComparer<T1> comparer)
        {
            return (T1 value) => comparer.Equals(expectedEntry, value);
        }
    }
}
