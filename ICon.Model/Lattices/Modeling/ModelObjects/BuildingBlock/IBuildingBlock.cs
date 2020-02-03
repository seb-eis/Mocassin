using System.Collections.Generic;
using Mocassin.Model.Basic;
using Mocassin.Model.Particles;

namespace Mocassin.Model.Lattices
{
    /// <summary>
    ///     Building Block for the lattice. Each building block has the size of the unit cell.
    /// </summary>
    public interface IBuildingBlock : IModelObject
    {
        /// <summary>
        ///     The occupation of the building block
        /// </summary>
        IReadOnlyList<IParticle> CellEntries { get; }
    }
}