namespace Mocassin.Model.Translator.ModelContext
{
    /// <inheritdoc />
    public class InteractionRangeModel : IInteractionRangeModel
    {
        /// <inheritdoc />
        public double InteractionRange { get; set; }

        /// <inheritdoc />
        public int CellsInDirectionA { get; set; }

        /// <inheritdoc />
        public int CellsInDirectionB { get; set; }

        /// <inheritdoc />
        public int CellsInDirectionC { get; set; }
    }
}