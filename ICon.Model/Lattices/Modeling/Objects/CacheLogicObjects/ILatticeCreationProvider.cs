using ICon.Model.Particles;
using ICon.Model.Simulations;
using ICon.Symmetry.Analysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Model.Lattices
{
    /// <summary>
    /// Interface of lattice creation provider. Constructs the lattice with a given simulationSeries
    /// </summary>
    public interface ILatticeCreationProvider
    {
        /// <summary>
        /// Generate lattices with simulationSeries
        /// </summary>
        /// <param name="simulationSeries"></param>
        /// <returns></returns>
        List<SupercellWrapper<IParticle>> ConstructLattices(ISimulationSeriesBase simulationSeries);
    }
}
