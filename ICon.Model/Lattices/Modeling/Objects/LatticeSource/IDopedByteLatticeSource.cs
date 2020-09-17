using System;
using System.Collections.Generic;
using Moccasin.Mathematics.ValueTypes;

namespace Mocassin.Model.Lattices
{
    /// <summary>
    ///     Represents a source for doped byte[,,,] based 4D lattices based on <see cref="IDoping" /> information
    /// </summary>
    public interface IDopedByteLatticeSource
    {
        /// <summary>
        ///     Generates a 4D lattice that encodes each particle as a byte
        /// </summary>
        /// <param name="sizeVector"></param>
        /// <param name="dopingDictionary"></param>
        /// <param name="rng"></param>
        /// <returns></returns>
        byte[,,,] CreateLattice(VectorI3 sizeVector, IDictionary<IDoping, double> dopingDictionary, Random rng);

        /// <summary>
        ///     Generates a 4D lattice that encodes each particle as a byte on an existing byte[,,,] object
        /// </summary>
        /// <param name="dopingDictionary"></param>
        /// <param name="rng"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        void PopulateLattice(IDictionary<IDoping, double> dopingDictionary, Random rng, byte[,,,] target);
    }
}