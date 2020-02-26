using System.Collections.Generic;
using Mocassin.Mathematics.Comparer;
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
        ///     Create a unique set of <see cref="UnstablePairInteraction"/> objects that results from the passed <see cref="IUnstableEnvironment"/> set.
        ///     Comparer is used for geometric tolerance comparisons
        /// </summary>
        /// <param name="unstableEnvironments"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        IEnumerable<UnstablePairInteraction> SampleUniqueUnstablePairs(IEnumerable<IUnstableEnvironment> unstableEnvironments,
            NumericComparer comparer);

        /// <summary>
        ///     Create a unique set of <see cref="StablePairInteraction"/> objects that results from the passed <see cref="IStableEnvironmentInfo"/>.
        ///     Comparer is used for geometric tolerance comparisons
        /// </summary>
        /// <param name="positions"></param>
        /// <param name="environmentInfo"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        IEnumerable<StablePairInteraction> SampleUniqueStablePairs(IEnumerable<ICellReferencePosition> positions,
            IStableEnvironmentInfo environmentInfo, NumericComparer comparer);
    }
}