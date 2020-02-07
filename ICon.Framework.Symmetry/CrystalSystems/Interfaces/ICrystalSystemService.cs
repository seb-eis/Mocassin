using Mocassin.Mathematics.Coordinates;
using Mocassin.Symmetry.SpaceGroups;

namespace Mocassin.Symmetry.CrystalSystems
{
    /// <summary>
    ///     Represents a crystal system service for loading and supplying crystal systems and affiliated vector converters
    ///     based on space group input
    /// </summary>
    public interface ICrystalSystemService
    {
        /// <summary>
        ///     Get the currently loaded crystal system
        /// </summary>
        CrystalSystem ActiveCrystalSystem { get; }

        /// <summary>
        ///     Get the vector transformer that handles transformations between coordinate systems
        /// </summary>
        IVectorTransformer VectorTransformer { get; }

        /// <summary>
        ///     Tries to the the parameter set. The vector encoder is updated if the set was applied successfully
        /// </summary>
        /// <param name="parameterSet"></param>
        /// <returns></returns>
        bool TrySetParameters(CrystalParameterSet parameterSet);

        /// <summary>
        ///     Load a new crystal system into the service by space group interface (Returns false if already loaded)
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        bool LoadNewSystem(ISpaceGroup group);

        /// <summary>
        ///     Get a <see cref="ActiveCrystalSystem"/> for the passed <see cref="ISpaceGroup"/>
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        CrystalSystem GetSystem(ISpaceGroup group);

        /// <summary>
        ///     Load a new crystal system into the service by a <see cref="CrystalSystemIdentification"/>
        /// </summary>
        /// <returns></returns>
        bool LoadNewSystem(CrystalSystemIdentification identification);

        /// <summary>
        ///     Get a copy of the current <see cref="CrystalParameterSet"/>
        /// </summary>
        /// <returns></returns>
        CrystalParameterSet CopyCurrentParameterSet();
    }
}