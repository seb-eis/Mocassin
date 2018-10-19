using System;
using System.Collections.Generic;

namespace Mocassin.Model.Simulations
{
    /// <summary>
    ///     Represents a lattice blueprint set that describes creation instructions for a sequence of custom lattices
    /// </summary>
    public interface ILatticeBlueprintSet : IEnumerable<ILatticeBlueprint>
    {
        /// <summary>
        ///     Get or set the the custom random number generator of the blueprint set
        /// </summary>
        Random CustomRng { get; set; }

        /// <summary>
        ///     Get the number of blueprints described by the set
        /// </summary>
        int BlueprintCount { get; }

        /// <summary>
        ///     Get an enumerable sequence off all single build blueprints the blueprint set describes or contains
        /// </summary>
        /// <returns></returns>
        IEnumerable<ILatticeBlueprint> GetBlueprints();
    }
}