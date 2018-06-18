using System;
using System.Collections.Generic;
using System.Text;
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
        /// Get the property group at the specififed index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public IPropertyGroup GetPropertyGroup(int index)
        {
            return Data.PropertyGroups[index];
        }

        /// <summary>
        /// Get a read only list of all property groups
        /// </summary>
        /// <returns></returns>
        public ReadOnlyList<IPropertyGroup> GetPropertyGroups()
        {
            return ReadOnlyList<IPropertyGroup>.FromEnumerable(Data.PropertyGroups);
        }

        /// <summary>
        /// Get a read only list of all property state pairs
        /// </summary>
        /// <returns></returns>
        public ReadOnlyList<IPropertyStatePair> GetPropertyStatePairs()
        {
            return ReadOnlyList<IPropertyStatePair>.FromEnumerable(Data.PropertyStatePairs);
        }

        /// <summary>
        /// Get the property state pair at the specified index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public IPropertyStatePair GetProperytStatePair(int index)
        {
            return Data.PropertyStatePairs[index];
        }
    }
}
