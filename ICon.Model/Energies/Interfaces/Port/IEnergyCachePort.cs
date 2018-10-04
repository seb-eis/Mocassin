using System.Collections.Generic;
using ICon.Mathematics.Constraints;
using ICon.Model.Basic;
using ICon.Model.Structures;

namespace ICon.Model.Energies
{
    /// <summary>
    /// Represents an access port for on-demand extended energy data that is automatically cached
    /// </summary>
    public interface IEnergyCachePort : IModelCachePort
    {
        /// <summary>
        /// Get a pair interaction finder that can be used to search the currently linked structure system for symmetric and asymmetric interactions
        /// </summary>
        /// <returns></returns>
        IPairInteractionFinder GetPairInteractionFinder();

        /// <summary>
        /// Get the position group information for all defined group interactions
        /// </summary>
        /// <returns></returns>
        IReadOnlyList<IPositionGroupInfo> GetPositionGroupInfos();

        /// <summary>
        /// Get the position group info that belongs to the interaction group at the provided index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        IPositionGroupInfo GetPositionGroupInfo(int index);

        /// <summary>
        /// Get a read only dictionary that assigns each unit cell position its set of defined pair interactions
        /// </summary>
        /// <returns></returns>
        IReadOnlyDictionary<IUnitCellPosition, IReadOnlyList<IPairInteraction>> GetPositionPairInteractions();

        /// <summary>
        /// Get a read only dictionary that assigns each unit cell position its set of defined group interactions
        /// </summary>
        /// <returns></returns>
        IReadOnlyDictionary<IUnitCellPosition, IReadOnlyList<IGroupInteraction>> GetPositionGroupInteractions();

        /// <summary>
        /// Get an energy setter provider that has the value constraints set to their porject settings defined values
        /// </summary>
        /// <returns></returns>
        IEnergySetterProvider GetFullEnergySetterProvider();

        /// <summary>
        /// Get a read only list of all stable pair interaction energy setters
        /// </summary>
        /// <returns></returns>
        IReadOnlyList<IPairEnergySetter> GetStablePairEnergySetters();

        /// <summary>
        /// Get the stable pair interaction energy setter for the interaction at the provided index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        IPairEnergySetter GetStablePairEnergySetter(int index);

        /// <summary>
        /// Get a read only list of all unstable pair interaction energy setters
        /// </summary>
        /// <returns></returns>
        IReadOnlyList<IPairEnergySetter> GetUnstablePairEnergySetters();

        /// <summary>
        /// Get the unstable pair interaction energy setter for the interaction at the provided index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        IPairEnergySetter GetUnstablePairEnergySetter(int index);

        /// <summary>
        /// Get a read only list of all energy setters for group interactions
        /// </summary>
        /// <returns></returns>
        IReadOnlyList<IGroupEnergySetter> GetGroupEnergySetters();

        /// <summary>
        /// Get the group interaction energy setter for the group interaction at the provided index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        IGroupEnergySetter GetGroupEnergySetter(int index);
    }
}
