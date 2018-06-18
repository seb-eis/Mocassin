using System.Collections.Generic;

using ICon.Mathematics.ValueTypes;
using ICon.Framework.Collections;
using ICon.Model.Basic;

namespace ICon.Model.Transitions
{
    /// <summary>
    /// Represents an access port for on-demand extended transition data that is automatically cached
    /// </summary>
    public interface ITransitionCachePort : IModelCachePort
    {
        /// <summary>
        /// Get a list interface for all metropolis transition mapping lists
        /// </summary>
        /// <returns></returns>
        IList<IList<MetropolisMapping>> GetAllMetropolisMappingLists();

        /// <summary>
        /// Get a list interface for the metropolis transition mappings belonging the specfified transition index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        IList<MetropolisMapping> GetMetropolisMappingList(int index);

        /// <summary>
        /// Get a list interface for all kinetic transition mapping lists
        /// </summary>
        /// <returns></returns>
        IList<IList<KineticMapping>> GetAllKineticMappingLists();

        /// <summary>
        /// Get a list interface for the kinetic transition mappings belonging to the specified transition index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        IList<KineticMapping> GetKineticMappingList(int index);

        /// <summary>
        /// Get a list interface for all metropolis transition rule lists
        /// </summary>
        /// <returns></returns>
        IList<IList<MetropolisRule>> GetAllMetropolisRuleLists();

        /// <summary>
        /// Get a list interface for all metropolis rules that belong to the specfified transition index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        IList<MetropolisRule> GetMetropolisRuleList(int index);

        /// <summary>
        /// Get a list interface for all kinetic transition rule lists
        /// </summary>
        /// <returns></returns>
        IList<IList<KineticRule>> GetAllKineticRuleLists();

        /// <summary>
        /// Get a list interface for all kinetic rules that belong to the specfified transition index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        IList<KineticRule> GetKineticRuleList(int index);

        /// <summary>
        /// Get a list of state pair groups for each refernce position of the unit cell. Groups contain all possible state pairs for these positions
        /// </summary>
        /// <returns></returns>
        IList<StatePairGroup> GetPossibleStatePairsForAllPositions();
    }
}
