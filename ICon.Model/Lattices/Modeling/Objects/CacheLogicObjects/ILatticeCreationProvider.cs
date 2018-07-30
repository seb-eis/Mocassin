using ICon.Model.Particles;
using ICon.Model.Simulations;
using ICon.Symmetry.Analysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Model.Lattices
{
    public interface ILatticeCreationProvider
    {
        List<SupercellWrapper<IParticle>> ConstructLattice(ISimulationSeriesBase bluePrint);
    }
}
