using System.Collections.Generic;
using Mocassin.Model.Basic;
using Mocassin.Model.Structures;

namespace Mocassin.Model.Energies
{
    /// <summary>
    ///     Pair interaction filter to customize ignored interaction in <see cref="IPairInteractionFinder" /> search routines
    /// </summary>
    public class InteractionFilter
    {
        /// <summary>
        ///     Get or set the unit cell position that should be ignored
        /// </summary>
        [UseTrackedReferences]
        public IUnitCellPosition UnitCellPosition { get; set; }

        /// <summary>
        ///     Get or set the start radius for the filter
        /// </summary>
        public double StartRadius { get; set; }

        /// <summary>
        ///     Get or set the end radius for the filter
        /// </summary>
        public double EndRadius { get; set; }

        /// <summary>
        ///     Check if the passed interaction information falls within the filter constraints using the provided double comparer
        /// </summary>
        /// <returns></returns>
        public bool CanBeFiltered(double radius, IUnitCellPosition targetUnitCellPosition)
        {
            if (targetUnitCellPosition.Index == UnitCellPosition.Index)
                return true;

            return radius >= StartRadius && radius <= EndRadius;
        }
    }
}