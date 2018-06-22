using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Model.Lattices
{
    /// <summary>
    /// Temporary Object for lattice creation
    /// </summary>
    public class WorkCell
    {
        /// <summary>
        /// Flag to indicate if Workcell may be doped
        /// </summary>
        public int BuildingBlockID { get; set; }

        /// <summary>
        /// Cellentries of Workcell
        /// </summary>
        public CellEntry[] CellEntries { get; set; }
    }
}
