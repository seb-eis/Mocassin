using System;
using System.Collections.Generic;
using System.Text;
using ICon.Framework.Collections;
using ICon.Model.Basic;

namespace ICon.Model.Energies
{
    /// <summary>
    /// Energy data manager that provides safe read only access to the energy base model data
    /// </summary>
    internal class EnergyDataManager : ModelDataManager<EnergyModelData>, IEnergyDataPort
    {
        /// <summary>
        /// Create new energy data manager for the provided data object
        /// </summary>
        /// <param name="data"></param>
        public EnergyDataManager(EnergyModelData data) : base(data)
        {
        }

        /// <summary>
        /// Get the stable environment info parameter
        /// </summary>
        /// <returns></returns>
        public IStableEnvironmentInfo GetStableEnvironmentInfo()
        {
            return Data.StableEnvironmentInfo;
        }

        /// <summary>
        /// Get the group interaction at the specfifed index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public IGroupInteraction GetGroupInteraction(int index)
        {
            return Data.GroupInteractions[index];
        }

        /// <summary>
        /// Get a read only list for all defined group interactions
        /// </summary>
        /// <returns></returns>
        public ReadOnlyList<IGroupInteraction> GetGroupInteractions()
        {
            return ReadOnlyList<IGroupInteraction>.FromEnumerable(Data.GroupInteractions);
        }

        /// <summary>
        /// Get the stable pair interaction at the specified index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public ISymmetricPairInteraction GetStablePairInteraction(int index)
        {
            return Data.SymmetricPairInteractions[index];
        }

        /// <summary>
        /// Get a read only list of all stable pair interactions
        /// </summary>
        /// <returns></returns>
        public ReadOnlyList<ISymmetricPairInteraction> GetStablePairInteractions()
        {
            return ReadOnlyList<ISymmetricPairInteraction>.FromEnumerable(Data.SymmetricPairInteractions);
        }

        /// <summary>
        /// Get the unstable environment info at the specified index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public IUnstableEnvironment GetUnstableEnvironment(int index)
        {
            return Data.UnstableEnvironmentInfos[index];
        }

        /// <summary>
        /// Get a read only list of all environment infos
        /// </summary>
        /// <returns></returns>
        public ReadOnlyList<IUnstableEnvironment> GetUnstableEnvironments()
        {
            return ReadOnlyList<IUnstableEnvironment>.FromEnumerable(Data.UnstableEnvironmentInfos);
        }

        /// <summary>
        /// Get the unstable pair interaction at the specified index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public IAsymmetricPairInteraction GetUnstablePairInteractions(int index)
        {
            return Data.AsymmetricPairInteractions[index];
        }

        /// <summary>
        /// Get a read only list of all unstable pair infos
        /// </summary>
        /// <returns></returns>
        public ReadOnlyList<IAsymmetricPairInteraction> GetUnstablePairInteractions()
        {
            return ReadOnlyList<IAsymmetricPairInteraction>.FromEnumerable(Data.AsymmetricPairInteractions);
        }
    }
}
