using ICon.Model.Particles;
using ICon.Model.Structures;
using ICon.Symmetry.Analysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Model.Lattices
{
    /// <summary>
    /// Produces WorkCells for the WorkLattice
    /// </summary>
    public class WorkCellFactory
    {
        /// <summary>
        /// Fabricate the WorkCell
        /// </summary>
        /// <param name="unitCellWrapper"></param>
        /// <param name="sublatticeIDs"></param>
        /// <param name="isCustom"></param>
        /// <returns></returns>
        public WorkCell Fabricate(IUnitCell<IParticle> unitCellWrapper, IReadOnlyDictionary<int, IUnitCellPosition> sublatticeIDs, bool isCustom)
        {

            IParticle[] particles = GenerateListOfParticles(unitCellWrapper);

            if (particles.Length != sublatticeIDs.Count)
            {
                throw new ArgumentException("WorkCellFactory", "Different number of unitCellWrapper entries and sublatticeID entries!");
            }

            WorkCell workCell = new WorkCell { IsCustom = isCustom, CellEntries = new CellEntry[particles.Length] };

            for (int i = 0; i < particles.Length; i++)
            {
                workCell.CellEntries[i] = new CellEntry { Particle = particles[i], CellPosition = sublatticeIDs[i] };
            }

            return workCell;
        }

        /// <summary>
        /// Translates the UnitCell to an array of particles
        /// </summary>
        /// <param name="unitCell"></param>
        /// <returns></returns>
        protected IParticle[] GenerateListOfParticles(IUnitCell<IParticle> unitCell)
        {
            IParticle[] particles = new IParticle[unitCell.EntryCount];

            int counter = 0;
            foreach (var entry in unitCell.GetAllEntries())
            {
                particles[counter] = entry.Entry;
                counter++;
            }

            return particles;
        }
    }
}
