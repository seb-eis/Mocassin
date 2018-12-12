using System;

namespace Mocassin.Model.Translator.Jobs
{
    /// <summary>
    ///     A simulation lattice source that transforms lattice configurations into simulation lattice objects
    /// </summary>
    public interface ISimulationLatticeSource
    {
        /// <summary>
        ///     Get the encoded lattice for the passed lattice configuration using the provided random number generator
        /// </summary>
        /// <param name="latticeConfiguration"></param>
        /// <param name="random"></param>
        /// <returns></returns>
        byte[] GetEncodedLattice(LatticeConfiguration latticeConfiguration, Random random);

        /// <summary>
        ///     Get the encoded energy background for the passed lattice configuration
        /// </summary>
        /// <param name="latticeConfiguration"></param>
        /// <returns></returns>
        double[] GetEncodedBackground(LatticeConfiguration latticeConfiguration);
    }
}