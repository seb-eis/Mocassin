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
    [Serializable]
    [DataContract(Name = "CustomBlockSet")]
    public class BlockInfo : ModelObject, IBlockInfo
    {

        /// <summary>
        /// Origin of building Block
        /// </summary>
        [DataMember]
        public DataIntegralVector3D Origin { get; set; }

        /// <summary>
        /// Extent of building Block
        /// </summary>
        [DataMember]
        public DataIntegralVector3D Extent { get; set; }

        /// <summary>
        /// Origin of building blocks in X direction
        /// </summary>
        [IgnoreDataMember]
        public int OriginX => Origin.A;

        /// <summary>
        /// Origin of building blocks in Y direction
        /// </summary>
        [IgnoreDataMember]
        public int OriginY => Origin.B;

        /// <summary>
        /// Origin of building blocks in Z direction
        /// </summary>
        [IgnoreDataMember]
        public int OriginZ => Origin.C;

        /// <summary>
        /// Number of building blocks in X direction
        /// </summary>
        [IgnoreDataMember]
        public int ExtentX => Extent.A;

        /// <summary>
        /// Number of building blocks in Y direction
        /// </summary>
        [IgnoreDataMember]
        public int ExtentY => Extent.B;

        /// <summary>
        /// Number of building blocks in Z direction
        /// </summary>
        [IgnoreDataMember]
        public int ExtentZ => Extent.C;

        /// <summary>
        /// User defined building block
        /// </summary>
        [DataMember(Name = "Block")]
        public int BuildingBlockID { get; set; }

        /// <summary>
        /// Get the type name string
        /// </summary>
        /// <returns></returns>
        public override string GetModelObjectName()
        {
            return "'Building Block Info'";
        }

        /// <summary>
        /// creates a string that contains the parameter information
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{GetModelObjectName()} ({Origin}, {Extent}, {BuildingBlockID})";
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
                return this;
            }
            return null;
        }
    }
}
