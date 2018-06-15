using System;
using System.Collections.Generic;
using System.Text;

using ICon.Framework.Collections;
using ICon.Model.Basic;

namespace ICon.Model.Structures
{
    /// <summary>
    /// Represents a read only data port for the structre manager that allows data access on the structure manager base data through interfaces
    /// </summary>
    public interface IStructureDataPort : IModelDataPort
    {
        /// <summary>
        /// Get the current space group information
        /// </summary>
        /// <returns></returns>
        ISpaceGroupInfo GetSpaceGroupInfo();

        /// <summary>
        /// Get the unit cell parameters
        /// </summary>
        /// <returns></returns>
        ICellParameters GetCellParameters();

        /// <summary>
        /// Get the structure miscellaneous info
        /// </summary>
        /// <returns></returns>
        IStructureInfo GetStructureInfo();

        /// <summary>
        /// Get read only access to the unit cell position list
        /// </summary>
        /// <returns></returns>
        ReadOnlyList<IUnitCellPosition> GetUnitCellPositions();

        /// <summary>
        /// Get a unit cell position by index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        IUnitCellPosition GetUnitCellPosition(int index);

        /// <summary>
        /// Get a read only list of all position dummies
        /// </summary>
        /// <returns></returns>
        ReadOnlyList<IPositionDummy> GetPositionDummies();

        /// <summary>
        /// Get the position dummy at the specfified index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        IPositionDummy GetPositionDummy(int index);

        /// <summary>
        /// Get a cleaned indexing for the unit cell positions (Where deprecated data is removed)
        /// </summary>
        /// <returns></returns>
        ReindexingList GetCleanUnitCellPositionIndexing();
    }
}
