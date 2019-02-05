using Mocassin.Model.Particles;
using Mocassin.Symmetry.Analysis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Mocassin.Framework.Collections;
using Mocassin.Mathematics.ValueTypes;
using Mocassin.Model.Structures;
using Moccasin.Mathematics.ValueTypes;

namespace Mocassin.Model.Lattices
{
    /// <summary>
    /// Produces WorkLattices for supercell creation
    /// </summary>
    public class WorkLatticeFactory
    {
        /// <summary>
        /// Fabricates the WorkLattice
        /// </summary>
        /// <param name="buildingBlocks"></param>
        /// <param name="blockInfos"></param>
        /// <param name="sublatticeIDs"></param>
        /// <param name="latticeSize"></param>
        /// <returns></returns>
        public WorkLattice Fabricate(ReadOnlyListAdapter<IBuildingBlock> buildingBlocks, ReadOnlyListAdapter<IBlockInfo> blockInfos, IReadOnlyDictionary<int, IUnitCellPosition> sublatticeIDs, VectorInt3D latticeSize)
        {
            if (!blockInfos.Single(x => x.Index == 0).Origin.Equals(new VectorInt3D(0,0,0)))
            {
                throw new ArgumentException("WorkLatticeFactory", "Extent of default building block does not originate at 0, 0, 0");
            }

            if (!blockInfos.Single(x => x.Index == 0).Extent.Equals(latticeSize))
            {
                throw new ArgumentException("WorkLatticeFactory", "Default block has different extent than lattice size");
            }

            WorkLattice workLattice = new WorkLattice() { WorkCells = new WorkCell[latticeSize.A, latticeSize.B, latticeSize.C] };

            foreach(var blockInfo in blockInfos)
            {
                for (int x = blockInfo.Origin.A; x < blockInfo.Extent.A; x++)
                {
                    for (int y = blockInfo.Origin.B; y < blockInfo.Extent.B; y++)
                    {
                        for (int z = blockInfo.Origin.B; z < blockInfo.Extent.C; z++)
                        {
                            workLattice.WorkCells[x, y, z] = (new WorkCellFactory()).Fabricate(blockInfo.BlockGrouping[x+y+z], sublatticeIDs, (blockInfo.Index != 0));
                        }
                    }
                }
            }

            return workLattice;
        }

    }
}
