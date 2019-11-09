﻿using System;
using System.Collections.Generic;
using Mocassin.Framework.Collections;
using Mocassin.Mathematics.Comparers;
using Mocassin.Mathematics.Constraints;
using Mocassin.Mathematics.Coordinates;
using Mocassin.Mathematics.ValueTypes;
using Moccasin.Mathematics.ValueTypes;

namespace Mocassin.Symmetry.Analysis
{
    /// <summary>
    ///     Generic unit cell entry locator that performs a radial search with a provided search criteria around a start
    ///     position
    ///     in a unit cell provider
    /// </summary>
    public class RadialPositionSampler
    {
        /// <summary>
        ///     The current basic offset for the active search
        /// </summary>
        protected VectorI3 BaseOffset { get; set; }

        /// <summary>
        ///     The currently active start position
        /// </summary>
        protected Cartesian3D StartVector { get; set; }

        /// <summary>
        ///     The current shift vector (negative start vector as fractional)
        /// </summary>
        protected Fractional3D StartShift { get; set; }

        /// <summary>
        ///     The currently active boundary info
        /// </summary>
        protected SearchBoundaryProvider BoundaryInfo { get; set; }

        /// <summary>
        ///     The currently active constraint for the search
        /// </summary>
        protected NumericConstraint Constraint { get; set; }

        /// <summary>
        ///     The currently active vector encoder found in the unit cell provider
        /// </summary>
        protected IUnitCellVectorEncoder VectorEncoder { get; set; }

        /// <summary>
        ///     The vector comparer for cartesian objects that uses the same tolerance as the specified double constraint
        /// </summary>
        protected VectorComparer3D<Cartesian3D> VectorComparer { get; set; }

        /// <summary>
        ///     Performs a radial entry search for entries in the unit cell provider.
        ///     The search is limited by the hollow sphere defined by start and constraint
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="provider"></param>
        /// <param name="start"></param>
        /// <param name="constraint"></param>
        /// <returns></returns>
        public IList<CellEntry<T1>> Search<T1>(IUnitCellProvider<T1> provider, in Fractional3D start, NumericConstraint constraint)
        {
            return Search(provider, start, constraint, value => true);
        }

        /// <summary>
        ///     Performs a radial entry search for entries in the unit cell provider.
        ///     The search is limited by the hollow sphere defined by start and constraint and the result sorted using the provided
        ///     comparer
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="provider"></param>
        /// <param name="start"></param>
        /// <param name="constraint"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public IList<CellEntry<T1>> Search<T1>(IUnitCellProvider<T1> provider, in Fractional3D start, NumericConstraint constraint,
            IComparer<CellEntry<T1>> comparer)
        {
            return Search(provider, start, constraint, value => true, comparer);
        }

        /// <summary>
        ///     Performs a radial entry search for all entries fulfilling the predicate in the unit cell provider.
        ///     The search is limited by the hollow sphere defined by start and constraint
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="provider"></param>
        /// <param name="start"></param>
        /// <param name="constraint"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IList<CellEntry<T1>> Search<T1>(IUnitCellProvider<T1> provider, in Fractional3D start, NumericConstraint constraint,
            Predicate<T1> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            SetBasicCalculationProperties(provider, constraint, start);
            var results = new List<CellEntry<T1>>(500);

            for (var offset = 0;; offset++)
            {
                SearchCellSet(results, provider, predicate, offset);
                if (CheckAndUpdateBoundaryInfo(offset, constraint))
                    break;
            }

