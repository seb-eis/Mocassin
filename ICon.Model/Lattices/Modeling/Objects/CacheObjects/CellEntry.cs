using ICon.Model.Particles;
using ICon.Model.Structures;
using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Model.Lattices
{
    /// <summary>
    /// Temporary Object for lattice creation
    /// </summary>
    public class CellEntry
    {
        /// <summary>
        /// Particle information on cell entry position
        /// </summary>
        public IParticle Particle { get; set; }

        /// <summary>
        /// Sublattice information on cell entry position
        /// </summary>
        public IUnitCellPosition CellPosition { get; set; }
    }
}
