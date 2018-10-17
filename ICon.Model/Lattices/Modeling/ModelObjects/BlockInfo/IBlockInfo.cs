using System;
using System.Collections.Generic;
using System.Text;
using Mocassin.Mathematics.ValueTypes;
using Mocassin.Model.Basic;

namespace Mocassin.Model.Lattices
{
    /// <summary>
    /// Defines the origin and extent of a building block
    /// </summary>
    public interface IBlockInfo : IModelObject
    {
        /// <summary>
        /// BuildingBlock Index
        /// </summary>
        IBuildingBlock Block { get; }

        /// <summary>
        /// Origin of building Block
        /// </summary>
        DataIntegralVector3D Origin { get; }

        /// <summary>
        /// Extent of building Block
        /// </summary>
        DataIntegralVector3D Extent { get; }

    }
}
