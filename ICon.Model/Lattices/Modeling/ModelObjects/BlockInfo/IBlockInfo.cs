using System;
using System.Collections.Generic;
using System.Text;
using Mocassin.Mathematics.ValueTypes;
using Mocassin.Model.Basic;
using Moccasin.Mathematics.ValueTypes;

namespace Mocassin.Model.Lattices
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
        VectorInt3D Origin { get; }

        /// <summary>
        /// Extent of building Block
        /// </summary>
        VectorInt3D Extent { get; }

        /// <summary>
        /// Size of the BuildingBlockAssembly
        /// </summary>
        VectorInt3D Size { get; }

        IEnumerable<IBuildingBlock> GetBlockGrouping();
    }
}
