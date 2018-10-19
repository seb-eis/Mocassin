using System;
using System.Collections.Generic;
using Mocassin.Model.Basic;
using Mocassin.Model.Structures;

namespace Mocassin.Model.Transitions
{
    /// <summary>
    ///     Represents a metropolis transition that is simply defined by two exchanging sub-lattices
    /// </summary>
    public interface IMetropolisTransition : IModelObject, IEquatable<IMetropolisTransition>
    {
        /// <summary>
        ///     The first unit cell position that describes the first sub-lattice
        /// </summary>
        IUnitCellPosition FirstUnitCellPosition { get; }

        /// <summary>
        ///     The second unit cell position that describes the second sub-lattice
        /// </summary>
        IUnitCellPosition SecondUnitCellPosition { get; }

        /// <summary>
        ///     The affiliated abstract transition describing the transition process
        /// </summary>
        IAbstractTransition AbstractTransition { get; }

        /// <summary>
        ///     Get the affiliated transition rules
        /// </summary>
        /// <returns></returns>
        IEnumerable<IMetropolisRule> GetTransitionRules();

        /// <summary>
        ///     Get the affiliated transition rules including the dependent rules
        /// </summary>
        /// <returns></returns>
        IEnumerable<IMetropolisRule> GetExtendedTransitionRules();

        /// <summary>
        ///     Returns true if the mappings contain their own inverted version
        /// </summary>
        /// <returns></returns>
        bool MappingsContainInversion();
    }
}