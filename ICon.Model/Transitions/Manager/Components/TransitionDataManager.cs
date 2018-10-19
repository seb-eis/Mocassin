using System.Linq;
using Mocassin.Framework.Collections;
using Mocassin.Model.Basic;

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
        public ReadOnlyListAdapter<IAbstractTransition> GetAbstractTransitions()
        {
            return ReadOnlyListAdapter<IAbstractTransition>.FromEnumerable(Data.AbstractTransitions);
        }

        /// <inheritdoc />
        public int GetKineticTransitionCount()
        {
            return Data.KineticTransitions.Count(a => !a.IsDeprecated);
        }

        /// <inheritdoc />
        public ReadOnlyListAdapter<IKineticTransition> GetKineticTransitions()
        {
            return ReadOnlyListAdapter<IKineticTransition>.FromEnumerable(Data.KineticTransitions);
        }

        /// <inheritdoc />
        public ReadOnlyListAdapter<IMetropolisTransition> GetMetropolisTransitions()
        {
            return ReadOnlyListAdapter<IMetropolisTransition>.FromEnumerable(Data.MetropolisTransitions);
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
        public ReadOnlyListAdapter<IStateExchangeGroup> GetStateExchangeGroups()
        {
            return ReadOnlyListAdapter<IStateExchangeGroup>.FromEnumerable(Data.StateExchangeGroups);
        }

        /// <inheritdoc />
        public ReadOnlyListAdapter<IStateExchangePair> GetStateExchangePairs()
        {
            return ReadOnlyListAdapter<IStateExchangePair>.FromEnumerable(Data.StateExchangePairs);
        }

        /// <inheritdoc />
        public IStateExchangePair GetStateExchangePair(int index)
        {
            return Data.StateExchangePairs[index];
        }
    }
}