using Mocassin.Framework.Collections;
using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Transitions
{
    /// <summary>
    ///     Represents a read only data access port for the transition reference data
    /// </summary>
    public interface ITransitionDataPort : IModelDataPort
    {
        /// <summary>
        ///     Get a read only list containing all state exchange pairs
        /// </summary>
        /// <returns></returns>
        ListReadOnlyWrapper<IStateExchangePair> GetStateExchangePairs();

        /// <summary>
        ///     Get the state exchange pair at the specified index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        IStateExchangePair GetStateExchangePair(int index);

        /// <summary>
        ///     Get a read only list containing all state exchange groups groups
        /// </summary>
        /// <returns></returns>
        ListReadOnlyWrapper<IStateExchangeGroup> GetStateExchangeGroups();

        /// <summary>
        ///     Get the property group at the specified index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        IStateExchangeGroup GetStateExchangeGroup(int index);

        /// <summary>
        ///     Get a read only list containing all abstract transitions
        /// </summary>
        /// <returns></returns>
        ListReadOnlyWrapper<IAbstractTransition> GetAbstractTransitions();

        /// <summary>
        ///     Get the abstract transition with the provided index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        IAbstractTransition GetAbstractTransition(int index);

        /// <summary>
        ///     Get a read only list containing all kinetic transitions
        /// </summary>
        /// <returns></returns>
        ListReadOnlyWrapper<IKineticTransition> GetKineticTransitions();

        /// <summary>
        ///     Get a read only list containing all metropolis transitions
        /// </summary>
        /// <returns></returns>
        ListReadOnlyWrapper<IMetropolisTransition> GetMetropolisTransitions();

        /// <summary>
        ///     Get the number of kinetic transitions
        /// </summary>
        /// <returns></returns>
        int GetKineticTransitionCount();

        /// <summary>
        ///     Get the number of metropolis transitions
        /// </summary>
        /// <returns></returns>
        int GetMetropolisTransitionCount();

        /// <summary>
        ///     Get a <see cref="IRuleSetterProvider" /> that uses the constraints defined by the passed
        ///     <see cref="ProjectSettings" />
        /// </summary>
        /// <param name="projectSettings"></param>
        /// <returns></returns>
        IRuleSetterProvider GetRuleSetterProvider(ProjectSettings projectSettings);
    }
}