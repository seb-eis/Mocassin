using System;
using System.Collections.Generic;
using System.Text;

using ICon.Mathematics.ValueTypes;
using ICon.Model.Lattices;

namespace ICon.Model.Simulations
{
    /// <summary>
    /// Represents a lattice blueprint that is used to create a custom lattice based upon a simple set of instructions
    /// </summary>
    public interface ILatticeBlueprint
    {
        /// <summary>
        /// The custom RNG instance that should be used for random doping distribution
        /// </summary>
        Random CustomRng { get; }

        /// <summary>
        /// Flag that defines that the lattice should use the special user defined lattice as a building base instead of a plain default occupied base
        /// </summary>
        bool UseCustomBase { get; }

        /// <summary>
        /// The size of the lattice that should be created
        /// </summary>
        DataIntVector3D SizeVector { get; }

        /// <summary>
        /// Get all doping infos of the blueprint
        /// </summary>
        IEnumerable<DopingInfo> GetDopingInfos { get; }
    }
}
