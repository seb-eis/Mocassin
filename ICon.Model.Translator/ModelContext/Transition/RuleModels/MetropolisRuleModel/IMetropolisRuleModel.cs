using Mocassin.Model.Transitions;

namespace Mocassin.Model.Translator.ModelContext
{
    /// <summary>
    ///     Transition rule model for metropolis transitions. Extends the basic rule model by metropolis specific data context
    ///     options
    /// </summary>
    public interface IMetropolisRuleModel : ITransitionRuleModel
    {
        /// <summary>
        ///     The kinetic transition rule this model is based upon
        /// </summary>
        IMetropolisRule MetropolisRule { get; set; }

        /// <summary>
        ///     The rule model that describes the neutralizing inverted sister rule
        /// </summary>
        IMetropolisRuleModel InverseRuleModel { get; set; }

        /// <summary>
        ///     Creates a raw inverted version of this rule model
        /// </summary>
        /// <returns></returns>
        IMetropolisRuleModel CreateInverse();
    }
}