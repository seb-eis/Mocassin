using ICon.Model.Particles;
using ICon.Model.Transitions;

namespace ICon.Model.Translator.ModelContext
{
    /// <inheritdoc cref="ICon.Model.Translator.ModelContext.IMetropolisRuleModel"/>
    public class MetropolisRuleModel : TransitionRuleModel, IMetropolisRuleModel
    {
        /// <inheritdoc />
        public override bool InverseIsSet => InverseRuleModel != null;

        /// <inheritdoc />
        public override IAbstractTransition AbstractTransition => MetropolisRule.AbstractTransition;

        /// <inheritdoc />
        public override IParticle SelectableParticle => MetropolisRule.SelectableParticle;

        /// <inheritdoc />
        public IMetropolisRule MetropolisRule { get; set; }

        /// <inheritdoc />
        public IMetropolisRuleModel InverseRuleModel { get; set; }

        /// <inheritdoc />
        public override bool LinkIfInverseMatch(ITransitionRuleModel ruleModel)
        {
            if (!IsInverse(ruleModel)) return false;

            var inverseRuleModel = (IMetropolisRuleModel) ruleModel;
            InverseRuleModel = inverseRuleModel;
            inverseRuleModel.InverseRuleModel = this;
            return true;
        }

        /// <inheritdoc />
        public override bool IsInverse(ITransitionRuleModel ruleModel)
        {
            if (!(ruleModel is IMetropolisRuleModel inverseRuleModel)) return false;
            return StartStateCode == inverseRuleModel.FinalStateCode && MetropolisRule == inverseRuleModel.MetropolisRule;
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
