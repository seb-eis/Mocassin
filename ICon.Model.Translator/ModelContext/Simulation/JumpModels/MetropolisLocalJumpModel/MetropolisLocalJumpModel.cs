namespace Mocassin.Model.Translator.ModelContext
{
    /// <inheritdoc cref="Mocassin.Model.Translator.ModelContext.IMetropolisLocalJumpModel"/>
    public class MetropolisLocalJumpModel : ModelComponentBase, IMetropolisLocalJumpModel
    {
        /// <inheritdoc />
        public IMetropolisMappingModel MappingModel { get; set; }

        /// <inheritdoc />
        public IMetropolisRuleModel RuleModel { get; set; }
    }
}