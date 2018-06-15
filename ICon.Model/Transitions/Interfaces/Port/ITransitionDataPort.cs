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
        /// Get a read only list containing all property state pairs
        /// </summary>
        /// <returns></returns>
        ReadOnlyList<IPropertyStatePair> GetPropertyStatePairs();

        /// <summary>
        /// Get the property state pair at the specififed index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        IPropertyStatePair GetProperytStatePair(int index);

        /// <summary>
        /// Get a read only list containing all properts groups
        /// </summary>
        /// <returns></returns>
        ReadOnlyList<IPropertyGroup> GetPropertyGroups();

        /// <summary>
        /// Get the property group at the specififed index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        IPropertyGroup GetPropertyGroup(int index);

        /// <summary>
        /// Get a read only list containing all abstract kinetic transitions
        /// </summary>
        /// <returns></returns>
        ReadOnlyList<IAbstractTransition> GetAbstractKineticTransitions();

        /// <summary>
        /// Get the abstract kinetic transition with the provided index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        IAbstractTransition GetAbstractKineticTransition(int index);

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
    }
}
