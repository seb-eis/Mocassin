using System;
using System.Linq;
using Mocassin.Framework.Collections;
using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Transitions
{
    /// <summary>
    ///     Transition data manager that provides safe read only access to the transition base data
    /// </summary>
    internal class TransitionDataManager : ModelDataManager<TransitionModelData>, ITransitionDataPort
    {
        /// <inheritdoc />
        public TransitionDataManager(TransitionModelData modelData)
            : base(modelData)
        {
        }

        /// <inheritdoc />
        public IAbstractTransition GetAbstractTransition(int index)
        {
            return Data.AbstractTransitions[index];
        }

        /// <inheritdoc />
        public ListReadOnlyWrapper<IAbstractTransition> GetAbstractTransitions()
        {
            return ListReadOnlyWrapper<IAbstractTransition>.FromEnumerable(Data.AbstractTransitions);
        }

        /// <inheritdoc />
        public int GetKineticTransitionCount()
        {
            return Data.KineticTransitions.Count(a => !a.IsDeprecated);
        }

        /// <inheritdoc />
        public ListReadOnlyWrapper<IKineticTransition> GetKineticTransitions()
        {
            return ListReadOnlyWrapper<IKineticTransition>.FromEnumerable(Data.KineticTransitions);
        }

        /// <inheritdoc />
        public ListReadOnlyWrapper<IMetropolisTransition> GetMetropolisTransitions()
        {
            return ListReadOnlyWrapper<IMetropolisTransition>.FromEnumerable(Data.MetropolisTransitions);
        }

        /// <inheritdoc />
        public int GetMetropolisTransitionCount()
        {
            return Data.MetropolisTransitions.Count(a => !a.IsDeprecated);
        }

        /// <inheritdoc />
        public IStateExchangeGroup GetStateExchangeGroup(int index)
        {
            return Data.StateExchangeGroups[index];
        }

        /// <summary>
        ///     Get a read only list of all state exchange groups
        /// </summary>
        /// <returns></returns>
        public ListReadOnlyWrapper<IStateExchangeGroup> GetStateExchangeGroups()
        {
            return ListReadOnlyWrapper<IStateExchangeGroup>.FromEnumerable(Data.StateExchangeGroups);
        }

        /// <inheritdoc />
        public ListReadOnlyWrapper<IStateExchangePair> GetStateExchangePairs()
        {
            return ListReadOnlyWrapper<IStateExchangePair>.FromEnumerable(Data.StateExchangePairs);
        }

        /// <inheritdoc />
        public IStateExchangePair GetStateExchangePair(int index)
        {
            return Data.StateExchangePairs[index];
        }

        /// <inheritdoc />
        public IRuleSetterProvider GetRuleSetterProvider(ProjectSettings projectSettings)
        {
            if (projectSettings == null)
                throw new ArgumentNullException(nameof(projectSettings));

            var transitionSetting = projectSettings.GetModuleSettings<MocassinTransitionSettings>();

            var lockSource = new AccessLockSource(projectSettings.ConcurrencySettings.MaxAttempts,
                projectSettings.ConcurrencySettings.AttemptInterval);

            return new RuleSetterProvider(new DataAccessorSource<TransitionModelData>(Data, lockSource))
            {
                AttemptFrequencyConstraint = transitionSetting.AttemptFrequency.ToConstraint()
            };
        }
    }
}