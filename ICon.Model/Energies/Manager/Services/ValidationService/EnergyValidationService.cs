using System;
using ICon.Framework.Operations;
using ICon.Model.Basic;
using ICon.Model.Energies.Validators;
using ICon.Model.ProjectServices;

namespace ICon.Model.Energies
{
    /// <summary>
    ///     Validation service for energy related model objects that checks new energy related model object inputs
    /// </summary>
    public class EnergyValidationService : ValidationService<IEnergyDataPort>
    {
        /// <summary>
        ///     The basic energy settings object that defines all data constraints
        /// </summary>
        protected BasicEnergySettings Settings { get; set; }

        /// <inheritdoc />
        public EnergyValidationService(IProjectServices projectServices, BasicEnergySettings settings)
            : base(projectServices)
        {
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        /// <summary>
        ///     Validates a new stable group info in terms of content and potential conflicts with existing data that is accessed
        ///     through the provided dada reader
        /// </summary>
        /// <param name="groupInfo"></param>
        /// <param name="dataReader"></param>
        /// <returns></returns>
        [ValidationOperation(ValidationType.Object)]
        protected IValidationReport ValidateStableGroupInfo(IGroupInteraction groupInfo, IDataReader<IEnergyDataPort> dataReader)
        {
            return new GroupInteractionValidator(ProjectServices, Settings, dataReader).Validate(groupInfo);
        }

        /// <summary>
        ///     Validates a new unstable environment info in terms of content and potential conflicts with existing data that is
        ///     accessed through the provided data reader
        /// </summary>
        /// <param name="envInfo"></param>
        /// <param name="dataReader"></param>
        /// <returns></returns>
        [ValidationOperation(ValidationType.Object)]
        protected IValidationReport ValidateUnstableEnvironmentInfo(IUnstableEnvironment envInfo, IDataReader<IEnergyDataPort> dataReader)
        {
            return new UnstableEnvironmentValidator(ProjectServices, Settings, dataReader).Validate(envInfo);
        }

        /// <summary>
        ///     Validates the environment info parameter in terms of content and equality to the already existing data that is
        ///     accessed through the provided data reader
        /// </summary>
        /// <param name="envInfo"></param>
        /// <param name="dataReader"></param>
        /// <returns></returns>
        [ValidationOperation(ValidationType.Parameter)]
        protected IValidationReport ValidateEnvironmentInfo(IStableEnvironmentInfo envInfo, IDataReader<IEnergyDataPort> dataReader)
        {
            return new StableEnvironmentInfoValidator(ProjectServices, Settings, dataReader).Validate(envInfo);
        }
    }
}