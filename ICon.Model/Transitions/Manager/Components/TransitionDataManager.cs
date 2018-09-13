using System;
using System.Collections.Generic;
using System.Linq;
using ICon.Framework.Collections;
using ICon.Model.Basic;

namespace ICon.Model.Transitions
{
    /// <summary>
    /// Transition data manager that provides safe read only access to the transition base data
    /// </summary>
    internal class TransitionDataManager : ModelDataManager<TransitionModelData>, ITransitionDataPort
    {
        /// <summary>
        /// Create new transition data manage for the provided data object
        /// </summary>
        /// <param name="data"></param>
        public TransitionDataManager(TransitionModelData data) : base(data)
        {

        }

        /// <summary>
        /// Get the abstract transition with the specified index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public IAbstractTransition GetAbstractTransition(int index)
        {
            return Data.AbstractTransitions[index];
        }

        /// <summary>
        /// Get a read only list of all abstract transitions
        /// </summary>
        /// <returns></returns>
        public ReadOnlyList<IAbstractTransition> GetAbstractTransitions()
        {
            return ReadOnlyList<IAbstractTransition>.FromEnumerable(Data.AbstractTransitions);
        }

        /// <summary>
        /// Get the number of metropolis transitions that are not deprecated
        /// </summary>
        /// <returns></returns>
        public int GetKineticTransitionCount()
        {
            return Data.KineticTransitions.Where(a => !a.IsDeprecated).Count();
        }

        /// <summary>
        /// Get a read only list of all kinetic transitions
        /// </summary>
        /// <returns></returns>
        public ReadOnlyList<IKineticTransition> GetKineticTransitions()
        {
            return ReadOnlyList<IKineticTransition>.FromEnumerable(Data.KineticTransitions);
        }

        /// <summary>
        /// Get a read only list of all metropolis transitions
        /// </summary>
        /// <returns></returns>
        public ReadOnlyList<IMetropolisTransition> GetMetropolisTransitions()
        {
            return ReadOnlyList<IMetropolisTransition>.FromEnumerable(Data.MetropolisTransitions);
        }

        /// <summary>
        /// Get the number of metropolis transitions that are not deprecated
        /// </summary>
        /// <returns></returns>
        public int GetMetropolisTransitonCount()
        {
            return Data.MetropolisTransitions.Where(a => !a.IsDeprecated).Count();
        }

        /// <summary>
        /// Get the state exchange group at the specififed index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public IStateExchangeGroup GetStateExchangeGroup(int index)
        {
            return Data.StateExchangeGroups[index];
        }

        /// <summary>
        /// Get a read only list of all state exchange groups
        /// </summary>
        /// <returns></returns>
        public ReadOnlyList<IStateExchangeGroup> GetStateExchangeGroups()
        {
            return ReadOnlyList<IStateExchangeGroup>.FromEnumerable(Data.StateExchangeGroups);
        }

        /// <summary>
        /// Get a read only list of all state exchange pairs
        /// </summary>
        /// <returns></returns>
        public ReadOnlyList<IStateExchangePair> GetStateExchangePairs()
        {
            return ReadOnlyList<IStateExchangePair>.FromEnumerable(Data.StateExchangePairs);
        }

        /// <summary>
        /// Get the state exchange pair at the specified index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public IStateExchangePair GetStateExchnagePair(int index)
        {
            return Data.StateExchangePairs[index];
        }
    }
}
