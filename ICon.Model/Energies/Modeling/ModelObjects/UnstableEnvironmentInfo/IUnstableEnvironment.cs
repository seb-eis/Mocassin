using System.Collections.Generic;
using Mocassin.Model.Basic;
using Mocassin.Model.Structures;

namespace Mocassin.Model.Energies
{
    /// <summary>
    ///     Represents an unstable environment description with pair interactions and groups that belongs to a specific unit
    ///     cell position
    /// </summary>
    public interface IUnstableEnvironment : IModelObject
    {
        /// <summary>
        ///     The interaction range of the environment
        /// </summary>
        double MaxInteractionRange { get; }

        /// <summary>
        ///     The unit cell position the environment info belongs to
        /// </summary>
        IUnitCellPosition UnitCellPosition { get; }

        /// <summary>
        ///     Get all interaction filters of the unstable environment
        /// </summary>
        /// <returns></returns>
        IEnumerable<IInteractionFilter> GetInteractionFilters();

        /// <summary>
        ///     Get all pair interactions affiliated with this environment
        /// </summary>
        /// <returns></returns>
        IEnumerable<IAsymmetricPairInteraction> GetPairInteractions();

        /// <summary>
        ///     Get all group interactions affiliated with this environment
        /// </summary>
        /// <returns></returns>
        IEnumerable<IGroupInteraction> GetGroupInteractions();
    }
}