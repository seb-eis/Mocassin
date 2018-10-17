using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using Mocassin.Mathematics.ValueTypes;
using Mocassin.Model.Basic;

namespace Mocassin.Model.Lattices
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
        [IndexResolved]
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
        public override string GetObjectName()
        {
            return "'Building Block Info'";
        }

        /// <summary>
        /// Copies the information from the provided model object interface and returns the object (Retruns null if type mismatch)
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override ModelObject PopulateFrom(IModelObject obj)
        {
            if (CastIfNotDeprecated<IBlockInfo>(obj) is var blockInfo)
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
