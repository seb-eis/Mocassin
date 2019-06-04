using Mocassin.Framework.Extensions;
using Mocassin.Model.Particles;
using Mocassin.Model.Transitions;

namespace Mocassin.Model.Translator.ModelContext
{
    /// <inheritdoc cref="IMetropolisRuleModel" />
    public class MetropolisRuleModel : TransitionRuleModel, IMetropolisRuleModel
    {
        /// <inheritdoc />
        public override bool InverseIsSet => InverseRuleModel != null;

        /// <inheritdoc />
        public override IAbstractTransition AbstractTransition => MetropolisRule.AbstractTransition;

        /// <inheritdoc />
        public override IParticle SelectableParticle => MetropolisRule.SelectableParticle;

        /// <inheritdoc />
        public override double AttemptFrequency => 0;

        /// <inheritdoc />
        public override long TransitionStateCode { get; set; }

        /// <inheritdoc />
        public IMetropolisRule MetropolisRule { get; set; }

        /// <inheritdoc />
        public IMetropolisRuleModel InverseRuleModel { get; set; }

        /// <inheritdoc />
        public override bool LinkIfLogicalInversions(ITransitionRuleModel ruleModel)
        {
            if (!IsLogicalInverse(ruleModel))
                return false;

            var inverseRuleModel = (IMetropolisRuleModel) ruleModel;
            InverseRuleModel = inverseRuleModel;
            inverseRuleModel.InverseRuleModel = this;
            return true;
        }

        /// <inheritdoc />
        public override bool IsLogicalInverse(ITransitionRuleModel ruleModel)
        {
            if (!(ruleModel is IMetropolisRuleModel inverseRuleModel))
                return false;

            if (inverseRuleModel.AbstractTransition != AbstractTransition)
                return false;

            return StartState.LexicographicCompare(inverseRuleModel.FinalState) == 0 &&
                   FinalState.LexicographicCompare(inverseRuleModel.StartState) == 0;
        }

        /// <inheritdoc />
        public IMetropolisRuleModel CreateGeometricInversion()
        {
            var inverseModel = new MetropolisRuleModel
            {
                IsSourceInversion = true,
                MetropolisRule = MetropolisRule
            };

            CopyInversionDataToModel(inverseModel);
            return inverseModel;
        }
    }
}