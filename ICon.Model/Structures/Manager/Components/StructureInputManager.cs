using System;
using System.Collections.Generic;
using ICon.Framework.Operations;
using ICon.Framework.Processing;
using ICon.Model.Basic;
using ICon.Model.ProjectServices;

namespace ICon.Model.Structures
{
    /// <summary>
    /// Basic implementation of the structure input manager that handles validated adding, removal and replacement of structure base data by an outside source
    /// </summary>
    internal class StructureInputManager : ModelInputManager<StructureModelData, IStructureDataPort, StructureEventManager>, IStructureInputPort
    {
        /// <summary>
        /// Creates new structre input manager for the provided data object, event manager and project services
        /// </summary>
        /// <param name="data"></param>
        /// <param name="events"></param>
        /// <param name="services"></param>
        public StructureInputManager(StructureModelData data, StructureEventManager events, IProjectServices services) : base(data, events, services)
        {

        }

        /// <summary>
        /// Tries to register a new unit cell position in the manager if it passes validation (Awaits distribution of affiliated events on operation success)
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        [DataOperation(DataOperationType.NewObject)]
        protected IOperationReport TryRegisterUnitCellPosition(IUnitCellPosition position)
        {
            return DefaultRegisterModelObject(position, accessor => accessor.Query(data => data.UnitCellPositions));
        }

        /// <summary>
        /// Tries to remove a unit cell position from the manager by deprecation (Awaits distribution of affiliated events on operation success)
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        [DataOperation(DataOperationType.ObjectRemoval)]
        protected IOperationReport TryRemoveUnitCellPosition(IUnitCellPosition position)
        {
            return DefaultRemoveModelObject(position, accessor => accessor.Query(data => data.UnitCellPositions));
        }

        /// <summary>
        /// Tries to replace a unit cell position from the manager if it passes validation (Awaits distribution of affiliated events on operation success)
        /// </summary>
        /// <param name="orgPosition"></param>
        /// <param name="newPosition"></param>
        /// <returns></returns>
        [DataOperation(DataOperationType.ObjectChange)]
        protected IOperationReport TryReplaceUnitCellPosition(IUnitCellPosition orgPosition, IUnitCellPosition newPosition)
        {
            return DefaultReplaceModelObject(orgPosition, newPosition, accessor => accessor.Query(data => data.UnitCellPositions));
        }

        /// <summary>
        /// Tries to register a new unit cell position in the manager if it passes validation (Awaits distribution of affiliated events on operation success)
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        [DataOperation(DataOperationType.NewObject)]
        protected IOperationReport TryRegisterPositionDummy(IPositionDummy position)
        {
            return DefaultRegisterModelObject(position, accessor => accessor.Query(data => data.PositionDummies));
        }

        /// <summary>
        /// Tries to remove a unit cell position from the manager by deprecation (Awaits distribution of affiliated events on operation success)
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        [DataOperation(DataOperationType.ObjectRemoval)]
        protected IOperationReport TryRemovePositionDummy(IPositionDummy position)
        {
            return DefaultRemoveModelObject(position, accessor => accessor.Query(data => data.PositionDummies));
        }

        /// <summary>
        /// Tries to replace a unit cell position from the manager if it passes validation (Awaits distribution of affiliated events on operation success)
        /// </summary>
        /// <param name="orgPosition"></param>
        /// <param name="newPosition"></param>
        /// <returns></returns>
        [DataOperation(DataOperationType.ObjectChange)]
        protected IOperationReport TryReplacePositionDummy(IPositionDummy orgPosition, IPositionDummy newPosition)
        {
            return DefaultReplaceModelObject(orgPosition, newPosition, accessor => accessor.Query(data => data.PositionDummies));
        }

        /// <summary>
        /// tries to set new cell parameters if they pass validation (Awaits distribution of affiliated events on operation success)
        /// </summary>
        /// <param name="cellParams"></param>
        /// <returns></returns>
        [DataOperation(DataOperationType.ParameterChange)]
        protected IOperationReport TrySetCellParameters(ICellParameters cellParams)
        {
            return DefaultSetModelParameter(cellParams, accessor => accessor.Query(data => data.CrystalParameters), true);
        }

        /// <summary>
        /// Tries to set a new space group info if it passes validation and resolves potential conflicts (Distributes affiliated events on operation success)
        /// </summary>
        /// <param name="groupInfo"></param>
        /// <returns></returns>
        [DataOperation(DataOperationType.ParameterChange)]
        protected IOperationReport TrySetSpaceGroup(ISpaceGroupInfo groupInfo)
        {
            return DefaultSetModelParameter(groupInfo, accessor => accessor.Query(data => data.SpaceGroupInfo), true);
        }

        /// <summary>
        /// Tries to set a new structure info if it passes validation (Distributes affiliated events on operation success)
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [DataOperation(DataOperationType.ParameterChange)]
        protected IOperationReport TrySetStructureInfo(IStructureInfo info)
        {
            return DefaultSetModelParameter(info, accessor => accessor.Query(data => data.StructureInfo), false);
        }

        /// <summary>
        /// Tries to clean deprecated model objects from the manager (Distributes reindexing events on operation succes)
        /// </summary>
        /// <returns></returns>
        [DataOperation(DataOperationType.ObjectCleaning)]
        protected override IOperationReport TryCleanDeprecatedData()
        {
            return DefaultCleanDeprecatedData();
        }

        /// <summary>
        /// Overrides the default empty conflict resolver by a custom resolver for internal structure conflicts
        /// </summary>
        /// <returns></returns>
        protected override IDataConflictHandlerProvider<StructureModelData> MakeConflictHandlerProvider()
        {
            return new ConflictHandling.StructureDataConflictHandlerProvider(ProjectServices);
        }
    }
}
