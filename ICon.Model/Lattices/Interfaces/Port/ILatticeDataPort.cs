﻿using System;
using System.Collections.Generic;
using System.Text;
using Mocassin.Mathematics.ValueTypes;
using Mocassin.Model.Particles;
using Mocassin.Symmetry.Analysis;
using Mocassin.Framework.Collections;
using Mocassin.Model.Basic;

namespace Mocassin.Model.Lattices
{
    /// <summary>
    /// Represents a read only data access port for the lattice reference data
    /// </summary>
    public interface ILatticeDataPort : IModelDataPort
    {

        /// <summary>
        /// Get read only list of used dopings
        /// </summary>
        /// <returns></returns>
        ListReadOnlyWrapper<IDoping> GetDopings();

        /// <summary>
        /// Get read only list of DopingCombinations (dopant, doped element, unit cell entry)
        /// </summary>
        /// <returns></returns>
        ListReadOnlyWrapper<IDopingCombination> GetDopingCombinations();

        /// <summary>
        /// Get read only list of building blocks
        /// </summary>
        /// <returns></returns>
        ListReadOnlyWrapper<IBuildingBlock> GetBuildingBlocks();

    }
}
