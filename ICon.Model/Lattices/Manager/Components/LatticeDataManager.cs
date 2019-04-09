﻿using System;
using System.Text;
using Mocassin.Mathematics.ValueTypes;
using Mocassin.Symmetry.Analysis;
using Mocassin.Model.Particles;
using Mocassin.Framework.Collections;
using Mocassin.Model.Basic;

namespace Mocassin.Model.Lattices
{
    /// <summary>
    /// Lattice data manager that provides safe read only access to the Lattice base model data
    /// </summary>
    internal class LatticeDataManager : ModelDataManager<LatticeModelData>, ILatticeDataPort
    {
        /// <summary>
        /// Create new lattice data manager for the provided data object
        /// </summary>
        /// <param name="modelData"></param>
        public LatticeDataManager(LatticeModelData modelData) : base(modelData)
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
        public ReadOnlyListAdapter<IDoping> GetDopings()
        {
            return ReadOnlyListAdapter<IDoping>.FromEnumerable(Data.Dopings);
        }

        /// <summary>
        /// Get read only list of block infos
        /// </summary>
        /// <returns></returns>
        public ReadOnlyListAdapter<IBlockInfo> GetBlockInfos()
        {
            return ReadOnlyListAdapter<IBlockInfo>.FromEnumerable(Data.BlockInfos);
        }

        /// <summary>
        /// Get read only list of building blocks
        /// </summary>
        /// <returns></returns>
        public ReadOnlyListAdapter<IBuildingBlock> GetBuildingBlocks()
        {
            return ReadOnlyListAdapter<IBuildingBlock>.FromEnumerable(Data.BuildingBlocks);
        }

        /// <summary>
        /// Get read only list of DopingCombinations
        /// </summary>
        /// <returns></returns>
        public ReadOnlyListAdapter<IDopingCombination> GetDopingCombinations()
        {
            return ReadOnlyListAdapter<IDopingCombination>.FromEnumerable(Data.DopingCombinations);
        }
    }
}
