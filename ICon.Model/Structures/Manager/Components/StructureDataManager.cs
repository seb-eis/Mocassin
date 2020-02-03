using Mocassin.Framework.Collections;
using Mocassin.Model.Basic;

namespace Mocassin.Model.Structures
{
    /// <summary>
    ///     Structure data manager that provides safe read only access to the structure base data
    /// </summary>
    internal class StructureDataManager : ModelDataManager<StructureModelData>, IStructureDataPort
    {
        /// <inheritdoc />
        public StructureDataManager(StructureModelData modelData)
            : base(modelData)
        {
        }

        /// <inheritdoc />
        public ICellParameters GetCellParameters()
        {
            return Data.CrystalParameters;
        }

        /// <inheritdoc />
        public ISpaceGroupInfo GetSpaceGroupInfo()
        {
            return Data.SpaceGroupInfo;
        }

        /// <inheritdoc />
        public IStructureInfo GetStructureInfo()
        {
            return Data.StructureInfo;
        }

        /// <inheritdoc />
        public ListReadOnlyWrapper<ICellReferencePosition> GetCellReferencePositions()
        {
            return ListReadOnlyWrapper<ICellReferencePosition>.FromEnumerable(Data.CellReferencePositions);
        }

        /// <inheritdoc />
        public ReindexingList GetCleanReferencePositionIndexing()
        {
            return CreateReindexing(Data.CellReferencePositions, Data.CellReferencePositions.Count);
        }

        /// <inheritdoc />
        public ICellReferencePosition GetCellReferencePosition(int index)
        {
            return Data.CellReferencePositions[index];
        }

        /// <inheritdoc />
        public ListReadOnlyWrapper<ICellDummyPosition> GetDummyPositions()
        {
            return ListReadOnlyWrapper<ICellDummyPosition>.FromEnumerable(Data.PositionDummies);
        }

        /// <inheritdoc />
        public ICellDummyPosition GetDummyPosition(int index)
        {
            return Data.PositionDummies[index];
        }
    }
}