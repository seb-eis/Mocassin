using Mocassin.Symmetry.SpaceGroups;

namespace Mocassin.Symmetry.CrystalSystems
{
    /// <summary>
    ///     Common interface for all crystal system provider implementations
    /// </summary>
    public interface ICrystalSystemSource
    {
        /// <summary>
        ///     The max value for parameters
        /// </summary>
        double ParameterMaxValue { get; set; }

        /// <summary>
        ///     The tolerance range for parameter comparisons within the crystal systems
        /// </summary>
        double ToleranceRange { get; set; }

        /// <summary>
        ///     Create a new crystal system for the provided space group interface
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        CrystalSystem Create(ISpaceGroup group);

        /// <summary>
        ///     Create a new crystal system for the provided system index and variation name
        /// </summary>
        /// <param name="systemIndex"></param>
        /// <param name="variationName"></param>
        /// <returns></returns>
        CrystalSystem Create(int systemIndex, string variationName);
    }
}