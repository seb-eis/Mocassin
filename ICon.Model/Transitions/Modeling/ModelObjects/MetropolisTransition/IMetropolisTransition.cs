using System;
using System.Collections.Generic;

using ICon.Model.Basic;
using ICon.Model.Structures;

namespace ICon.Model.Transitions
{
    /// <summary>
    /// Represents a metropolis transition that is simply defined by two exchanging sub-lattices
    /// </summary>
    public interface IMetropolisTransition : IModelObject, IEquatable<IMetropolisTransition>
    {
        /// <summary>
        /// The first unit cell position that describes the first sublattice
        /// </summary>
        IUnitCellPosition CellPosition0 { get; }

        /// <summary>
        /// The second unit cell position that describes the second sublattice
        /// </summary>
        IUnitCellPosition CellPosition1 { get; }

        /// <summary>
        /// Get the affiliated transition rules
        /// </summary>
        /// <returns></returns>
        IEnumerable<IMetropolisRule> GetTransitionRules();
    }
}
