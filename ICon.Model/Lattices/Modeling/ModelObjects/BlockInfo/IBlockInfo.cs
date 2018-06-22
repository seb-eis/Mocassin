﻿using System;
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
        List<IBuildingBlock> BlockAssembly { get; }

        /// <summary>
        /// Origin of building Block
        /// </summary>
        DataIntVector3D Origin { get; }

        /// <summary>
        /// Extent of building Block
        /// </summary>
        DataIntVector3D Extent { get; }

        /// <summary>
        /// Size of the BuildingBlockAssembly
        /// </summary>
        DataIntVector3D Size { get; }
    }
}
