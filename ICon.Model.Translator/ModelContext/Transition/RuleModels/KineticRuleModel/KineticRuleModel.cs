﻿using System;
using System.Collections.Generic;
using System.Linq;
using Mocassin.Framework.Extensions;
using Mocassin.Mathematics.Extensions;
using Mocassin.Mathematics.ValueTypes;
using Mocassin.Model.Particles;
using Mocassin.Model.Transitions;

namespace Mocassin.Model.Translator.ModelContext
{
    /// <inheritdoc cref="IKineticRuleModel" />
    public class KineticRuleModel : TransitionRuleModel, IKineticRuleModel
    {
        /// <inheritdoc />
        public override IParticle SelectableParticle => KineticRule.SelectableParticle;

        /// <inheritdoc />
        public override bool InverseIsSet => InverseRuleModel != null;

        /// <inheritdoc />
        public override IAbstractTransition AbstractTransition => KineticRule.AbstractTransition;

        /// <inheritdoc />
        public int RuleDirectionValue { get; set; }

        /// <inheritdoc />
        public IKineticTransitionModel TransitionModel { get; set; }

        /// <inheritdoc />
        public IKineticRule KineticRule { get; set; }

        /// <inheritdoc />
        public IKineticRuleModel InverseRuleModel { get; set; }

        /// <inheritdoc />
        public IList<IParticle> TransitionState { get; set; }

        /// <inheritdoc />
        public override double AttemptFrequency => KineticRule?.AttemptFrequency ?? 0;

        /// <inheritdoc />
        public override long TransitionStateCode { get; set; }

        /// <inheritdoc />
        public Matrix2D ChargeTransportMatrix { get; set; }

        /// <inheritdoc />
        public IKineticRuleModel CreateGeometricInversion()
        {
            var inverseModel = new KineticRuleModel
            {
                RuleDirectionValue = RuleDirectionValue,
                IsSourceInversion = true,
                InverseRuleModel = this,
                TransitionModel = TransitionModel.InverseTransitionModel ?? throw new InvalidOperationException("Inverse transition model is unknown!"),
                TransitionState = TransitionState.Reverse().ToList(),
                TransitionStateCode = MakeInvertedStateCode(TransitionState),
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
        ///     Inverts the charge transport matrix
        /// </summary>
        /// <returns></returns>
        protected Matrix2D GetInvertedTransportMatrix()
        {
            var result = new Matrix2D(ChargeTransportMatrix.Rows, ChargeTransportMatrix.Cols, ChargeTransportMatrix.Comparer);

            for (var i = 0; i < result.Cols; i++) 
                result[0, i] = ChargeTransportMatrix[0, result.Cols - i - 1];

            return result;
        }

        /// <summary>
        ///     Creates an inverted state code for the passed <see cref="IParticle"/> sequence
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        protected long MakeInvertedStateCode(IList<IParticle> state)
        {
            var bytes = new byte[8];
            var index = 0;
            for (var i = state.Count - 1; i >= 0; i--) bytes[index++] = (byte) state[i].Index;

            var result = BitConverter.ToInt64(bytes, 0);
            return result;
        }
    }
}