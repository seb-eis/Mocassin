namespace Mocassin.Model.Translator.ModelContext
{
    /// <summary>
    ///     Range model that describes the interaction range of regular positions
    /// </summary>
    public interface IInteractionRangeModel
    {
        /// <summary>
        ///     The interaction range in internal units
        /// </summary>
        double InteractionRange { get; set; }

        /// <summary>
        ///     The interaction range in cells (rounded up) in the A direction
        /// </summary>
        int CellsInDirectionA { get; set; }

        /// <summary>
        ///     The interaction range in cells (rounded up) in the A direction
        /// </summary>
        int CellsInDirectionB { get; set; }

        /// <summary>
        ///     The interaction range in cells (rounded up) in the A direction
        /// </summary>
        int CellsInDirectionC { get; set; }
    }
}