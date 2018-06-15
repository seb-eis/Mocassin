using System;
using System.Collections.Generic;
using System.Text;

using System.Runtime.Serialization;
using ICon.Model.Basic;
using ICon.Symmetry.Analysis;
using ICon.Model.Particles;

namespace ICon.Model.Lattices
{
    /// <summary>
    /// The reference model data object for the lattice manager
    /// </summary>
    [Serializable]
    [DataContract(Name ="LatticeModelData")]
    public class LatticeModelData : ModelData<ILatticeDataPort>
    {
        /// <summary>
        /// Informations about the lattice, such as the extent of the super cell
        /// </summary>
        [DataMember]
        [ModelParameter(typeof(ILatticeInfo))]
        public LatticeInfo LatticeInfo { get; set; }

        /// <summary>
        /// The dopings specified by the user
        /// </summary>
        [DataMember]
        [IndexedModelData(typeof(IDoping))]
        public List<Doping> Dopings { get; set; }

        /// <summary>
        /// Default building block (index = 0) and custom building blocks of lattice
        /// </summary>
        [DataMember]
        [IndexedModelData(typeof(UnitCellWrapper<IParticle>))]
        public List<UnitCellWrapper<IParticle>> BuildingBlocks { get; set; }

        /// <summary>
        /// List of building block information (origin and extent)
        /// </summary>
        [DataMember]
        [IndexedModelData(typeof(List<BlockInfo>))]
        public List<BlockInfo> BlockInfos { get; set; }

        /// <summary>
        /// Creates new read only wrapper for this data object
        /// </summary>
        /// <returns></returns>
        public override ILatticeDataPort AsReadOnly()
        {
            return new LatticeDataManager(this);
        }

        /// <summary>
        /// Resets the lattice model data to default conditions
        /// </summary>
        public override void ResetToDefault()
        {
            LatticeInfo = LatticeInfo.CreateDefault();
            Dopings = new List<Doping>();
            BuildingBlocks = new List<UnitCellWrapper<IParticle>>();
            BlockInfos = new List<BlockInfo>();
        }

        /// <summary>
        /// Creates a new default lattice model data object
        /// </summary>
        /// <returns></returns>
        public static LatticeModelData CreateNew()
        {
            return CreateDefault<LatticeModelData>();
        }
    }
}
