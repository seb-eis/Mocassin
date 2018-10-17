using System;
using System.Collections.Generic;
using System.Text;
using Mocassin.Mathematics.ValueTypes;
using Mocassin.Model.Basic;

namespace Mocassin.Model.Lattices
{
    /// <summary>
    /// Lattice information, such as the extent of the super cell
    /// </summary>
    public interface ILatticeInfo : IModelParameter
    {
        /// <summary>
        /// Extent of lattice
        /// </summary>
        DataIntegralVector3D Extent { get; }
    }
}
