using System.Collections.Generic;
using Mocassin.Mathematics.Comparers;
using Mocassin.Model.Structures;
using Mocassin.Symmetry.Analysis;
using Mocassin.Symmetry.SpaceGroups;

namespace Mocassin.Model.Energies
{
    /// <summary>
    ///     Represents a pair interaction finder that can search a specific system of space group service and unit cell
    ///     provider for symmetric and asymmetric pair interactions
    /// </summary>
    public interface IPairInteractionFinder
    {
        /// <summary>
        ///     The used space group service
        /// </summary>
        ISpaceGroupService SpaceGroupService { get; }

        /// <summary>
        ///     The used unit cell provider
        /// </summary>
        IUnitCellProvider<ICellReferencePosition> UnitCellProvider { get; }

        /// <summary>
        ///     Create a unique set of asymmetric pair interactions that results from the passed unstable environment information.
        ///     Comparer is used for geometric tolerance comparisons
        /// </summary>
        /// <param name="unstableEnvironments"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        IEnumerable<AsymmetricPairInteraction> CreateUniqueAsymmetricPairs(IEnumerable<IUnstableEnvironment> unstableEnvironments,
            NumericComparer comparer);

        /// <summary>
        ///     Searches all provided unit cell position start points for unique symmetric pair interactions that fit the
        ///     definition within the stable environment info.
        ///     Comparer is used for geometric tolerance comparisons
        /// </summary>
        /// <param name="positions"></param>
        /// <param name="environmentInfo"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        IEnumerable<SymmetricPairInteraction> CreateUniqueSymmetricPairs(IEnumerable<ICellReferencePosition> positions,
            IStableEnvironmentInfo environmentInfo, NumericComparer comparer);
    }
}