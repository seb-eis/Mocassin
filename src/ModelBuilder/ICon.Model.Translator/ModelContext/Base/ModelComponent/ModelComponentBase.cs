namespace Mocassin.Model.Translator.ModelContext
{
    /// <summary>
    ///     Abstract base class for model component implementations that form the model data context
    /// </summary>
    public abstract class ModelComponentBase : IModelComponent
    {
        /// <inheritdoc />
        public int ModelId { get; set; }
    }
}