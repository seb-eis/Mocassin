using Mocassin.Framework.Collections;
using Mocassin.Model.Basic;

namespace Mocassin.Model.Structures
{
    /// <summary>
    ///     Represents a read only data port for the structure manager that allows data access on the structure manager base
    ///     data through interfaces
    /// </summary>
    public interface IStructureDataPort : IModelDataPort
    {
        /// <summary>
        ///     Get the current space group information
        /// </summary>
        /// <returns></returns>
        ISpaceGroupInfo GetSpaceGroupInfo();

        /// <summary>
        ///     Get the unit cell parameters
        /// </summary>
        /// <returns></returns>
        ICellParameters GetCellParameters();

        /// <summary>
        ///     Get the structure miscellaneous info
        /// </summary>
        /// <returns></returns>
        IStructureInfo GetStructureInfo();

        /// <summary>
        ///     Get read only access to the unit cell position list
        /// </summary>
        /// <returns></returns>
        FixedList<ICellSite> GetCellReferencePositions();

        /// <summary>
        ///     Get a unit cell position by index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        ICellSite GetCellReferencePosition(int index);

        /// <summary>
        ///     Get a read only list of all position dummies
        /// </summary>
        /// <returns></returns>
        FixedList<ICellDummyPosition> GetDummyPositions();

        /// <summary>
        ///     Get the position dummy at the specified index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        ICellDummyPosition GetDummyPosition(int index);

        /// <summary>
        ///     Get a cleaned indexing for the unit cell positions (Where deprecated data is removed)
        /// </summary>
        /// <returns></returns>
        ReindexingList GetCleanReferencePositionIndexing();
    }
}