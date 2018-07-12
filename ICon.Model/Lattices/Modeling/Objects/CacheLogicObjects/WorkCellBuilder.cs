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
    public class WorkCellBuilder
    {
        /// <summary>
        /// Build the WorkCell
        /// </summary>
        /// <param name="buildingBlock"></param>
        /// <param name="sublatticeIDs"></param>
        /// <param name="isCustom"></param>
        /// <returns></returns>
        public WorkCell Build(IBuildingBlock buildingBlock, IReadOnlyDictionary<int, IUnitCellPosition> sublatticeIDs, int buildingBlockID)
        {
            if (buildingBlock.CellEntries.Count != sublatticeIDs.Count)
            {
                throw new ArgumentException("WorkCellFactory", "Different number of unitCellWrapper entries and sublatticeID entries!");
            }

            WorkCell workCell = new WorkCell { BuildingBlockID = buildingBlockID, CellEntries = new CellEntry[buildingBlock.CellEntries.Count] };

            for (int i = 0; i < buildingBlock.CellEntries.Count; i++)
            {
                workCell.CellEntries[i] = new CellEntry { Particle = buildingBlock.CellEntries[i], CellPosition = sublatticeIDs[i] };
            }

            return workCell;
        }
    }
}
