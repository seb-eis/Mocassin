using Mocassin.Symmetry.Analysis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Mocassin.Model.Basic;
using Mocassin.Model.Particles;

namespace Mocassin.Model.Lattices
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
