using System;
using System.Collections.Generic;
using System.Text;
using Mocassin.Model.Particles;
using Mocassin.Model.Structures;

namespace Mocassin.Model.Lattices
{
    /// <summary>
    /// Temporary Object for lattice creation
    /// </summary>
    public struct CellEntry
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
