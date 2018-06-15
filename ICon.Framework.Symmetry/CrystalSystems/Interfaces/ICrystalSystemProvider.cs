using System;
using ICon.Symmetry.SpaceGroups;

namespace ICon.Symmetry.CrystalSystems
{
    /// <summary>
    /// Common interface for all cyrstal system provider implementations
    /// </summary>
    public interface ICrystalSystemProvider
    {
        /// <summary>
        /// The max value for parameters
        /// </summary>
        Double ParameterMaxValue { get; set; }

        /// <summary>
        /// The tolerance range for parameter comparisons within the crystal systems
        /// </summary>
        Double ToleranceRange { get; set; }

        /// <summary>
        /// Create a new crystal system for the provided space group interface
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        CrystalSystem Create(ISpaceGroup group);

        /// <summary>
        /// Create a new crytsal system for the provided system index and variation name
        /// </summary>
        /// <param name="SystemIndex"></param>
        /// <param name="VariationName"></param>
        /// <returns></returns>
        CrystalSystem Create(Int32 systemIndex, String variationName);
    }
}