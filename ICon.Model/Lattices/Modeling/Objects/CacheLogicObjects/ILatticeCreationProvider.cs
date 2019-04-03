using Mocassin.Model.Particles;
using Mocassin.Model.Simulations;
using Mocassin.Symmetry.Analysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mocassin.Model.Lattices
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
        // TODO: delete if no longer needed
        //List<SupercellAdapter<IParticle>> ConstructLattices(ISimulationSeriesBase simulationSeries);
    }
}
