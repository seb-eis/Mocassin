namespace Mocassin.Model.Translator.ModelContext
{
    /// <summary>
    ///     Structure model context builder. Extends structural reference information of a project into a full data context
    /// </summary>
    public interface IStructureModelContextBuilder : IModelContextBuilder<IStructureModelContext>
    {
        /// <summary>
        ///     The environment model builder that is used by the context builder
        /// </summary>
        IEnvironmentModelBuilder EnvironmentModelBuilder { get; set; }

        /// <summary>
        ///     The position model builder that is used by the context
        /// </summary>
        IPositionModelBuilder PositionModelBuilder { get; set; }
    }
}