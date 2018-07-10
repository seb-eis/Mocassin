using ICon.Mathematics.Coordinates;
using ICon.Model.Particles;
using ICon.Symmetry.Analysis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using ICon.Framework.Extensions;

namespace ICon.Model.Lattices
{
    /// <summary>
    /// Translates the WorkLattice to a SupercellWrapper
    /// </summary>
    public class SupercellTranslator
    {
        /// <summary>
        /// Translates the WorkLattice to a SupercellWrapper
        /// </summary>
        /// <param name="workLattice"></param>
        /// <param name="encoder"></param>
        /// <returns></returns>
        public SupercellWrapper<IParticle> Translate(WorkLattice workLattice, UnitCellVectorEncoder encoder)
        {

            IParticle[][] particlesLineratized = new IParticle[workLattice.WorkCells.GetLength(0)*workLattice.WorkCells.GetLength(1)*workLattice.WorkCells.GetLength(2)][];

            int counter = 0;
            foreach (var item in workLattice.WorkCells)
            {
                IParticle[] entries = new IParticle[item.CellEntries.Length];
                for (int p = 0; p < item.CellEntries.Length; p++)
                {
                    entries[p] = item.CellEntries[p].Particle;
                }
                particlesLineratized[counter] = entries;
            }

            var particlesMultiDim = new IParticle[workLattice.WorkCells.GetLength(0), workLattice.WorkCells.GetLength(1), workLattice.WorkCells.GetLength(2)][].Populate(particlesLineratized);

            return new SupercellWrapper<IParticle>(particlesMultiDim, encoder);
        }

    }
}
