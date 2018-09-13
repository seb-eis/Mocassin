using ICon.Framework.Collections;
using ICon.Model.Basic;

namespace ICon.Model.Transitions
{
    /// <summary>
    /// Represents a read only data access port for the transition reference data
    /// </summary>
    public interface ITransitionDataPort : IModelDataPort
    {
        /// <summary>
        /// Get a read only list containing all state exchange pairs
        /// </summary>
        /// <returns></returns>
        ReadOnlyList<IStateExchangePair> GetStateExchangePairs();

        /// <summary>
        /// Get the state exchaage pair at the specified index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        IStateExchangePair GetStateExchnagePair(int index);

        /// <summary>
        /// Get a read only list containing all state exchange groups groups
        /// </summary>
        /// <returns></returns>
        ReadOnlyList<IStateExchangeGroup> GetStateExchangeGroups();

        /// <summary>
        /// Get the property group at the specififed index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        IStateExchangeGroup GetStateExchangeGroup(int index);

        /// <summary>
        /// Get a read only list containing all abstract transitions
        /// </summary>
        /// <returns></returns>
        ReadOnlyList<IAbstractTransition> GetAbstractTransitions();

        /// <summary>
        /// Get the abstract transition with the provided index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        IAbstractTransition GetAbstractTransition(int index);

        /// <summary>
        /// Get a read only list containing all kinetic transitions
        /// </summary>
        /// <returns></returns>
        ReadOnlyList<IKineticTransition> GetKineticTransitions();

        /// <summary>
        /// Get a read only list containing all metropolis transitions
        /// </summary>
        /// <returns></returns>
        ReadOnlyList<IMetropolisTransition> GetMetropolisTransitions();

        /// <summary>
        /// Get the number of kinetic transitions
        /// </summary>
        /// <returns></returns>
        int GetKineticTransitionCount();

        /// <summary>
        /// Get the number of metropolis transitions
        /// </summary>
        /// <returns></returns>
        int GetMetropolisTransitonCount();
    }
}
