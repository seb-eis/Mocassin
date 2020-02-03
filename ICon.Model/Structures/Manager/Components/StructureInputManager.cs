using Mocassin.Framework.Operations;
using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;
using Mocassin.Model.Structures.ConflictHandling;

namespace Mocassin.Model.Structures
{
    /// <summary>
    ///     Basic implementation of the structure input manager that handles validated adding, removal and replacement of
    ///     structure base data by an outside source
    /// </summary>
    internal class StructureInputManager : ModelInputManager<StructureModelData, IStructureDataPort, StructureEventManager>,
        IStructureInputPort
    {
        /// <inheritdoc />
        public StructureInputManager(StructureModelData modelData, StructureEventManager eventManager, IModelProject project)
            : base(modelData, eventManager, project)
        {
        }

        /// <summary>
        ///     Tries to register a new unit cell position in the manager if it passes validation (Awaits distribution of
        ///     affiliated events on operation success)
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        [DataOperation(DataOperationType.NewObject)]
        protected IOperationReport TryRegisterCellReferencePosition(ICellReferencePosition position)
        {
            return DefaultRegisterModelObject(position, accessor => accessor.Query(data => data.CellReferencePositions));
        }

        /// <summary>
        ///     Tries to remove a unit cell position from the manager by deprecation (Awaits distribution of affiliated events on
        ///     operation success)
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        [DataOperation(DataOperationType.ObjectRemoval)]
        protected IOperationReport TryRemoveCellReferencePosition(ICellReferencePosition position)
        {
            return DefaultRemoveModelObject(position, accessor => accessor.Query(data => data.CellReferencePositions));
        }

        /// <summary>
        ///     Tries to replace a unit cell position from the manager if it passes validation (Awaits distribution of affiliated
        ///     events on operation success)
        /// </summary>
        /// <param name="orgPosition"></param>
        /// <param name="newPosition"></param>
        /// <returns></returns>
        [DataOperation(DataOperationType.ObjectChange)]
        protected IOperationReport TryReplaceCellReferencePosition(ICellReferencePosition orgPosition, ICellReferencePosition newPosition)
        {
            return DefaultReplaceModelObject(orgPosition, newPosition, accessor => accessor.Query(data => data.CellReferencePositions));
        }

        /// <summary>
        ///     Tries to register a new unit cell position in the manager if it passes validation (Awaits distribution of
        ///     affiliated events on operation success)
        /// </summary>
        /// <param name="cellDummyPosition"></param>
        /// <returns></returns>
        [DataOperation(DataOperationType.NewObject)]
        protected IOperationReport TryRegisterPositionDummy(ICellDummyPosition cellDummyPosition)
        {
            return DefaultRegisterModelObject(cellDummyPosition, accessor => accessor.Query(data => data.PositionDummies));
        }

        /// <summary>
        ///     Tries to remove a unit cell position from the manager by deprecation (Awaits distribution of affiliated events on
        ///     operation success)
        /// </summary>
        /// <param name="cellDummyPosition"></param>
        /// <returns></returns>
        [DataOperation(DataOperationType.ObjectRemoval)]
        protected IOperationReport TryRemovePositionDummy(ICellDummyPosition cellDummyPosition)
        {
            return DefaultRemoveModelObject(cellDummyPosition, accessor => accessor.Query(data => data.PositionDummies));
        }

        /// <summary>
        ///     Tries to replace a unit cell position from the manager if it passes validation (Awaits distribution of affiliated
        ///     events on operation success)
        /// </summary>
        /// <param name="orgCellDummyPosition"></param>
        /// <param name="newCellDummyPosition"></param>
        /// <returns></returns>
        [DataOperation(DataOperationType.ObjectChange)]
        protected IOperationReport TryReplacePositionDummy(ICellDummyPosition orgCellDummyPosition, ICellDummyPosition newCellDummyPosition)
        {
            return DefaultReplaceModelObject(orgCellDummyPosition, newCellDummyPosition, accessor => accessor.Query(data => data.PositionDummies));
        }

        /// <summary>
        ///     tries to set new cell parameters if they pass validation (Awaits distribution of affiliated events on operation
        ///     success)
        /// </summary>
        /// <param name="cellParams"></param>
        /// <returns></returns>
        [DataOperation(DataOperationType.ParameterChange)]
        protected IOperationReport TrySetCellParameters(ICellParameters cellParams)
        {
            return DefaultSetModelParameter(cellParams, accessor => accessor.Query(data => data.CrystalParameters), true);
        }

        /// <summary>
        ///     Tries to set a new space group info if it passes validation and resolves potential conflicts (Distributes
        ///     affiliated events on operation success)
        /// </summary>
        /// <param name="groupInfo"></param>
        /// <returns></returns>
        [DataOperation(DataOperationType.ParameterChange)]
        protected IOperationReport TrySetSpaceGroup(ISpaceGroupInfo groupInfo)
        {
            return DefaultSetModelParameter(groupInfo, accessor => accessor.Query(data => data.SpaceGroupInfo), true);
        }

        /// <summary>
        ///     Tries to set a new structure info if it passes validation (Distributes affiliated events on operation success)
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [DataOperation(DataOperationType.ParameterChange)]
        protected IOperationReport TrySetStructureInfo(IStructureInfo info)
        {
            return DefaultSetModelParameter(info, accessor => accessor.Query(data => data.StructureInfo), false);
        }

        /// <inheritdoc />
        [DataOperation(DataOperationType.ObjectCleaning)]
        protected override IOperationReport TryCleanDeprecatedData()
        {
            return DefaultCleanDeprecatedData();
        }

        /// <inheritdoc />
        protected override IDataConflictHandlerProvider<StructureModelData> CreateDataConflictHandlerProvider()
        {
            return new StructureDataConflictHandlerProvider(ModelProject);
        }
    }
}