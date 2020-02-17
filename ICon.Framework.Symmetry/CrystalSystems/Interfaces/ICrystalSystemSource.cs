using Mocassin.Symmetry.SpaceGroups;

namespace Mocassin.Symmetry.CrystalSystems
{
    /// <summary>
    ///     Common interface for all crystal system provider implementations
    /// </summary>
    public interface ICrystalSystemSource
    {
        /// <summary>
        ///     Get or set the max value for parameters
        /// </summary>
        double ParameterMaxValue { get; set; }

        /// <summary>
        ///     Get or set tolerance range for parameter comparisons within the crystal systems
        /// </summary>
        double Tolerance { get; set; }

        /// <summary>
        ///     Get a <see cref="CrystalSystem" /> for the provided <see cref="ISpaceGroup" />
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        CrystalSystem GetSystem(ISpaceGroup group);

        /// <summary>
        ///     Get a <see cref="CrystalSystem" /> based oon a <see cref="CrystalSystemIdentification" />
        /// </summary>
        /// <param name="identification"></param>
        /// <returns></returns>
        CrystalSystem GetSystem(CrystalSystemIdentification identification);
    }
}