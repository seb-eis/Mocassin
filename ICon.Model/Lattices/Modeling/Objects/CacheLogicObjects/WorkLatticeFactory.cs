using ICon.Framework.Collections;
using ICon.Mathematics.ValueTypes;
using ICon.Model.Particles;
using ICon.Model.Structures;
using ICon.Symmetry.Analysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Model.Lattices
{
    public class WorkLatticeFactory
    {

        public WorkLattice Fabricate(ReadOnlyList<IUnitCell<IParticle>> buildingBlocks, ReadOnlyList<IBlockInfo> blockInfos, IReadOnlyDictionary<int, IUnitCellPosition> sublatticeIDs, DataIntegralVector3D latticeSize)
        {
            if (buildingBlocks.Count != blockInfos.Count )
            {
                throw new ArgumentException("WorkLatticeFactory", "Number of Building Blocks and Block Infos is not the same");
            }

            if (!blockInfos[0].Origin.Equals(new DataIntegralVector3D(0,0,0)))
            {
                throw new ArgumentException("WorkLatticeFactory", "Extent of default building block does not originate at 0, 0, 0");
            }

            if (!blockInfos[0].Extent.Equals(latticeSize))
            {
                throw new ArgumentException("WorkLatticeFactory", "Default block has smaller extent than lattice size");
            }

            WorkLattice workLattice = new WorkLattice() { WorkCells = new WorkCell[latticeSize.A, latticeSize.B, latticeSize.C] };

            for (int i = 0; i < buildingBlocks.Count; i++)
            {
                for (int x = blockInfos[i].Origin.A; x < blockInfos[i].Extent.A; x++)
                {
                    for (int y = blockInfos[i].Origin.B; y < blockInfos[i].Extent.B; y++)
                    {
                        for (int z = blockInfos[i].Origin.B; z < blockInfos[i].Extent.C; z++)
                        {
                            workLattice.WorkCells[x, y, z] = (new WorkCellFactory()).Fabricate(buildingBlocks[i], sublatticeIDs, (i != 0));
                        }
                    }
                }
            }

            return workLattice;
        }

    }
}
