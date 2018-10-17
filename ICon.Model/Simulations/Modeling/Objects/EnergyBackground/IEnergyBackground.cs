using System;
using System.Collections.Generic;
using System.Text;
using Mocassin.Mathematics.ValueTypes;
using Mocassin.Model.Particles;

namespace Mocassin.Model.Simulations
{
    /// <summary>
    /// Represents a 5D simulation energy background that assigns each particle on each possible position an energetic offset value
    /// </summary>
    public interface IEnergyBackground : IEnumerable<double>
    {
        /// <summary>
        /// Generates the full 5D energy hyper surface by the set of existing particles and the lattice dimension as a 4D vector
        /// </summary>
        /// <param name="particles"></param>
        /// <returns></returns>
        double[,,,,] GetHyperSurface(IEnumerable<IParticle> particles, ILinearVector4D latticeDimension);

        /// <summary>
        /// Get the energy value belonging to the provided 4D coordinate information and particle
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="particle"></param>
        /// <returns></returns>
        double GetValue(ILinearVector4D vector, IParticle particle);

        /// <summary>
        /// Get an energy value belonging to the provided 3D fractional position and particle
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="particle"></param>
        /// <returns></returns>
        double GetValue(IFractional3D vector, IParticle particle);
    }
}
