using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using ICon.Mathematics.ValueTypes;
using ICon.Model.Basic;

namespace ICon.Model.Lattices
{
    /// <summary>
    /// Defines the origin and extent of a building block
    /// </summary>
    [DataContract(Name = "BlockInfo")]
    public class BlockInfo : ModelObject, IBlockInfo
    {
        /// <summary>
        /// BuildingBlock Index
        /// </summary>
        [DataMember]
        [LinkableByIndex]
        public IBuildingBlock Block { get; set; }

        /// <summary>
        /// Origin of BuildingBlock
        /// </summary>
        [DataMember]
        public DataIntegralVector3D Origin { get; set; }

        /// <summary>
        /// Extent of BuildingBlock
        /// </summary>
        [DataMember]
        public DataIntegralVector3D Extent { get; set; }

        /// <summary>
        /// Get the type name string
        /// </summary>
        /// <returns></returns>
        public override string GetModelObjectName()
        {
            return "'Building Block Info'";
        }

        /// <summary>
        /// Copies the information from the provided model object interface and returns the object (Retruns null if type mismatch)
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override ModelObject PopulateObject(IModelObject obj)
        {
            if (CastWithDepricatedCheck<IBlockInfo>(obj) is var blockInfo)
            {
                Origin = blockInfo.Origin;
                Extent = blockInfo.Extent;
                Block = blockInfo.Block;
                return this;
            }
            return null;
        }
    }
}
