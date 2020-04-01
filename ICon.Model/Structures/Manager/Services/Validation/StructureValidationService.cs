using System;
using Mocassin.Framework.Operations;
using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;
using Mocassin.Model.Structures.Validators;

namespace Mocassin.Model.Structures
{
    /// <summary>
    ///     Validation service for structure related model objects, uses space group service to validate potential duplicate
    ///     conflicts
    /// </summary>
    public class StructureValidationService : ValidationService<IStructureDataPort>
    {
        /// <summary>
        ///     The basic structure settings object
        /// </summary>
        private MocassinStructureSettings Settings { get; }

        /// <inheritdoc />
        public StructureValidationService(MocassinStructureSettings settings, IModelProject modelProject)
            : base(modelProject)
        {
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        /// <summary>
        ///     Validates a new unit cell position in terms of conflicts with basic limitations and existing data
        /// </summary>
        /// <param name="position"></param>
        /// <param name="dataReader"></param>
        /// <returns></returns>
        [ValidationOperation(ValidationType.Object)]
        protected IValidationReport ValidateCellReferencePosition(ICellSite position, IDataReader<IStructureDataPort> dataReader)
        {
            return new CellReferencePositionValidator(ModelProject, Settings, dataReader).Validate(position);
        }

        /// <summary>
        ///     Validates a position dummy in terms of conflicts with basic limitations
        /// </summary>
        /// <param name="cellDummyPosition"></param>
        /// <param name="dataReader"></param>
        /// <returns></returns>
        [ValidationOperation(ValidationType.Object)]
        protected IValidationReport ValidatePositionDummy(ICellDummyPosition cellDummyPosition, IDataReader<IStructureDataPort> dataReader)
        {
            return new PositionDummyValidator(ModelProject, Settings, dataReader).Validate(cellDummyPosition);
        }

        /// <summary>
        ///     Validate a structure info in terms of conflicts with basic limitations
        /// </summary>
        /// <param name="info"></param>
        /// <param name="dataReader"></param>
        /// <returns></returns>
        [ValidationOperation(ValidationType.Parameter)]
        protected IValidationReport ValidateStructureInfo(IStructureInfo info, IDataReader<IStructureDataPort> dataReader)
        {
            return new StructureInfoValidator(ModelProject, Settings, dataReader).Validate(info);
        }

        /// <summary>
        ///     Validates a set of cell parameters in terms of conflicts with basic limitations and the currently active space
        ///     group
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="dataReader"></param>
        /// <returns></returns>
        [ValidationOperation(ValidationType.Parameter)]
        protected IValidationReport ValidateCellParameters(ICellParameters parameters, IDataReader<IStructureDataPort> dataReader)
        {
            return new UnitCellParameterValidator(ModelProject, Settings, dataReader).Validate(parameters);
        }

        /// <summary>
        ///     Validates a space group info in terms of conflicts with basic limitations (Does not load group into the service!)
        /// </summary>
        /// <param name="groupInfo"></param>
        /// <param name="dataReader"></param>
        /// <returns></returns>
        [ValidationOperation(ValidationType.Parameter)]
        protected IValidationReport ValidateSpaceGroupInfo(ISpaceGroupInfo groupInfo, IDataReader<IStructureDataPort> dataReader)
        {
            return new SpaceGroupInfoValidator(ModelProject, Settings, dataReader).Validate(groupInfo);
        }
    }
}