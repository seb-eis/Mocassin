using System;
using System.Collections.Generic;
using Mocassin.Mathematics.ValueTypes;

namespace Mocassin.Model.Lattices
{
    /// <summary>
    ///     General interface for systems that create 4D lattices based on doping information
    /// </summary>
    public interface IDopedLatticeSource
    {
        /// <summary>
        ///     Generates a 4D lattice that encodes each particle as a byte
        /// </summary>
        /// <param name="sizeVector"></param>
        /// <param name="dopingDictionary"></param>
        /// <param name="rng"></param>
        /// <returns></returns>
        byte[,,,] BuildByteLattice(DataIntVector3D sizeVector, IDictionary<IDoping, double> dopingDictionary, Random rng);
    }
}