﻿using System.Collections.Generic;
using Mocassin.Mathematics.Extensions;
using Mocassin.Mathematics.ValueTypes;

namespace Mocassin.Symmetry.Analysis
{
    /// <summary>
    ///     Provides extension methods for cell analysis with <see cref="IUnitCellProvider{T1}" /> interfaces
    /// </summary>
    public static class CellAnalysisExtensions
    {
        /// <summary>
        ///     Get an <see cref="IEnumerable{T}" /> sequence for a subset of a unit cell provider defined by a cuboid start and
        ///     end point
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cellProvider"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static IEnumerable<LatticePoint<T>> GetCuboid<T>(this IUnitCellProvider<T> cellProvider, Fractional3D start, Fractional3D end)
        {
            var comparer = cellProvider.VectorEncoder.Transformer.FractionalSystem.Comparer;

            var (aMin, bMin, cMin) = (start.A.FloorToInt(comparer), start.B.FloorToInt(comparer), start.C.FloorToInt(comparer));
            var (aMax, bMax, cMax) = (end.A.FloorToInt(comparer), end.B.FloorToInt(comparer), end.C.FloorToInt(comparer));

            for (var a = aMin; a <= aMax; a++)
            {
                for (var b = bMin; b <= bMax; b++)
                {
                    for (var c = cMin; c <= cMax; c++)
                    {
                        var cell = cellProvider.GetUnitCell(a, b, c);
                        foreach (var entry in cell.GetAllEntries())
                        {
                            var vector = entry.Fractional;
                            if (comparer.Compare(vector.A, start.A) < 0) continue;
                            if (comparer.Compare(vector.B, start.B) < 0) continue;
                            if (comparer.Compare(vector.C, start.C) < 0) continue;
                            if (comparer.Compare(vector.A, end.A) > 0) continue;
                            if (comparer.Compare(vector.B, end.B) > 0) continue;
                            if (comparer.Compare(vector.C, end.C) > 0) continue;
                            yield return entry;
                        }
                    }
                }
            }
        }
    }
}