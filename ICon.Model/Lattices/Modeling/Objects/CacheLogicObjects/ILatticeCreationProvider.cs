using System;
using System.Collections.Generic;
using Mocassin.Mathematics.ValueTypes;

namespace Mocassin.Model.Lattices
{
    /// <summary>
    ///     Interface of lattice creation provider. Constructs the lattice with a given simulationSeries
    /// </summary>
    public interface ILatticeCreationProvider
    {
        /// <summary>
        ///     Generate lattices with simulationSeries
        /// </summary>
        /// <param name="simulationSeries"></param>
        /// <returns></returns>
        byte[,,,] BuildLattice(DataIntVector3D sizeVector, IDictionary<IDoping, double> dopings, Random rng);
    }
}