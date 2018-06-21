using ICon.Model.Basic;
using ICon.Model.Particles;
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
    public interface IBuildingBlock : IModelObject
    {
        /// <summary>
        /// The list interface of unit cell entries
        /// </summary>
        List<IParticle> CellEntries { get; }
    }
}
