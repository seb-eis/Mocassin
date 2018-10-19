using System;
using Mocassin.Framework.Operations;
using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;
using Mocassin.Model.Transitions.Validators;

namespace Mocassin.Model.Transitions
{
    /// <summary>
    ///     Validation service for transition related model objects that checks new transition related model object inputs
    /// </summary>
    public class TransitionValidationService : ValidationService<ITransitionDataPort>
    {
        /// <summary>
        ///     The settings object for the transitions
        /// </summary>
        protected MocassinTransitionSettings Settings { get; set; }

        /// <inheritdoc />
        public TransitionValidationService(MocassinTransitionSettings settings, IModelProject modelProject)
            : base(modelProject)
        {
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        /// <summary>
        ///     Validates a new state exchange pair in terms of content and potential conflicts with existing data
        /// </summary>
        /// <param name="pair"></param>
        /// <param name="dataReader"></param>
        /// <returns></returns>
        [ValidationOperation(ValidationType.Object)]
        protected IValidationReport ValidateStateExchangePair(IStateExchangePair pair, IDataReader<ITransitionDataPort> dataReader)
        {
            return new PropertyStatePairValidator(ModelProject, Settings, dataReader).Validate(pair);
        }

        /// <summary>
        ///     Validates a new state exchange group in terms of content and potential conflicts with existing data
        /// </summary>
        /// <param name="group"></param>
        /// <param name="dataReader"></param>
        /// <returns></returns>
        [ValidationOperation(ValidationType.Object)]
        protected IValidationReport ValidateStateExchangeGroup(IStateExchangeGroup group, IDataReader<ITransitionDataPort> dataReader)
        {
            return new PropertyGroupValidator(ModelProject, Settings, dataReader).Validate(group);
        }

        /// <summary>
        ///     Validates a new abstract transition in terms of content and potential conflicts with existing data
        /// </summary>
        /// <param name="transition"></param>
        /// <param name="dataReader"></param>
        /// <returns></returns>
        [ValidationOperation(ValidationType.Object)]
        protected IValidationReport ValidateAbstractTransition(IAbstractTransition transition, IDataReader<ITransitionDataPort> dataReader)
        {
            return new AbstractTransitionValidator(ModelProject, Settings, dataReader).Validate(transition);
        }

        /// <summary>
        ///     Validates a new model transition in terms of content and potential conflicts with existing data
        /// </summary>
        /// <param name="transition"></param>
        /// <param name="dataReader"></param>
        /// <returns></returns>
        [ValidationOperation(ValidationType.Object)]
        protected IValidationReport ValidateKineticTransition(IKineticTransition transition, IDataReader<ITransitionDataPort> dataReader)
        {
            return new KineticTransitionValidator(ModelProject, Settings, dataReader).Validate(transition);
        }

        /// <summary>
        ///     Validates a new metropolis transition in terms of content and potential conflicts with existing data
        /// </summary>
        /// <param name="transition"></param>
        /// <param name="dataReader"></param>
        /// <returns></returns>
        [ValidationOperation(ValidationType.Object)]
        protected IValidationReport ValidateMetropolisTransition(IMetropolisTransition transition,
            IDataReader<ITransitionDataPort> dataReader)
        {
            return new MetropolisTransitionValidator(ModelProject, Settings, dataReader).Validate(transition);
        }
    }
}