            return results;
        }

        /// <summary>
        ///     Performs a radial entry search for all entries fulfilling the predicate in the unit cell provider.
        ///     The search is limited by the hollow sphere defined by start and constraint and the result sorted by the supplied
        ///     comparer
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="provider"></param>
        /// <param name="start"></param>
        /// <param name="constraint"></param>
        /// <param name="predicate"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public IList<CellEntry<T1>> Search<T1>(IUnitCellProvider<T1> provider, in Fractional3D start, NumericConstraint constraint,
            Predicate<T1> predicate, IComparer<CellEntry<T1>> comparer)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            SetBasicCalculationProperties(provider, constraint, start);
            var results = new MultisetList<CellEntry<T1>>(comparer, 500);

            for (var offset = 0;; offset++)
            {
                SearchCellSet(results, provider, predicate, offset);
                if (CheckAndUpdateBoundaryInfo(offset, constraint))
                    break;
            }

            return results;
        }

        /// <summary>
        ///     Searches all unit cells with the specified offset from the start cell for positions within the constraint range
        ///     that match the predicate
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="results"></param>
        /// <param name="provider"></param>
        /// <param name="predicate"></param>
        /// <param name="offset"></param>
        protected void SearchCellSet<T1>(IList<CellEntry<T1>> results, IUnitCellProvider<T1> provider, Predicate<T1> predicate, int offset)
        {
            for (var a = -offset; a <= offset; a++)
            {
                for (var b = -offset; b <= offset; b++)
                {
                    for (var c = -offset; c <= offset; c++)
                    {
                        if (Math.Abs(a) != offset && Math.Abs(b) != offset && Math.Abs(c) != offset) continue;

                        var cellOffsets = new VectorI3(BaseOffset.A + a, BaseOffset.B + b, BaseOffset.C + c);
                        var cellOffsetVector =
                            VectorEncoder.Transformer.ToCartesian(new Fractional3D(cellOffsets.A, cellOffsets.B, cellOffsets.C));
                        var unitCell = provider.GetUnitCell(cellOffsets.A, cellOffsets.B, cellOffsets.C);

                        SearchUnitCell(results, unitCell, predicate, cellOffsetVector);
                    }
                }
            }
        }

        /// <summary>
        ///     Searches a single cell that starts at the specified cartesian offset vector for entries matching the radial search
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="results"></param>
        /// <param name="unitCell"></param>
        /// <param name="predicate"></param>
        /// <param name="cellOffsetVector"></param>
        protected void SearchUnitCell<T1>(IList<CellEntry<T1>> results, IUnitCell<T1> unitCell, Predicate<T1> predicate,
            in Cartesian3D cellOffsetVector)
        {
            var index = 0;
            foreach (var position in unitCell.GetAllEntries())
            {
                if (predicate(position.Entry))
                {
                    var absoluteVector = cellOffsetVector + VectorEncoder.GetCartesianPosition(index);
                    if (Constraint.IsValid((absoluteVector - StartVector).GetLength()))
                        results.Add(position);
                }

                index++;
            }
        }

        /// <summary>
        ///     Sets the properties required for calculations (Base offset, start vector and boundary distance info) and corrects
        ///     the boundary distances to the active start cell
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="constraint"></param>
        /// <param name="start"></param>
        protected void SetBasicCalculationProperties<T1>(IUnitCellProvider<T1> provider, NumericConstraint constraint,
            in Fractional3D start)
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));

            if (constraint == null)
                throw new ArgumentNullException(nameof(constraint));

            VectorEncoder = provider.VectorEncoder;
            VectorComparer = new VectorComparer3D<Cartesian3D>(constraint.Comparer);
            Constraint = constraint;
            StartShift = new Fractional3D(0, 0, 0) - start;
            StartVector = VectorEncoder.Transformer.ToCartesian(start);
            BaseOffset = VectorEncoder.GetTargetCellOffset(start);
            BoundaryInfo = new SearchBoundaryProvider(StartVector, VectorEncoder.GetBaseVectors());

            BoundaryInfo.ChangeDistanceToAbPlain(-BaseOffset.C);
            BoundaryInfo.ChangeDistanceToAcPlain(-BaseOffset.B);
            BoundaryInfo.ChangeDistanceToBcPlain(-BaseOffset.A);
        }

        /// <summary>
        ///     Updates the boundaries by one step. Returns true if the boundary break condition (Search radius is smaller than
        ///     current boundaries) is reached
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="constraint"></param>
        /// <returns></returns>
        protected bool CheckAndUpdateBoundaryInfo(int offset, NumericConstraint constraint)
        {
            if (offset != 0)
                BoundaryInfo.ChangeAllDistances(1);

            return BoundaryInfo.DistanceWithinBoundaries(constraint.MaxValue, constraint.Comparer);
        }


        /// <summary>
        ///     Creates a combined distance and entry priority comparer that sorts cell entries by distance and than the priority
        ///     of the entry
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="start"></param>
        /// <param name="entryComparer"></param>
        /// <returns></returns>
        public IComparer<CellEntry<T1>> MakeDistancePriorityComparer<T1>(Fractional3D start, IComparer<T1> entryComparer)
        {
            int CompareLengthThanValue(CellEntry<T1> lhs, CellEntry<T1> rhs)
            {
                var lhsLength = VectorEncoder.Transformer.ToCartesian(lhs.AbsoluteVector - start).GetLength();
                var rhsLength = VectorEncoder.Transformer.ToCartesian(rhs.AbsoluteVector - start).GetLength();
                var radialCompare = VectorEncoder.Transformer.FractionalSystem.Comparer.Compare(lhsLength, rhsLength);

                return radialCompare == 0
                    ? entryComparer.Compare(lhs.Entry, rhs.Entry)
                    : radialCompare;
            }

            return new CompareAdapter<CellEntry<T1>>(CompareLengthThanValue);
        }
    }
}