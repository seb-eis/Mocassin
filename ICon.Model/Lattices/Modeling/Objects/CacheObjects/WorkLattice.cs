using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Model.Lattices
{
    /// <summary>
    /// Temporary lattice for later creation of Supercellwrapper
    /// </summary>
    public class WorkLattice
    {
        /// <summary>
        /// Work Cells (building blocks) of WorkLattice
        /// </summary>
        public WorkCell[,,] WorkCells { get; set; }

    }
}
