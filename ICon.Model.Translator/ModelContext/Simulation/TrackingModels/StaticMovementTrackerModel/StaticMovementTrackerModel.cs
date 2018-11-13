namespace Mocassin.Model.Translator.ModelContext
{
    /// <inheritdoc cref="Mocassin.Model.Translator.ModelContext.IStaticMovementTrackerModel" />
    public class StaticMovementTrackerModel : MovementTrackerModel, IStaticMovementTrackerModel
    {
        /// <inheritdoc />
        public int TrackedPositionIndex { get; set; }
    }
}