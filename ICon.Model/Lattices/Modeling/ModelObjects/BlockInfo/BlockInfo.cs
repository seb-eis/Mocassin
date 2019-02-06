﻿using System;
using System.Collections.Generic;
using System.Linq;
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
        /// BuildingBlocks which may construct a superblock
        /// </summary>
        [DataMember]
        [UseTrackedReferences]
        public IBuildingBlock Block { get; set; }

        /// <summary>
        /// Origin of BuildingBlockAssembly
        /// </summary>
        [DataMember]
        public DataIntVector3D Origin { get; set; }

        /// <summary>
        /// Extent of BuildingBlockAssembly
        /// </summary>
        [DataMember]
        public DataIntVector3D Extent { get; set; }

        /// <summary>
        /// Size of the BuildingBlockAssembly
        /// </summary>
        [DataMember]
        public DataIntVector3D Size { get; set; }

        /// <summary>
        /// Return origin vector as read only struct
        /// </summary>
        [IgnoreDataMember]
        VectorInt3D IBlockInfo.Origin => Origin.AsReadOnly();

        /// <summary>
        /// Return extent vector as read only struct
        /// </summary>
        [IgnoreDataMember]
        VectorInt3D IBlockInfo.Extent => Extent.AsReadOnly();

        /// <summary>
        /// Return size vector as read only struct
        /// </summary>
        [IgnoreDataMember]
        VectorInt3D IBlockInfo.Size => Size.AsReadOnly();

        IEnumerable<IBuildingBlock> IBlockInfo.GetBlockGrouping()
        {
            return (BlockGrouping ?? new List<IBuildingBlock>()).AsEnumerable();
        }

		/// <summary>
		/// Get the type name string
		/// </summary>
		/// <returns></returns>
		public override string ObjectName => "Building Block Info";

		/// <summary>
		/// Copies the information from the provided model object interface and returns the object (Retruns null if type mismatch)
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override ModelObject PopulateFrom(IModelObject obj)
        {
            if (CastIfNotDeprecated<IBlockInfo>(obj) is var blockInfo)
            {
                Origin = new DataIntVector3D(blockInfo.Origin.Coordinates);
                Extent = new DataIntVector3D(blockInfo.Extent.Coordinates);
                Size = new DataIntVector3D(blockInfo.Size.Coordinates);
                BlockGrouping = blockInfo.GetBlockGrouping().ToList();
                return this;
            }
            return null;
        }
    }
}
