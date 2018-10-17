using Mocassin.Model.Particles;
using Mocassin.Symmetry.Analysis;
using System;
using System.Collections.Generic;
using System.Text;
using Mocassin.Model.Structures;

namespace Mocassin.Model.Lattices
{
    /// <summary>
    /// Produces WorkCells for the WorkLattice
    /// </summary>
    public class WorkCellFactory
    {
        /// <summary>
        /// Fabricate the WorkCell
        /// </summary>
        /// <param name="buildingBlock"></param>
        /// <param name="sublatticeIDs"></param>
        /// <param name="isCustom"></param>
        /// <returns></returns>
        public WorkCell Fabricate(IBuildingBlock buildingBlock, IReadOnlyDictionary<int, IUnitCellPosition> sublatticeIDs, bool isCustom)
        {
            if (buildingBlock.CellEntries.Count != sublatticeIDs.Count)
            {
                throw new ArgumentException("WorkCellFactory", "Different number of unitCellWrapper entries and sublatticeID entries!");
            }

            WorkCell workCell = new WorkCell { IsCustom = isCustom, CellEntries = new CellEntry[buildingBlock.CellEntries.Count] };

            for (int i = 0; i < buildingBlock.CellEntries.Count; i++)
            {
                workCell.CellEntries[i] = new CellEntry { Particle = buildingBlock.CellEntries[i], CellPosition = sublatticeIDs[i] };
            }

            return workCell;
        }
    }
}
