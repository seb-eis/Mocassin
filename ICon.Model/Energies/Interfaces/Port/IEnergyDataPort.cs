using Mocassin.Framework.Collections;
using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;
using Mocassin.Model.Structures;

namespace Mocassin.Model.Energies
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
        ///     Get the unstable environment that belongs to the passed unit cell position. Returns null if it does not exist
        /// </summary>
        /// <param name="unitCellPosition"></param>
        /// <returns></returns>
        IUnstableEnvironment GetUnstableEnvironment(IUnitCellPosition unitCellPosition);

        /// <summary>
        ///     Get an <see cref="IEnergySetterProvider" /> for all interactions that conforms to the passed
        ///     <see cref="ProjectSettings" />
        /// </summary>
        /// <returns></returns>
        IEnergySetterProvider GetEnergySetterProvider(ProjectSettings projectSettings);
    }
}