using ICon.Model.Transitions;
using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Model.Translator.ModelContext
{
    /// <summary>
    /// Transition rule model for metropolis transitions that extends the basic transition rule model
    /// </summary>
    public interface IMetropolisRuleModel : ITransitionRuleModel
    {
        /// <summary>
        /// Boolean flag if the inverse rule model is set
        /// </summary>
        bool InverseIsSet { get; }

        /// <summary>
        /// The kinetic transition rule this model is based upon
        /// </summary>
        IMetropolisRule MetropolisRule { get; set; }

        /// <summary>
        /// The rule model that describes the neutralizing inverted sister rule
        /// </summary>
        IMetropolisRuleModel InverseRuleModel { get; set; }

        /// <summary>
        /// Tries to link the passed rule model and this one as inverses. Returns flase if theiy do not describe inverse rule cases
        /// </summary>
        /// <param name="ruleModel"></param>
        bool LinkIfInverseMatch(IMetropolisRuleModel ruleModel);

        /// <summary>
        /// Creates an inverse version of this rule model
        /// </summary>
        /// <returns></returns>
        IMetropolisRuleModel CreateInverse();
    }
}
