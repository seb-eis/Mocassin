using System;
using System.Collections.Generic;
using System.Text;
using ICon.Symmetry.SpaceGroups;
using ICon.Mathematics.Coordinates;

namespace ICon.Symmetry.CrystalSystems
{
    /// <summary>
    /// Represents a cyrstal system service for loading and supplying crystal systems and affiliated vector convertes based on space group input
    /// </summary>
    public interface ICrystalSystemService
    {
        /// <summary>
        /// Get the currently loaded crystal system
        /// </summary>
        CrystalSystem CrystalSystem { get; }

        /// <summary>
        /// Get the vector transformer that handles transformations between coordinate systems
        /// </summary>
        VectorTransformer VectorTransformer { get; }

        /// <summary>
        /// Tries to the the parameter set. The vector encoder is updated if the set was applied successfully
        /// </summary>
        /// <param name="parameterSet"></param>
        /// <returns></returns>
        bool TrySetParameters(CrystalParameterSet parameterSet);

        /// <summary>
        /// Load a new crystal system into the service by space group interface (Returns false if already loaded)
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        bool LoadNewSystem(ISpaceGroup group);

        /// <summary>
        /// Load a new crytsal system into the service by system index and variation name
        /// </summary>
        /// <param name="SystemIndex"></param>
        /// <param name="VariationName"></param>
        /// <returns></returns>
        bool LoadNewSystem(int SystemIndex, string VariationName);

        /// <summary>
        /// Get a copy of the current parameter set
        /// </summary>
        /// <returns></returns>
        CrystalParameterSet GetCurrentParameterSet();
    }
}
