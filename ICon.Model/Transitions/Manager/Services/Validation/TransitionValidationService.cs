using System;

using ICon.Framework.Operations;
using ICon.Model.ProjectServices;
using ICon.Model.Basic;
using ICon.Model.Transitions.Validators;

namespace ICon.Model.Transitions
{
    /// <summary>
    /// Validation service for transition related model objects that checks new transition related model object inputs
    /// </summary>
    public class TransitionValidationService : ValidationService<ITransitionDataPort>
    {
        /// <summary>
        /// The settings object for the transitions
        /// </summary>
        protected BasicTransitionSettings Settings { get; set; }

        /// <summary>
        /// Create new transition validation service with the specififed project services and transition settings
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="projectServices"></param>
        public TransitionValidationService(BasicTransitionSettings settings, IProjectServices projectServices) : base(projectServices)
        {
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        /// <summary>
        /// Validates a new state exchange pair in terms of content and potentail conflicts with existing data
        /// </summary>
        /// <param name="pair"></param>
        /// <param name="dataReader"></param>
        /// <returns></returns>
        [ValidationOperation(ValidationType.Object)]
        protected IValidationReport ValidateStateExchangePair(IStateExchangePair pair, IDataReader<ITransitionDataPort> dataReader)
        {
            return new PropertyStatePairValidator(ProjectServices, Settings, dataReader).Validate(pair);
        }

        /// <summary>
        /// Validates a new state exchange group in terms of content and potential conflicts with existing data
        /// </summary>
        /// <param name="group"></param>
        /// <param name="dataReader"></param>
        /// <returns></returns>
        [ValidationOperation(ValidationType.Object)]
        protected IValidationReport ValidateStateExchangeGroupp(IStateExchangeGroup group, IDataReader<ITransitionDataPort> dataReader)
        {
            return new PropertyGroupValidator(ProjectServices, Settings, dataReader).Validate(group);
        }

        /// <summary>
        /// Validates a new abstract transition in terms of content and potential conflicts with existing data
        /// </summary>
        /// <param name="transition"></param>
        /// <param name="dataReader"></param>
        /// <returns></returns>
        [ValidationOperation(ValidationType.Object)]
        protected IValidationReport ValidateAbstractTransition(IAbstractTransition transition, IDataReader<ITransitionDataPort> dataReader)
        {
            return new AbstractTransitionValidator(ProjectServices, Settings, dataReader).Validate(transition);
        }

        /// <summary>
        /// Validates a new model transition in terms of content and potential conflicts with existing data
        /// </summary>
        /// <param name="transition"></param>
        /// <param name="dataReader"></param>
        /// <returns></returns>
        [ValidationOperation(ValidationType.Object)]
        protected IValidationReport ValidateKineticTransition(IKineticTransition transition, IDataReader<ITransitionDataPort> dataReader)
        {
            return new KineticTransitionValidator(ProjectServices, Settings, dataReader).Validate(transition);
        }

        /// <summary>
        /// Validates a new metropolis transition in terms of content and potential conflicts with existing data
        /// </summary>
        /// <param name="transition"></param>
        /// <param name="dataReader"></param>
        /// <returns></returns>
        [ValidationOperation(ValidationType.Object)]
        protected IValidationReport ValidateMetropolisTransition(IMetropolisTransition transition, IDataReader<ITransitionDataPort> dataReader)
        {
            return new MetropolisTransitionValidator(ProjectServices, Settings, dataReader).Validate(transition);
        }
    }
}
