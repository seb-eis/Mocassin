using System;
using System.Text;
using ICon.Framework.Collections;

using ICon.Model.Basic;
using ICon.Mathematics.ValueTypes;
using ICon.Symmetry.Analysis;
using ICon.Model.Particles;

namespace ICon.Model.Lattices
{
    /// <summary>
    /// Lattice data manager that provides safe read only access to the Lattice base model data
    /// </summary>
    internal class LatticeDataManager : ModelDataManager<LatticeModelData>, ILatticeDataPort
    {
        /// <summary>
        /// Create new lattice data manager for the provided data object
        /// </summary>
        /// <param name="data"></param>
        public LatticeDataManager(LatticeModelData data) : base(data)
        {
        }

        /// <summary>
        /// Get lattice info
        /// </summary>
        /// <returns></returns>
        public ILatticeInfo GetLatticeInfo()
        {
            return Data.LatticeInfo;
        }

        /// <summary>
        /// Get read only list of used dopings
        /// </summary>
        /// <returns></returns>
        public ReadOnlyList<IDoping> GetDopings()
        {
            return ReadOnlyList<IDoping>.FromEnumerable(Data.Dopings);
        }

        /// <summary>
        /// Get read only list of block infos
        /// </summary>
        /// <returns></returns>
        public ReadOnlyList<IBlockInfo> GetBlockInfos()
        {
            return ReadOnlyList<IBlockInfo>.FromEnumerable(Data.BlockInfos);
        }

        /// <summary>
        /// Get read only list of building blocks
        /// </summary>
        /// <returns></returns>
        public ReadOnlyList<IBuildingBlock> GetBuildingBlocks()
        {
            return ReadOnlyList<IBuildingBlock>.FromEnumerable(Data.BuildingBlocks);
        }

        /// <summary>
        /// Get read only list of DopingCombinations
        /// </summary>
        /// <returns></returns>
        public ReadOnlyList<IDopingCombination> GetDopingCombinations()
        {
            return ReadOnlyList<IDopingCombination>.FromEnumerable(Data.DopingCombinations);
        }
    }
}
