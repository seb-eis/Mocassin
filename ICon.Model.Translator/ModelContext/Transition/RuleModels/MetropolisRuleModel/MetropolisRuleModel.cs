using ICon.Model.Particles;
using ICon.Model.Transitions;

namespace ICon.Model.Translator.ModelContext
{
    /// <inheritdoc />
    public class MetropolisRuleModel : TransitionRuleModel, IMetropolisRuleModel
    {
        /// <inheritdoc />
        public bool InverseIsSet => InverseRuleModel != null;

        /// <inheritdoc />
        public override IAbstractTransition AbstractTransition => MetropolisRule.AbstractTransition;

        /// <inheritdoc />
        public override IParticle SelectableParticle => MetropolisRule.SelectableParticle;

        /// <inheritdoc />
        public IMetropolisRule MetropolisRule { get; set; }

        /// <inheritdoc />
        public IMetropolisRuleModel InverseRuleModel { get; set; }

        /// <inheritdoc />
        public bool LinkIfInverseMatch(IMetropolisRuleModel ruleModel)
        {
            if (StartStateCode != ruleModel.FinalStateCode || MetropolisRule != ruleModel.MetropolisRule)
            {
                return false;
            }

            InverseRuleModel = ruleModel;
            ruleModel.InverseRuleModel = this;
            return true;
        }

        /// <inheritdoc />
        public IMetropolisRuleModel CreateInverse()
        {
            var inverseModel = new MetropolisRuleModel
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
