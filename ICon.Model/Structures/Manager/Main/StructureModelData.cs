﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

using ICon.Model.Basic;
using ICon.Symmetry.SpaceGroups;
using ICon.Symmetry.CrystalSystems;

namespace ICon.Model.Structures
{
    /// <summary>
    /// Basic structure manager reference data that represents the base data required for load/save actions and caluclation of all dependent data
    /// </summary>
    [DataContract(Name = "StructureBaseData")]
    public class StructureModelData : ModelData<IStructureDataPort>
    {
        /// <summary>
        /// The name of the structure
        /// </summary>
        [DataMember]
        [ModelParameter(typeof(IStructureInfo))]
        public StructureInfo StructureInfo { get; set; }

        /// <summary>
        /// The space group information
        /// </summary>
        [DataMember]
        [ModelParameter(typeof(ISpaceGroupInfo))]
        public SpaceGroupInfo SpaceGroupInfo { get; set; }

        /// <summary>
        /// The crystal parameter set
        /// </summary>
        [DataMember]
        [ModelParameter(typeof(ICellParameters))]
        public CellParameters CrystalParameters { get; set; }

        /// <summary>
        /// The list of reference unit cell positions
        /// </summary>
        [DataMember]
        [IndexedModelData(typeof(IUnitCellPosition))]
        public List<UnitCellPosition> UnitCellPositions { get; set; }

        /// <summary>
        /// The list of reference unit cell position dummies
        /// </summary>
        [DataMember]
        [IndexedModelData(typeof(IPositionDummy))]
        public List<PositionDummy> PositionDummies { get; set; }

        /// <summary>
        /// Creates a read only data port for this data object
        /// </summary>
        /// <returns></returns>
        public override IStructureDataPort AsReadOnly()
        {
            return new StructureDataManager(this);
        }

        /// <summary>
        /// Resets the structure model data to default conditions
        /// </summary>
        public override void ResetToDefault()
        {
            StructureInfo = StructureInfo.CreateDefault();
            SpaceGroupInfo = SpaceGroupEntry.CreateDefault();
            CrystalParameters = CrystalParameterSet.CreateDefault();
            ResetAllIndexedData();
        }

        /// <summary>
        /// Creates a new default structure model data object
        /// </summary>
        /// <returns></returns>
        public static StructureModelData CreateNew()
        {
            return CreateDefault<StructureModelData>();
        }
    }
}
