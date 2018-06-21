using System;
using ICon.Framework.Operations;

using ICon.Model.ProjectServices;
using ICon.Model.Basic;
using ICon.Model.Lattices.Validators;

namespace ICon.Model.Lattices
{
    /// <summary>
    /// Validation service for lattice related model objects that checks new lattice related model object inputs
    /// </summary>
    public class LatticeValidationService : ValidationService<ILatticeDataPort>
    {
        /// <summary>
        /// The basic Lattice settings object that defines all data constraints
        /// </summary>
        protected BasicLatticeSettings Settings { get; set; }

        /// <summary>
        /// Create new Lattice validation service that uses the provided project service and settings object
        /// </summary>
        /// <param name="projectServices"></param>
        /// <param name="settings"></param>
        public LatticeValidationService(IProjectServices projectServices, BasicLatticeSettings settings) : base(projectServices)
        {
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        /// <summary>
        /// Validate a lattice info in terms of conflicts with basic limitations
        /// </summary>
        /// <param name="info"></param>
        /// <param name="dataReader"></param>
        /// <returns></returns>
        [ValidationMethod(ValidationType.Parameter)]
        protected IValidationReport ValidateStructureInfo(ILatticeInfo info, IDataReader<ILatticeDataPort> dataReader)
        {
            return new LatticeInfoValidator(ProjectServices, Settings, dataReader).Validate(info);
        }

        /// <summary>
        /// Validate a BuildingBlock in terms of conflicts with basic limitations
        /// </summary>
        /// <param name="buildingBlock"></param>
        /// <param name="dataReader"></param>
        /// <returns></returns>
        [ValidationMethod(ValidationType.Object)]
        protected IValidationReport ValidateBuildingBlock(IBuildingBlock buildingBlock, IDataReader<ILatticeDataPort> dataReader)
        {
            return new BuildingBlockValidator(ProjectServices, Settings, dataReader).Validate(buildingBlock);
        }

        /// <summary>
        /// Validate a BlockInfo in terms of conflicts with basic limitations
        /// </summary>
        /// <param name="blockInfo"></param>
        /// <param name="dataReader"></param>
        /// <returns></returns>
        [ValidationMethod(ValidationType.Object)]
        protected IValidationReport ValidateBlockInfo(IBlockInfo blockInfo, IDataReader<ILatticeDataPort> dataReader)
        {
            return new BlockInfoValidator(ProjectServices, Settings, dataReader).Validate(blockInfo);
        }

        /// <summary>
        /// Validate a DopingCombination in terms of conflicts with basic limitations and ParticleSets of UnitcellPosition
        /// </summary>
        /// <param name="doping"></param>
        /// <param name="dataReader"></param>
        /// <returns></returns>
        [ValidationMethod(ValidationType.Object)]
        protected IValidationReport ValidateDopingCombination(IDopingCombination dopingCombination, IDataReader<ILatticeDataPort> dataReader)
        {
            return new DopingCombinationValidator(ProjectServices, Settings, dataReader).Validate(dopingCombination);
        }

        /// <summary>
        /// Validate a Doping in terms of conflicts with basic limitations
        /// </summary>
        /// <param name="doping"></param>
        /// <param name="dataReader"></param>
        /// <returns></returns>
        [ValidationMethod(ValidationType.Object)]
        protected IValidationReport ValidateDoping(IDoping doping, IDataReader<ILatticeDataPort> dataReader)
        {
            return new DopingValidator(ProjectServices, Settings, dataReader).Validate(doping);
        }
    }
}
