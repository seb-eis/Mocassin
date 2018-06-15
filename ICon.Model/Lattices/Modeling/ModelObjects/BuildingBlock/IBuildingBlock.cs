using ICon.Model.Basic;
using ICon.Symmetry.Analysis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace ICon.Model.Lattices
{
    /// <summary>
    /// Building Block for the lattice. Each building block has the size of the unit cell.
    /// </summary>
    public interface IBuildingBlock : IModelObject, IEnumerable<uint>
    {
        /// <summary>
        /// The list interface of unit cell entries
        /// </summary>
        IList<uint> CellEntries { get; }

        /// <summary>
        /// Flag that indicates whether building block is user defined
        /// </summary>
        bool IsCustom { get; }
    }
}
