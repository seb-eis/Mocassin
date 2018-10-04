namespace ICon.Model.Translator.ModelContext
{
    /// <summary>
    /// Transition model context builder. Extends transition reference information of a project into a full data context
    /// </summary>
    public interface ITransitionModelContextBuilder : IModelContextBuilder<ITransitionModelContext>
    {
        /// <summary>
        /// The model builder for the metropolis transition model collection
        /// </summary>
        IMetropolisTransitionModelBuilder MetropolisTransitionModelBuilder { get; set; }

        /// <summary>
        /// The model builder for the kinetic transition model collection
        /// </summary>
        IKineticTransitionModelBuilder KineticTransitionModelBuilder { get; set; }

        /// <summary>
        /// The model builder for the position transition model collection
        /// </summary>
         IPositionTransitionModelBuilder PositionTransitionModelBuilder { get; set; }
    }
}