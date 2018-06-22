using System;
using System.Collections.Generic;
using System.Text;
using ICon.Mathematics.ValueTypes;
using ICon.Model.Basic;

namespace ICon.Model.Lattices
{
    /// <summary>
    /// Lattice information, such as the extent of the super cell
    /// </summary>
    public interface ILatticeInfo : IModelParameter
    {
        /// <summary>
        /// Extent of lattice
        /// </summary>
        DataIntVector3D Extent { get; }
    }
}
