using ICon.Mathematics.Coordinates;
using ICon.Model.Particles;
using ICon.Symmetry.Analysis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

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

            IParticle[,,][] particles = new IParticle[workLattice.WorkCells.GetLength(0), workLattice.WorkCells.GetLength(1), workLattice.WorkCells.GetLength(2)][];

            for (int x = 0; x < workLattice.WorkCells.GetLength(0); x++)
            {
                for (int y = 0; y < workLattice.WorkCells.GetLength(1); y++)
                {
                    for (int z = 0; z < workLattice.WorkCells.GetLength(2); z++)
                    {
                        IParticle[] entries = new IParticle[workLattice.WorkCells[x, y, z].CellEntries.Length];
                        for (int p = 0; p < workLattice.WorkCells[x, y, z].CellEntries.Length; p++)
                        {
                            entries[p] = workLattice.WorkCells[x, y, z].CellEntries[p].Particle;
                        }
                        particles[x, y, z] = entries;
                    }
                }
            }

            return new SupercellWrapper<IParticle>(particles, encoder);
        }

    }
}
