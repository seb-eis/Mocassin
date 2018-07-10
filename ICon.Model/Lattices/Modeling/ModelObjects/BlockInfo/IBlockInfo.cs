using System;
using System.Collections.Generic;
using System.Text;
using ICon.Mathematics.ValueTypes;
using ICon.Model.Basic;

namespace ICon.Model.Lattices
{
    /// <summary>
    /// Defines the origin and extent of a building block
    /// </summary>
    public interface IBlockInfo : IModelObject
    {
        /// <summary>
        /// Assembly of BuildingBlocks
        /// </summary>
        List<IBuildingBlock> BlockGrouping { get; }

        /// <summary>
        /// Origin of building Block
        /// </summary>
        CartesianInt3D Origin { get; }

        /// <summary>
        /// Extent of building Block
        /// </summary>
        CartesianInt3D Extent { get; }

        /// <summary>
        /// Size of the BuildingBlockAssembly
        /// </summary>
        CartesianInt3D Size { get; }
    }
}
