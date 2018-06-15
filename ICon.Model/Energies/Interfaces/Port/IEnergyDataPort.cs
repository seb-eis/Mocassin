﻿using System;
using System.Collections.Generic;
using System.Text;

using ICon.Framework.Collections;
using ICon.Model.Basic;

namespace ICon.Model.Energies
{
    /// <summary>
    /// Represents a read only data access port for the energy reference data
    /// </summary>
    public interface IEnergyDataPort : IModelDataPort
    {
        /// <summary>
        /// Get the szabel environment info parameter
        /// </summary>
        /// <returns></returns>
        IStableEnvironmentInfo GetStableEnvironmentInfo();

        /// <summary>
        /// Get a read only list of all stable pair infos
        /// </summary>
        /// <returns></returns>
        ReadOnlyList<ISymmetricPairInteraction> GetStablePairInteractions();

        /// <summary>
        /// Get the stable pair info at the specified index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        ISymmetricPairInteraction GetStablePairInteraction(int index);

        /// <summary>
        /// GEt a read only list of all unstable pair interactions
        /// </summary>
        /// <returns></returns>
        ReadOnlyList<IAsymmetricPairInteraction> GetUnstablePairInteractions();

        /// <summary>
        /// Get the unstable pair interactions at the specififed index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        IAsymmetricPairInteraction GetUnstablePairInteractions(int index);

        /// <summary>
        /// Get a read only list of all existing group interaction definitions
        /// </summary>
        /// <returns></returns>
        ReadOnlyList<IGroupInteraction> GetGroupInteractions();

        /// <summary>
        /// Get the group interaction ath the specfified index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        IGroupInteraction GetGroupInteraction(int index);

        /// <summary>
        /// Get a read only list of all unstable environment infos
        /// </summary>
        /// <returns></returns>
        ReadOnlyList<IUnstableEnvironment> GetUnstableEnvironments();

        /// <summary>
        /// Get the unstable environmnet info at the specified index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        IUnstableEnvironment GetUnstableEnvironment(int index);
    }
}
