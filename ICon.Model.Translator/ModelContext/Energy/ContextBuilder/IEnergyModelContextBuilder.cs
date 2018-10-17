
namespace Mocassin.Model.Translator.ModelContext
{
    /// <summary>
    /// Energy model context builder. Extends the energy reference information of a project into a full data context
    /// </summary>
    public interface IEnergyModelContextBuilder : IModelContextBuilder<IEnergyModelContext>
    {
        /// <summary>
        /// The builder instance for group energy models
        /// </summary>
        IGroupEnergyModelBuilder GroupEnergyModelBuilder { get; set; }

        /// <summary>
        /// The builder instance for pair energy models
        /// </summary>
        IPairEnergyModelBuilder PairEnergyModelBuilder { get; set; }
    }
}