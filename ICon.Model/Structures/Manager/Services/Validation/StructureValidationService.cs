using System;
using ICon.Framework.Operations;
using ICon.Model.ProjectServices;
using ICon.Model.Basic;
using ICon.Model.Structures.Validators;

namespace ICon.Model.Structures
{
    /// <summary>
    /// Validation service for structure related model objects, uses space group service to validate potential duplicate conflicts
    /// </summary>
    public class StructureValidationService : ValidationService<IStructureDataPort>
    {
        /// <summary>
        /// The basic structure settings object
        /// </summary>
        private BasicStructureSettings Settings { get; set; }

        /// <summary>
        /// Create new structure validation service for the provided basic settings and space group service
        /// </summary>
        /// <param name="settings"></param>
        public StructureValidationService(BasicStructureSettings settings, IProjectServices projectServices) : base(projectServices)
        {
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        /// <summary>
        /// Validates a new unit cell position in terms of conflicts with basic limitations and existing data
        /// </summary>
        /// <param name="position"></param>
        /// <param name="dataReader"></param>
        /// <returns></returns>
        [ValidationMethod(ValidationType.Object)]
        protected IValidationReport ValidateUnitCellPosition(IUnitCellPosition position, IDataReader<IStructureDataPort> dataReader)
        {
            return new UnitCellPositionValidator(ProjectServices, Settings, dataReader).Validate(position);
        }

        /// <summary>
        /// Validates a position dummy in terms of conflicts with basic limitations
        /// </summary>
        /// <param name="position"></param>
        /// <param name="dataReader"></param>
        /// <returns></returns>
        [ValidationMethod(ValidationType.Object)]
        protected IValidationReport ValidatePositionDummy(IPositionDummy position, IDataReader<IStructureDataPort> dataReader)
        {
            return new PositionDummyValidator(ProjectServices, Settings, dataReader).Validate(position);
        }

        /// <summary>
        /// Validate a structure info in terms of conflicts with basic limitations
        /// </summary>
        /// <param name="info"></param>
        /// <param name="dataReader"></param>
        /// <returns></returns>
        [ValidationMethod(ValidationType.Parameter)]
        protected IValidationReport ValidateStructureInfo(IStructureInfo info, IDataReader<IStructureDataPort> dataReader)
        {
            return new StructureInfoValidator(ProjectServices, Settings, dataReader).Validate(info);
        }

        /// <summary>
        /// Validates a set of cell parameters in terms of conflicts with basic limitations and the currently active space group
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="dataReader"></param>
        /// <returns></returns>
        [ValidationMethod(ValidationType.Parameter)]
        protected IValidationReport ValidateCellParameters(ICellParameters parameters, IDataReader<IStructureDataPort> dataReader)
        {
            return new UnitCellParameterValidator(ProjectServices, Settings, dataReader).Validate(parameters);
        }

        /// <summary>
        /// Validates a space group info in terms of conflicts with basic limitations (Does not load group into the service!)
        /// </summary>
        /// <param name="groupInfo"></param>
        /// <param name="dataReader"></param>
        /// <returns></returns>
        [ValidationMethod(ValidationType.Parameter)]
        protected IValidationReport ValidateSpaceGroupInfo(ISpaceGroupInfo groupInfo, IDataReader<IStructureDataPort> dataReader)
        {
            return new SpaceGroupInfoValidator(ProjectServices, Settings, dataReader).Validate(groupInfo);
        }
    }
}
