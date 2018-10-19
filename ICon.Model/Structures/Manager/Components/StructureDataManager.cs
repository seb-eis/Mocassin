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
        public ReadOnlyListAdapter<IUnitCellPosition> GetUnitCellPositions()
        {
            return ReadOnlyListAdapter<IUnitCellPosition>.FromEnumerable(Data.UnitCellPositions);
        }

        /// <inheritdoc />
        public ReindexingList GetCleanUnitCellPositionIndexing()
        {
            return CreateReindexing(Data.UnitCellPositions, Data.UnitCellPositions.Count);
        }

        /// <inheritdoc />
        public IUnitCellPosition GetUnitCellPosition(int index)
        {
            return Data.UnitCellPositions[index];
        }

        /// <inheritdoc />
        public ReadOnlyListAdapter<IPositionDummy> GetPositionDummies()
        {
            return ReadOnlyListAdapter<IPositionDummy>.FromEnumerable(Data.PositionDummies);
        }

        /// <inheritdoc />
        public IPositionDummy GetPositionDummy(int index)
        {
            return Data.PositionDummies[index];
        }
    }
}