using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Mocassin.Model.Basic;

namespace Mocassin.Model.Lattices
{
    /// <summary>
    ///     The reference model data object for the lattice manager
    /// </summary>
    [Serializable]
    [DataContract(Name = "LatticeModelData")]
    public class LatticeModelData : ModelData<ILatticeDataPort>
    {
        /// <summary>
        ///     Default building block (index = 0) and custom building blocks of lattice
        /// </summary>
        [DataMember]
        [IndexedModelData(typeof(IBuildingBlock))]
        public List<BuildingBlock> BuildingBlocks { get; set; }

        /// <summary>
        ///     List of doping combinations (dopant, doped element, unitcell position)
        /// </summary>
        [DataMember]
        [IndexedModelData(typeof(IDopingCombination))]
        public List<DopingCombination> DopingCombinations { get; set; }

        /// <summary>
        ///     The dopings specified by the user
        /// </summary>
        [DataMember]
        [IndexedModelData(typeof(IDoping))]
        public List<Doping> Dopings { get; set; }

        /// <summary>
        ///     Creates new read only wrapper for this data object
        /// </summary>
        /// <returns></returns>
        public override ILatticeDataPort AsReadOnly()
        {
            return new LatticeDataManager(this);
        }

        /// <summary>
        ///     Resets the lattice model data to default conditions
        /// </summary>
        public override void ResetToDefault()
        {
            ResetAllIndexedData();
        }

        /// <summary>
        ///     Creates a new default lattice model data object
        /// </summary>
        /// <returns></returns>
        public static LatticeModelData CreateNew()
        {
            return CreateDefault<LatticeModelData>();
        }
    }
}