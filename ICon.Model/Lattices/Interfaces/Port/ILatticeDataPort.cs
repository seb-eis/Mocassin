using System;
using System.Collections.Generic;
using System.Text;

using ICon.Framework.Collections;
using ICon.Mathematics.ValueTypes;
using ICon.Model.Basic;
using ICon.Model.Particles;
using ICon.Symmetry.Analysis;

namespace ICon.Model.Lattices
{
    /// <summary>
    /// Represents a read only data access port for the lattice reference data
    /// </summary>
    public interface ILatticeDataPort : IModelDataPort
    {
        /// <summary>
        /// Get super cell dimensions
        /// </summary>
        /// <returns></returns>
        ILatticeInfo GetLatticeInfo();

        /// <summary>
        /// Get read only list of used dopings
        /// </summary>
        /// <returns></returns>
        ReadOnlyListAdapter<IDoping> GetDopings();

        /// <summary>
        /// Get read only list of DopingCombinations (dopant, doped element, unit cell entry)
        /// </summary>
        /// <returns></returns>
        ReadOnlyListAdapter<IDopingCombination> GetDopingCombinations();

        /// <summary>
        /// Get read only list of building blocks
        /// </summary>
        /// <returns></returns>
        ReadOnlyListAdapter<IBuildingBlock> GetBuildingBlocks();

        /// <summary>
        /// Get read only list of block infos
        /// </summary>
        /// <returns></returns>
        ReadOnlyListAdapter<IBlockInfo> GetBlockInfos();

    }
}
