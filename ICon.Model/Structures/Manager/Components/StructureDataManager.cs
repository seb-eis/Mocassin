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
        public ICellParameters GetCellParameters() => Data.CrystalParameters;

        /// <inheritdoc />
        public ISpaceGroupInfo GetSpaceGroupInfo() => Data.SpaceGroupInfo;

        /// <inheritdoc />
        public IStructureInfo GetStructureInfo() => Data.StructureInfo;

        /// <inheritdoc />
        public FixedList<ICellSite> GetCellReferencePositions() => FixedList<ICellSite>.FromEnumerable(Data.CellReferencePositions);

        /// <inheritdoc />
        public ReindexingList GetCleanReferencePositionIndexing() => CreateReindexing(Data.CellReferencePositions, Data.CellReferencePositions.Count);

        /// <inheritdoc />
        public ICellSite GetCellReferencePosition(int index) => Data.CellReferencePositions[index];

        /// <inheritdoc />
        public FixedList<ICellDummyPosition> GetDummyPositions() => FixedList<ICellDummyPosition>.FromEnumerable(Data.PositionDummies);

        /// <inheritdoc />
        public ICellDummyPosition GetDummyPosition(int index) => Data.PositionDummies[index];
    }
}