using System.Collections.Generic;
using Mocassin.Model.Basic;
using Mocassin.Model.Structures;

namespace Mocassin.Model.Transitions
{
    /// <summary>
    ///     Represents an access port for on-demand extended transition data that is automatically cached
    /// </summary>
    public interface ITransitionCachePort : IModelCachePort
    {
        /// <summary>
        ///     Get a list interface for all metropolis transition mapping lists
        /// </summary>
        /// <returns></returns>
        IList<IList<MetropolisMapping>> GetAllMetropolisMappingLists();

        /// <summary>
        ///     Get a list interface for the metropolis transition mappings belonging the specified transition index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        IList<MetropolisMapping> GetMetropolisMappingList(int index);

        /// <summary>
        ///     Get a list interface for all kinetic transition mapping lists
        /// </summary>
        /// <returns></returns>
        IList<IList<KineticMapping>> GetAllKineticMappingLists();

        /// <summary>
        ///     Get a list interface for the kinetic transition mappings belonging to the specified transition index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        IList<KineticMapping> GetKineticMappingList(int index);

        /// <summary>
        ///     Get a list interface for all metropolis transition rule lists
        /// </summary>
        /// <returns></returns>
        IList<IList<MetropolisRule>> GetAllMetropolisRuleLists();

        /// <summary>
        ///     Get a list interface for all metropolis rules that belong to the specified transition index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        IList<MetropolisRule> GetMetropolisRuleList(int index);

        /// <summary>
        ///     Get a list interface for all kinetic transition rule lists
        /// </summary>
        /// <returns></returns>
        IList<IList<KineticRule>> GetAllKineticRuleLists();

        /// <summary>
        ///     Get a list interface for all kinetic rules that belong to the specified transition index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        IList<KineticRule> GetKineticRuleList(int index);

        /// <summary>
        ///     Get a dictionary that contains which kinetic transitions are possible on which unit cell positions
        /// </summary>
        /// <returns></returns>
        IDictionary<IUnitCellPosition, HashSet<IKineticTransition>> GetKineticTransitionPositionDictionary();

        /// <summary>
        ///     Get a dictionary that contains which metropolis transitions are possible on which unit cell positions
        /// </summary>
        /// <returns></returns>
        IDictionary<IUnitCellPosition, HashSet<IMetropolisTransition>> GetMetropolisTransitionPositionDictionary();

        /// <summary>
        ///     Get the abstract charge transport chain of the abstract movement with the provided index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        IList<double> GetAbstractChargeTransportChain(int index);

        /// <summary>
        ///     Get the abstract charge transport chains for all abstract transitions
        /// </summary>
        /// <returns></returns>
        IList<IList<double>> GetAbstractChargeTransportChains();

        /// <summary>
        ///     Get a <see cref="IRuleSetterProvider" /> that confirms to the internally set
        ///     <see cref="Mocassin.Model.ModelProject.ProjectSettings" />
        /// </summary>
        /// <returns></returns>
        IRuleSetterProvider GetRuleSetterProvider();
    }
}