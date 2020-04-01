using System;
using System.Collections.Generic;
using System.Linq;
using Mocassin.Framework.Extensions;
using Mocassin.Mathematics.Comparer;
using Mocassin.Mathematics.Constraints;
using Mocassin.Mathematics.ValueTypes;

namespace Mocassin.Symmetry.Analysis
{
    /// <summary>
    ///     Searches unit cell providers for cell entry chains that potentially match the input geometry. The class does not
    ///     perform the actual symmetry comparison
    /// </summary>
    public class PositionChainSampler<T>
    {
        /// <summary>
        ///     Unit cell provider that is searched
        /// </summary>
        public IUnitCellProvider<T> UnitCellProvider { get; set; }

        /// <summary>
        ///     The double comparer for the tolerance comparison of the distances
        /// </summary>
        public NumericComparer NumericComparer { get; set; }

        /// <summary>
        ///     Creates new chain cell searcher from a unit cell provider and double comparer for radial search with tolerance
        /// </summary>
        /// <param name="unitCellProvider"></param>
        /// <param name="numericComparer"></param>
        public PositionChainSampler(IUnitCellProvider<T> unitCellProvider, NumericComparer numericComparer)
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
        public IEnumerable<IList<LatticePoint<T>>> MultiPointSearch(IEnumerable<Fractional3D> vectors, IEnumerable<LatticePoint<T>> refGeometry,
            IEqualityComparer<T> comparer)
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
        public IEnumerable<IList<LatticePoint<T>>> PointSearch(Fractional3D startVector, IEnumerable<LatticePoint<T>> refGeometry,
            IEqualityComparer<T> comparer)
        {
            if (refGeometry == null)
                throw new ArgumentNullException(nameof(refGeometry));

            if (comparer == null)
                throw new ArgumentNullException(nameof(comparer));

            var geometry = refGeometry.ToList();
            return MakeSearchEnumerable(GetStartSequence(geometry[0].Content, startVector), GetDefaultConstraints(geometry),
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
        protected IEnumerable<IList<LatticePoint<T>>> MakeSearchEnumerable(IEnumerable<IList<LatticePoint<T>>> startSequence,
            IList<NumericConstraint> constraints, IList<Func<T, bool>> predicates)
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
        protected IEnumerable<LatticePoint<T>> ShellSearch(Fractional3D start, NumericConstraint constraint, Func<T, bool> predicate)
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
        protected IEnumerable<IList<LatticePoint<T>>> ExtendSearchSequence(IEnumerable<IList<LatticePoint<T>>> sequences,
            NumericConstraint radialConstraint, Func<T, bool> predicate)
        {
            foreach (var sequence in sequences)
            {
                foreach (var item in ShellSearch(sequence[sequence.Count - 1].Fractional, radialConstraint, predicate))
                {
                    var extended = new List<LatticePoint<T>>(sequence.Count + 1);
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
        protected IEnumerable<IList<LatticePoint<T>>> GetStartSequence(T entry, in Fractional3D vector)
        {
            return new List<List<LatticePoint<T>>>
            {
                new List<LatticePoint<T>>
                {
                    new LatticePoint<T>(vector, entry)
                }
            };
        }

        /// <summary>
        ///     Takes a reference geometry and creates a sequence of radial search constraints to lookup possible symmetry
        ///     equivalents. Length is geometry -1
        /// </summary>
        /// <param name="geometry"></param>
        /// <returns></returns>
        public IList<NumericConstraint> GetDefaultConstraints(List<LatticePoint<T>> geometry)
        {
            var cartesianGeometry = geometry
                .Select(entry => UnitCellProvider.VectorEncoder.Transformer.ToCartesian(entry.Fractional))
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
        public IList<Func<T, bool>> GetDefaultPredicates(IList<LatticePoint<T>> geometry, IEqualityComparer<T> comparer)
        {
            var result = new List<Func<T, bool>>(geometry.Count - 1);
            for (var i = 0; i < geometry.Count - 1; i++)
                result.Add(GetDefaultEntryPredicate(geometry[i + 1].Content, comparer));

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
        protected Func<T, bool> GetDefaultEntryPredicate(T expectedEntry, IEqualityComparer<T> comparer)
        {
            return value => comparer.Equals(expectedEntry, value);
        }
    }
}