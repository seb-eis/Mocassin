using ICon.Model.Particles;
using ICon.Model.Transitions;
using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Model.Translator.ModelContext
{
    /// <inheritdoc cref="ICon.Model.Translator.ModelContext.IKineticRuleModel"/>
    public class KineticRuleModel : TransitionRuleModel, IKineticRuleModel
    {
        /// <inheritdoc />
        public override IParticle SelectableParticle => KineticRule.SelectableParticle;

        /// <inheritdoc />
        public override IAbstractTransition AbstractTransition => KineticRule.AbstractTransition;

        /// <inheritdoc />
        public IKineticRule KineticRule { get; set; }

        /// <inheritdoc />
        public IKineticRuleModel InverseRuleModel { get; set; }

        /// <inheritdoc />
        public IList<IParticle> TransitionState { get; set; }

        /// <inheritdoc />
        public long TransitionStateCode { get; set; }

        /// <inheritdoc />
        public double EffectiveTransportCharge { get; set; }
    }
}
