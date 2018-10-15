using System;
using System.Collections.Generic;
using System.Text;
using ICon.Framework.Collections;
using ICon.Model.Basic;

namespace ICon.Model.Structures
{
    /// <summary>
    /// Structure data manager that provides safe read only access to the structure base data
    /// </summary>
    internal class StructureDataManager : ModelDataManager<StructureModelData>, IStructureDataPort
    {
        /// <summary>
        /// Creates new structure data manager from base data object
        /// </summary>
        /// <param name="data"></param>
        public StructureDataManager(StructureModelData data) :  base(data)
        {

        }

        /// <summary>
        /// Get the unit cell parameters as read only interface
        /// </summary>
        /// <returns></returns>
        public ICellParameters GetCellParameters()
        {
            return Data.CrystalParameters;
        }

        /// <summary>
        /// Get the space group info as read only interface
        /// </summary>
        /// <returns></returns>
        public ISpaceGroupInfo GetSpaceGroupInfo()
        {
            return Data.SpaceGroupInfo;
        }

        /// <summary>
        /// Get the structure info as read only interface
        /// </summary>
        /// <returns></returns>
        public IStructureInfo GetStructureInfo()
        {
            return Data.StructureInfo;
        }

        /// <summary>
        /// Get read only access to the unit cell position list
        /// </summary>
        /// <returns></returns>
        public ReadOnlyListAdapter<IUnitCellPosition> GetUnitCellPositions()
        {
            return ReadOnlyListAdapter<IUnitCellPosition>.FromEnumerable(Data.UnitCellPositions);
        }

        /// <summary>
        /// Get a cleaned unit cell position indexing set where deprecated data is removed
        /// </summary>
        /// <returns></returns>
        public ReindexingList GetCleanUnitCellPositionIndexing()
        {
            return CreateReindexing(Data.UnitCellPositions, Data.UnitCellPositions.Count);
        }

        /// <summary>
        /// Get a unit cell position by index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public IUnitCellPosition GetUnitCellPosition(int index)
        {
            return Data.UnitCellPositions[index];
        }

        /// <summary>
        /// Get a aread only list of all position dummies
        /// </summary>
        /// <returns></returns>
        public ReadOnlyListAdapter<IPositionDummy> GetPositionDummies()
        {
            return ReadOnlyListAdapter<IPositionDummy>.FromEnumerable(Data.PositionDummies);
        }

        /// <summary>
        /// Get the position dummy at the specfified index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public IPositionDummy GetPositionDummy(int index)
        {
            return Data.PositionDummies[index];
        }
    }
}
