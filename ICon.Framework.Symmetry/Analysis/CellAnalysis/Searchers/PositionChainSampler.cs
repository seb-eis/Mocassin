using System;
using System.Collections.Generic;
using System.Linq;
using Mocassin.Framework.Extensions;
using Mocassin.Mathematics.Comparers;
using Mocassin.Mathematics.Constraints;
using Mocassin.Mathematics.ValueTypes;

namespace Mocassin.Symmetry.Analysis
{
    /// <summary>
    ///     Searches unit cell providers for cell entry chains that potentially match the input geometry. The class does not
    ///     perform the actual symmetry comparison
    /// </summary>
    public class PositionChainSampler<T1>
    {
        /// <summary>
        ///     Unit cell provider that is searched
        /// </summary>
        public IUnitCellProvider<T1> UnitCellProvider { get; set; }

        /// <summary>
        ///     The double comparer for the tolerance comparison of the distances
        /// </summary>
        public NumericComparer NumericComparer { get; set; }

        /// <summary>
        ///     Creates new chain cell searcher from a unit cell provider and double comparer for radial search with tolerance
        /// </summary>
        /// <param name="unitCellProvider"></param>
        /// <param name="numericComparer"></param>
        public PositionChainSampler(IUnitCellProvider<T1> unitCellProvider, NumericComparer numericComparer)
        {
            UnitCellProvider = unitCellProvider ?? throw new ArgumentNullException(nameof(unitCellProvider));
            NumericComparer = numericComparer ?? throw new ArgumentNullException(nameof(numericComparer));
        }

        /// <summary>
        ///     Get an enumerable that contains the chain searches around all provided start vectors for the specified reference
        ///     geometry
        /// </summary>
        /// <param name="vectors"></param>
        /// <param name="refGeometry"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public IEnumerable<IList<CellEntry<T1>>> MultiPointSearch(IEnumerable<Fractional3D> vectors, IEnumerable<CellEntry<T1>> refGeometry,
            IEqualityComparer<T1> comparer)
        {
            var refGeoCollection = refGeometry.ToCollection();

            foreach (var start in vectors)
            {
                foreach (var item in PointSearch(start, refGeoCollection, comparer))
                    yield return item;
            }
        }

