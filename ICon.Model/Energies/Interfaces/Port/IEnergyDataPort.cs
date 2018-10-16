using ICon.Framework.Collections;
using ICon.Model.Basic;

namespace ICon.Model.Energies
{
    /// <summary>
    ///     Represents a read only data access port for the energy reference data
    /// </summary>
    public interface IEnergyDataPort : IModelDataPort
    {
        /// <summary>
        ///     Get the stable environment info parameter
        /// </summary>
        /// <returns></returns>
        IStableEnvironmentInfo GetStableEnvironmentInfo();

        /// <summary>
        ///     Get a read only list of all stable pair infos
        /// </summary>
        /// <returns></returns>
        ReadOnlyListAdapter<ISymmetricPairInteraction> GetStablePairInteractions();

        /// <summary>
        ///     Get the stable pair info at the specified index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        ISymmetricPairInteraction GetStablePairInteraction(int index);

        /// <summary>
        ///     GEt a read only list of all unstable pair interactions
        /// </summary>
        /// <returns></returns>
        ReadOnlyListAdapter<IAsymmetricPairInteraction> GetUnstablePairInteractions();

        /// <summary>
        ///     Get the unstable pair interactions at the specified index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        IAsymmetricPairInteraction GetUnstablePairInteractions(int index);

        /// <summary>
        ///     Get a read only list of all existing group interaction definitions
        /// </summary>
        /// <returns></returns>
        ReadOnlyListAdapter<IGroupInteraction> GetGroupInteractions();

        /// <summary>
        ///     Get the group interaction ath the specified index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        IGroupInteraction GetGroupInteraction(int index);

        /// <summary>
        ///     Get a read only list of all unstable environment infos
        /// </summary>
        /// <returns></returns>
        ReadOnlyListAdapter<IUnstableEnvironment> GetUnstableEnvironments();

        /// <summary>
        ///     Get the unstable environment info at the specified index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        IUnstableEnvironment GetUnstableEnvironment(int index);

        /// <summary>
        ///     Get a raw energy setter provider that enables creation of energy value setters for interaction objects
        /// </summary>
        /// <returns></returns>
        IEnergySetterProvider GetEnergySetterProvider();
    }
}