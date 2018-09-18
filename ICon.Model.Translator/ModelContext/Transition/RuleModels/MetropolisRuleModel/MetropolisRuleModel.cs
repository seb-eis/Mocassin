using ICon.Model.Particles;
using ICon.Model.Transitions;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace ICon.Model.Translator.ModelContext
{
    /// <summary>
    /// Metropolis specific transition rule model that fully describes the exchange process of an allowed metropolis transition
    /// </summary>
    public class MetropolisRuleModel : TransitionRuleModel, IMetropolisRuleModel
    {
        /// <summary>
        /// Boolean flag if the inverse rule model is set
        /// </summary>
        public bool InverseIsSet => InverseRuleModel != null;

        /// <summary>
        /// Boolean flag that indicates that this rule model describes in inverted version of its source rule and abstract transition
        /// </summary>
        public bool IsSourceInversion { get; set; }

        /// <summary>
        /// The abstract transition the rule model is based upon
        /// </summary>
        public override IAbstractTransition AbstractTransition => MetropolisRule.AbstractTransition;

        /// <summary>
        /// The selectable particle the rule describes
        /// </summary>
        public override IParticle SelectableParticle => MetropolisRule.SelectableParticle;

        /// <summary>
        /// The kinetic transition rule this model is based upon
        /// </summary>
        public IMetropolisRule MetropolisRule { get; set; }

        /// <summary>
        /// The rule model that describes the neutralizing inverted sister rule
        /// </summary>
        public IMetropolisRuleModel InverseRuleModel { get; set; }

        /// <summary>
        /// Tries to link the passed rule model and this one as inverses. Returns flase if theiy do not describe inverse rule cases
        /// </summary>
        /// <param name="ruleModel"></param>
        public bool LinkIfInverseMatch(IMetropolisRuleModel ruleModel)
        {
            if (StartStateCode == ruleModel.FinalStateCode && MetropolisRule == ruleModel.MetropolisRule)
            {
                InverseRuleModel = ruleModel;
                ruleModel.InverseRuleModel = this;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Creates an inverse version of this rule model
        /// </summary>
        /// <returns></returns>
        public IMetropolisRuleModel CreateInverse()
        {
            var inverseModel = new MetropolisRuleModel()
            {
                IsSourceInversion = true,
                InverseRuleModel = this,
                MetropolisRule = MetropolisRule
            };

            CopyInversionDataToModel(inverseModel);

            return inverseModel;
        }
    }
}
