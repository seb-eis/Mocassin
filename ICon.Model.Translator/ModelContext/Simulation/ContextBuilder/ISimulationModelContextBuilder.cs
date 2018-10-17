namespace Mocassin.Model.Translator.ModelContext
{
    /// <summary>
    /// Simulation model context builder. Extends simulation reference information of a project into a full data context
    /// </summary>
    public interface ISimulationModelContextBuilder : IModelContextBuilder<ISimulationModelContext>
    {
        /// <summary>
        /// Get or set the builder instance for metropolis simulation models
        /// </summary>
        IMetropolisSimulationModelBuilder MetropolisSimulationModelBuilder { get; set; }

        /// <summary>
        /// Get or set the builder instance for kinetic simulation models
        /// </summary>
        IKineticSimulationModelBuilder KineticSimulationModelBuilder {get; set; }
    }
}