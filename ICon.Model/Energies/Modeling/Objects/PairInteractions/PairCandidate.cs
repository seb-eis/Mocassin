using System;
using System.Collections.Generic;
using System.Text;

using ICon.Mathematics.ValueTypes;
using ICon.Model.Structures;

namespace ICon.Model.Energies
{
    /// <summary>
    /// Raw pair interaction candidate struct that is used to compare and assign the indexing to the rwa pair interactions
    /// </summary>
    public readonly struct PairCandidate
    {
        /// <summary>
        /// The first unit cell position
        /// </summary>
        public IUnitCellPosition Position0 { get; }

        /// <summary>
        /// The second unit cell position
        /// </summary>
        public IUnitCellPosition Position1 { get; }

        /// <summary>
        /// The actaul position vector of the second position
        /// </summary>
        public Fractional3D PositionVector { get; }

        /// <summary>
        /// The distance value of the interaction in internal units
        /// </summary>
        public double Distance { get; }

        /// <summary>
        /// The assigned index value
        /// </summary>
        public int Index { get; }

        /// <summary>
        /// Creates new raw pair interaction where the index is set to -1
        /// </summary>
        /// <param name="position0"></param>
        /// <param name="position1"></param>
        /// <param name="positionVector"></param>
        /// <param name="distance"></param>
        public PairCandidate(IUnitCellPosition position0, IUnitCellPosition position1, Fractional3D positionVector, double distance) : this()
        {
            Position0 = position0;
            Position1 = position1;
            PositionVector = positionVector;
            Distance = distance;
            Index = -1;
        }

        /// <summary>
        /// Creates new raw pair interaction
        /// </summary>
        /// <param name="position0"></param>
        /// <param name="position1"></param>
        /// <param name="positionVector"></param>
        /// <param name="distance"></param>
        public PairCandidate(IUnitCellPosition position0, IUnitCellPosition position1, Fractional3D positionVector, double distance, int index) : this()
        {
            Position0 = position0;
            Position1 = position1;
            PositionVector = positionVector;
            Distance = distance;
            Index = index;
        }

        /// <summary>
        /// Creates a copy of the current value with a new index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public PairCandidate CopyWithNewIndex(int index)
        {
            return new PairCandidate(Position0, Position1, PositionVector, Distance, index);
        }
    }
}
