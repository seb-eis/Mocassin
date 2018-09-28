﻿using ICon.Model.Particles;
using ICon.Model.Transitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICon.Framework.Extensions;
using ICon.Mathematics.Extensions;
using ICon.Mathematics.ValueTypes;

namespace ICon.Model.Translator.ModelContext
{
    /// <inheritdoc cref="ICon.Model.Translator.ModelContext.IKineticRuleModel"/>
    public class KineticRuleModel : TransitionRuleModel, IKineticRuleModel
    {
        /// <inheritdoc />
        public override IParticle SelectableParticle => KineticRule.SelectableParticle;

        /// <inheritdoc />
        public override bool InverseIsSet => InverseRuleModel != null;

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
        public Matrix2D ChargeTransportMatrix { get; set; }

        /// <inheritdoc />
        public IKineticRuleModel CreateInverse()
        {
            var inverseModel = new KineticRuleModel
            {
                IsSourceInversion = true,
                InverseRuleModel = this,
                TransitionState = TransitionState.Reverse().ToList(),
                TransitionStateCode = TransitionStateCode.InvertBytes(),
                ChargeTransportMatrix = GetInvertedTransportMatrix(),
                KineticRule = KineticRule
            };
            CopyInversionDataToModel(inverseModel);
            return inverseModel;
        }

        /// <inheritdoc />
        public override bool LinkIfInverseMatch(ITransitionRuleModel ruleModel)
        {
            if (!IsInverse(ruleModel))
                return false;

            var inverseRuleModel = (IKineticRuleModel) ruleModel;
            InverseRuleModel = inverseRuleModel;
            inverseRuleModel.InverseRuleModel = this;
            return true;
        }

        /// <inheritdoc />
        public override bool IsInverse(ITransitionRuleModel ruleModel)
        {
            if (!(ruleModel is IKineticRuleModel inverseRuleModel)) 
                return false;

            if (inverseRuleModel.AbstractTransition != AbstractTransition) 
                return false;

            return StartState.LexicographicCompare(inverseRuleModel.FinalState) == 0 &&
                   FinalState.LexicographicCompare(inverseRuleModel.StartState) == 0;
        }

        /// <summary>
        /// Inverts the charge transport matrix
        /// </summary>
        /// <returns></returns>
        protected Matrix2D GetInvertedTransportMatrix()
        {
            var result = new Matrix2D(ChargeTransportMatrix.Rows, ChargeTransportMatrix.Cols, ChargeTransportMatrix.Comparer);

            for (var i = 0; i < result.Cols; i++)
            {
                result[0, i] = ChargeTransportMatrix[0, result.Cols - i - 1];
            }

            return result;
        }
    }
}
