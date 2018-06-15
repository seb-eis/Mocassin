using ICon.Mathematics.ValueTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace ICon.Model.Lattices
{
    class BlockedSuperCell : IEnumerable<BuildingBlock>
    {
        public List<BuildingBlock> Lattice { get; set; }

        public Coordinates<int, int, int> LatticeSize { get; set; }

        BlockedSuperCell(BuildingBlock defaultBlock, Coordinates<int,int,int> latticeSize)
        {
            LatticeSize = latticeSize;
            Lattice = new List<BuildingBlock>();
            for (int i = 0; i < LatticeSize.A*LatticeSize.B*LatticeSize.C; i++)
            {
                Lattice.Add(defaultBlock);
            }
        }

        public BuildingBlock GetBuildingBlock(int x, int y, int z)
        {

            return Lattice[x, y, z];
        }

        public IEnumerator<BuildingBlock> GetEnumerator()
        {
            return Lattice.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
