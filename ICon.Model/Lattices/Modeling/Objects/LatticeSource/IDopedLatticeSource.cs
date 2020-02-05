﻿using System;
using System.Collections.Generic;
using Moccasin.Mathematics.ValueTypes;

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
        byte[,,,] BuildByteLattice(VectorI3 sizeVector, IDictionary<IDoping, double> dopingDictionary, Random rng);

        /// <summary>
        ///     Generates a 4D lattice that encodes each particle as a byte on an existing byte[,,,] object
        /// </summary>
        /// <param name="dopingDictionary"></param>
        /// <param name="rng"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        void PopulateByteLattice(IDictionary<IDoping, double> dopingDictionary, Random rng, byte[,,,] target);
    }
}