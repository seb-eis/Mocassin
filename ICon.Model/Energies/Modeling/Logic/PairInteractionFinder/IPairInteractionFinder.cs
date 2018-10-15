using System;
using System.Collections.Generic;
using ICon.Mathematics.Comparers;
using ICon.Mathematics.Constraints;
using ICon.Mathematics.Permutation;
using ICon.Model.Particles;
using ICon.Model.Structures;
using ICon.Symmetry.Analysis;
using ICon.Symmetry.SpaceGroups;

namespace ICon.Model.Energies
{
    /// <summary>
    /// Represents a pair interaction finder that can search a specfific system of space group service and unit cell provider for symmetric and asymmetric
    /// pair interactions
    /// </summary>
    public interface IPairInteractionFinder
    {
        /// <summary>
        /// The used space group service
        /// </summary>
        ISpaceGroupService SpaceGroupService { get; }

        /// <summary>
        /// The used unit cell provider
        /// </summary>
        IUnitCellProvider<IUnitCellPosition> UnitCellProvider { get; }

        /// <summary>
        /// Create a unique set of asymmetric pair interactions that results from the passed unstable environment informations.
        /// Comparer is used for geometric tolerance comparisons
        /// </summary>
        /// <param name="positions"></param>
        /// <param name="radialConstraint"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        IEnumerable<AsymmetricPairInteraction> CreateUniqueAsymmetricPairs(IEnumerable<IUnstableEnvironment> unstableEnvironments, NumericComparer comparer);

        /// <summary>
        /// Searches all provided unit cell position start points for unique symmetric pair interactions that fit the definition within the stable environment info.
        /// Comparer is used for geometric tolerance comparisons
        /// </summary>
        /// <param name="positions"></param>
        /// <param name="environmentInfo"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        IEnumerable<SymmetricPairInteraction> CreateUniqueSymmetricPairs(IEnumerable<IUnitCellPosition> positions, IStableEnvironmentInfo environmentInfo, NumericComparer comparer);
    }
}