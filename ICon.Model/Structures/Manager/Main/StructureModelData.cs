using System.Collections.Generic;
using System.Runtime.Serialization;
using Mocassin.Model.Basic;
using Mocassin.Symmetry.CrystalSystems;
using Mocassin.Symmetry.SpaceGroups;

namespace Mocassin.Model.Structures
{
    /// <summary>
    ///     Basic structure manager reference data that represents the base data required for load/save actions and caluclation
    ///     of all dependent data
    /// </summary>
    [DataContract(Name = "StructureBaseData")]
    public class StructureModelData : ModelData<IStructureDataPort>
    {
        /// <summary>
        ///     The name of the structure
        /// </summary>
        [DataMember]
        [ModelParameter(typeof(IStructureInfo))]
        public StructureInfo StructureInfo { get; set; }

        /// <summary>
        ///     The space group information
        /// </summary>
        [DataMember]
        [ModelParameter(typeof(ISpaceGroupInfo))]
        public SpaceGroupInfo SpaceGroupInfo { get; set; }

        /// <summary>
        ///     The crystal parameter set
        /// </summary>
        [DataMember]
        [ModelParameter(typeof(ICellParameters))]
        public CellParameters CrystalParameters { get; set; }

        /// <summary>
        ///     The list of reference unit cell positions
        /// </summary>
        [DataMember]
        [IndexedModelData(typeof(ICellSite))]
        public List<CellSite> CellReferencePositions { get; set; }

        /// <summary>
        ///     The list of reference unit cell position dummies
        /// </summary>
        [DataMember]
        [IndexedModelData(typeof(ICellDummyPosition))]
        public List<CellDummyPosition> PositionDummies { get; set; }

        /// <inheritdoc />
        public override IStructureDataPort AsReadOnly()
        {
            return new StructureDataManager(this);
        }

        /// <inheritdoc />
        public override void ResetToDefault()
        {
            StructureInfo = StructureInfo.CreateDefault();
            SpaceGroupInfo = SpaceGroupEntry.CreateDefault();
            CrystalParameters = CrystalParameterSet.CreateDefault();
            ResetAllIndexedData();
        }

        /// <summary>
        ///     Creates a new default structure model data object
        /// </summary>
        /// <returns></returns>
        public static StructureModelData CreateNew()
        {
            return CreateDefault<StructureModelData>();
        }
    }
}