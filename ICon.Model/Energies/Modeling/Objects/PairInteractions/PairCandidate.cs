using Mocassin.Mathematics.ValueTypes;
using Mocassin.Model.Structures;

namespace Mocassin.Model.Energies
{
    /// <summary>
    ///     Raw pair interaction candidate struct that is used to compare and assign the indexing to the raw pair interactions
    /// </summary>
    public class PairCandidate
    {
        /// <summary>
        ///     The first unit cell position
        /// </summary>
        public ICellReferencePosition Position0 { get; }

        /// <summary>
        ///     The second unit cell position
        /// </summary>
        public ICellReferencePosition Position1 { get; }

        /// <summary>
        ///     The actual position vector of the second position
        /// </summary>
        public Fractional3D PositionVector { get; }

        /// <summary>
        ///     The distance value of the interaction in internal units
        /// </summary>
        public double Distance { get; }

        /// <summary>
        ///     The assigned index value
        /// </summary>
        public int Index { get; }

        /// <summary>
        ///     Creates new raw pair interaction where the index is set to -1
        /// </summary>
        /// <param name="position0"></param>
        /// <param name="position1"></param>
        /// <param name="positionVector"></param>
        /// <param name="distance"></param>
        public PairCandidate(ICellReferencePosition position0, ICellReferencePosition position1, Fractional3D positionVector, double distance)
        {
            Position0 = position0;
            Position1 = position1;
            PositionVector = positionVector;
            Distance = distance;
            Index = -1;
        }

        /// <summary>
        ///     Creates new raw pair interaction
        /// </summary>
        /// <param name="position0"></param>
        /// <param name="position1"></param>
        /// <param name="positionVector"></param>
        /// <param name="distance"></param>
        /// <param name="index"></param>
        public PairCandidate(ICellReferencePosition position0, ICellReferencePosition position1, Fractional3D positionVector, double distance,
            int index)
        {
            Position0 = position0;
            Position1 = position1;
            PositionVector = positionVector;
            Distance = distance;
            Index = index;
        }

        /// <summary>
        ///     Creates a copy of the current value with a new index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public PairCandidate CopyWithNewIndex(int index)
        {
            return new PairCandidate(Position0, Position1, PositionVector, Distance, index);
        }
    }
}