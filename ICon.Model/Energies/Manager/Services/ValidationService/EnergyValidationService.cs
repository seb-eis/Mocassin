using System;
using Mocassin.Framework.Operations;
using Mocassin.Model.Basic;
using Mocassin.Model.Energies.Validators;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Energies
{
    /// <summary>
    ///     Validation service for energy related model objects that checks new energy related model object inputs
    /// </summary>
    public class EnergyValidationService : ValidationService<IEnergyDataPort>
    {
        /// <summary>
        ///     The basic energy settings object that defines all data constraints
        /// </summary>
        protected MocassinEnergySettings Settings { get; set; }

        /// <inheritdoc />
        public EnergyValidationService(IModelProject modelProject, MocassinEnergySettings settings)
            : base(modelProject)
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
            return new GroupInteractionValidator(ModelProject, Settings, dataReader).Validate(groupInfo);
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
            return new UnstableEnvironmentValidator(ModelProject, Settings, dataReader).Validate(envInfo);
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
            return new StableEnvironmentInfoValidator(ModelProject, Settings, dataReader).Validate(envInfo);
        }
    }
}