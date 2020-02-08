using System;
using Mocassin.Framework.Operations;
using Mocassin.Model.Basic;
using Mocassin.Model.Lattices.Validators;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Lattices
{
    /// <summary>
    ///     Validation service for lattice related model objects that checks new lattice related model object inputs
    /// </summary>
    public class LatticeValidationService : ValidationService<ILatticeDataPort>
    {
        /// <summary>
        ///     The basic Lattice settings object that defines all data constraints
        /// </summary>
        protected MocassinLatticeSettings Settings { get; set; }

        /// <summary>
        ///     Create new Lattice validation service that uses the provided project service and settings object
        /// </summary>
        /// <param name="modelProject"></param>
        /// <param name="settings"></param>
        public LatticeValidationService(IModelProject modelProject, MocassinLatticeSettings settings)
            : base(modelProject)
        {
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        /// <summary>
        ///     Validate a BuildingBlock in terms of conflicts with basic limitations
        /// </summary>
        /// <param name="buildingBlock"></param>
        /// <param name="dataReader"></param>
        /// <returns></returns>
        [ValidationOperation(ValidationType.Object)]
        protected IValidationReport ValidateBuildingBlock(IBuildingBlock buildingBlock, IDataReader<ILatticeDataPort> dataReader)
        {
            return new BuildingBlockValidator(ModelProject, Settings, dataReader).Validate(buildingBlock);
        }


        /// <summary>
        ///     Validate a DopingCombination in terms of conflicts with basic limitations and ParticleSets of cell reference
        ///     Position
        /// </summary>
        /// <param name="dopingCombination"></param>
        /// <param name="dataReader"></param>
        /// <returns></returns>
        [ValidationOperation(ValidationType.Object)]
        protected IValidationReport ValidateDopingCombination(IDopingCombination dopingCombination, IDataReader<ILatticeDataPort> dataReader)
        {
            return new DopingCombinationValidator(ModelProject, Settings, dataReader).Validate(dopingCombination);
        }

        /// <summary>
        ///     Validate a Doping in terms of conflicts with basic limitations
        /// </summary>
        /// <param name="doping"></param>
        /// <param name="dataReader"></param>
        /// <returns></returns>
        [ValidationOperation(ValidationType.Object)]
        protected IValidationReport ValidateDoping(IDoping doping, IDataReader<ILatticeDataPort> dataReader)
        {
            return new DopingValidator(ModelProject, Settings, dataReader).Validate(doping);
        }
    }
}