using System;
using System.Collections.Generic;
using System.Text;

using ICon.Mathematics.Coordinates;

namespace ICon.Symmetry.Analysis
{
    /// <summary>
    /// Represents a generic unit cell at arbitrary (a,b,c) offset that supplies a limited set of sorted unit cell entries of specififed type
    /// </summary>
    public interface IUnitCell<T1>
    {
        /// <summary>
        /// Get the number of entries in the cell
        /// </summary>
        int EntryCount { get; }

        /// <summary>
        /// Get the unit cell vector encoder of the unit cell
        /// </summary>
        IUnitCellVectorEncoder VectorEncoder { get; }

        /// <summary>
        /// Access the unit cell at the specififed position index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        CellEntry<T1> this[int index] { get; }

        /// <summary>
        /// Get all unit cell entries as en enumerable
        /// </summary>
        /// <returns></returns>
        IEnumerable<CellEntry<T1>> GetAllEntries();
    }
}