        /// <summary>
        ///     Get a default chain search enumerable from the provided start vector that contains all found geometry sequences
        ///     that potentially match the provided reference
        /// </summary>
        /// <param name="startVector"></param>
        /// <param name="refGeometry"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public IEnumerable<IList<CellEntry<T1>>> PointSearch(Fractional3D startVector, IEnumerable<CellEntry<T1>> refGeometry,
            IEqualityComparer<T1> comparer)
        {
            if (refGeometry == null)
                throw new ArgumentNullException(nameof(refGeometry));

            if (comparer == null)
                throw new ArgumentNullException(nameof(comparer));

            var geometry = refGeometry.ToList();
            return MakeSearchEnumerable(GetStartSequence(geometry[0].Entry, startVector), GetDefaultConstraints(geometry),
                GetDefaultPredicates(geometry, comparer));
        }

        /// <summary>
        ///     Gets a custom chain search enumerable for the provided start sequence. The search is limited by the provided set of
        ///     radial range constraints and predicates
        /// </summary>
        /// <param name="startSequence"></param>
        /// <param name="constraints"></param>
        /// <param name="predicates"></param>
        /// <returns></returns>
        protected IEnumerable<IList<CellEntry<T1>>> MakeSearchEnumerable(IEnumerable<IList<CellEntry<T1>>> startSequence,
            IList<NumericConstraint> constraints, IList<Predicate<T1>> predicates)
        {
            if (constraints.Count != predicates.Count)
                throw new ArgumentException("Incompatible size of the predicate list", nameof(predicates));

            for (var i = 0; i < constraints.Count; i++)
                startSequence = ExtendSearchSequence(startSequence, constraints[i], predicates[i]);

            foreach (var item in startSequence)
                yield return item;
        }

        /// <summary>
        ///     Get an enumerable for all cell entries around a specified start that fulfill the requirements of the distant
        ///     constraint and provided predicate
        /// </summary>
        /// <param name="start"></param>
        /// <param name="constraint"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        protected IEnumerable<CellEntry<T1>> ShellSearch(Fractional3D start, NumericConstraint constraint, Predicate<T1> predicate)
        {
            return new RadialPositionSampler().Search(UnitCellProvider, start, constraint, predicate);
        }

        /// <summary>
        ///     Extends the chain search enumerable by the next cell search iterator
        /// </summary>
        /// <param name="sequences"></param>
        /// <param name="radialConstraint"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        protected IEnumerable<IList<CellEntry<T1>>> ExtendSearchSequence(IEnumerable<IList<CellEntry<T1>>> sequences,
            NumericConstraint radialConstraint, Predicate<T1> predicate)
        {
            foreach (var sequence in sequences)
            {
                foreach (var item in ShellSearch(sequence[sequence.Count - 1].AbsoluteVector, radialConstraint, predicate))
                {
                    var extended = new List<CellEntry<T1>>(sequence.Count + 1);
                    extended.AddRange(sequence);
                    extended.Add(item);
                    yield return extended;
                }
            }
        }

        /// <summary>
        ///     Creates the search starting sequence from a single cell entry information
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="vector"></param>
        /// <returns></returns>
        protected IEnumerable<IList<CellEntry<T1>>> GetStartSequence(T1 entry, in Fractional3D vector)
        {
            return new List<List<CellEntry<T1>>>
            {
                new List<CellEntry<T1>>
                {
                    new CellEntry<T1>(vector, entry)
                }
            };
        }

        /// <summary>
        ///     Takes a reference geometry and creates a sequence of radial search constraints to lookup possible symmetry
        ///     equivalents. Length is geometry -1
        /// </summary>
        /// <param name="geometry"></param>
        /// <returns></returns>
        public IList<NumericConstraint> GetDefaultConstraints(List<CellEntry<T1>> geometry)
        {
            var cartesianGeometry = geometry
                .Select(entry => UnitCellProvider.VectorEncoder.Transformer.ToCartesian(entry.AbsoluteVector))
                .ToList();

            var result = new List<NumericConstraint>(cartesianGeometry.Count - 1);
            for (var i = 0; i < cartesianGeometry.Count - 1;)
                result.Add(GetSearchConstraint(cartesianGeometry[i], cartesianGeometry[++i]));

            return result;
        }

        /// <summary>
        ///     Creates the default search predicates that compare the cell entries for equality using the provided comparer.
        ///     Length is geometry - 1
        /// </summary>
        /// <param name="geometry"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public IList<Predicate<T1>> GetDefaultPredicates(IList<CellEntry<T1>> geometry, IEqualityComparer<T1> comparer)
        {
            var result = new List<Predicate<T1>>(geometry.Count - 1);
            for (var i = 0; i < geometry.Count - 1; i++)
                result.Add(GetDefaultEntryPredicate(geometry[i + 1].Entry, comparer));

            return result;
        }

        /// <summary>
        ///     Takes two cartesian vectors and creates a radial search constraint for the distance between the vector points
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        protected NumericConstraint GetSearchConstraint(in Cartesian3D first, in Cartesian3D second)
        {
            var length = (second - first).GetLength();
            return new NumericConstraint(true, length, length, true, NumericComparer);
        }

        /// <summary>
        ///     Creates the default entry comparer predicate that compares the occuring entries to the specified expected one
        /// </summary>
        /// <param name="expectedEntry"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        protected Predicate<T1> GetDefaultEntryPredicate(T1 expectedEntry, IEqualityComparer<T1> comparer)
        {
            return value => comparer.Equals(expectedEntry, value);
        }
    }
}