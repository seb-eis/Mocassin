using Mocassin.Model.Structures;

namespace Mocassin.Model.Energies
{
    /// <summary>
    ///     General interface for interaction filters used to customize interaction filtering during pair interaction searches
    /// </summary>
    public interface IInteractionFilter
    {
        /// <summary>
        ///     Get the unit cell position of the center
        /// </summary>
        IUnitCellPosition CenterUnitCellPosition { get; }

        /// <summary>
        ///     Get the unit cell position of the partner
        /// </summary>
        IUnitCellPosition PartnerUnitCellPosition { get; }

        /// <summary>
        ///     Get  the start radius for the filter
        /// </summary>
        double StartRadius { get; }

        /// <summary>
        ///     Get the end radius for the filter
        /// </summary>
        double EndRadius { get; }

        /// <summary>
        ///     Check if the passed interaction properties fall within the constraints of the interaction filter
        /// </summary>
        /// <param name="distance"></param>
        /// <param name="centerUnitCellPosition"></param>
        /// <param name="partnerUnitCellPosition"></param>
        /// <returns></returns>
        bool IsApplicable(double distance, IUnitCellPosition centerUnitCellPosition, IUnitCellPosition partnerUnitCellPosition);

        /// <summary>
        /// Check if the passed pair interaction falls within the constraints of the interaction filter
        /// </summary>
        /// <param name="pairInteraction"></param>
        /// <returns></returns>
        bool IsApplicable(IPairInteraction pairInteraction);

        /// <summary>
        ///     Check if the filter behaves equally to another filter interface
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        bool IsEqualFilter(IInteractionFilter other);
    }
}