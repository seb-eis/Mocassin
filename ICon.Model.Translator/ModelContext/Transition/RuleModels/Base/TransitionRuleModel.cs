using System;
using System.Collections.Generic;
using System.Linq;
using Mocassin.Framework.Collections;
using Mocassin.Model.Particles;
using Mocassin.Model.Transitions;

namespace Mocassin.Model.Translator.ModelContext
{
    /// <inheritdoc />
    public abstract class TransitionRuleModel : ITransitionRuleModel
    {
        /// <inheritdoc />
        public abstract bool InverseIsSet { get; }

        /// <inheritdoc />
        public bool IsSourceInversion { get; set; }

        /// <inheritdoc />
        public abstract IAbstractTransition AbstractTransition { get; }

        /// <inheritdoc />
        public abstract IParticle SelectableParticle { get; }

        /// <inheritdoc />
        public IList<IParticle> StartState { get; set; }

        /// <inheritdoc />
        public IList<IParticle> FinalState { get; set; }

        /// <inheritdoc />
        public IList<int> EndIndexingDeltas { get; set; }

        /// <inheritdoc />
        public abstract double AttemptFrequency { get; }

        /// <inheritdoc />
        public long StartStateCode { get; set; }

        /// <inheritdoc />
        public abstract long TransitionStateCode { get; set; }

        /// <inheritdoc />
        public long FinalStateCode { get; set; }

        /// <inheritdoc />
        public long FinalTrackerOrderCode { get; set; }

        /// <inheritdoc />
        public void CopyInversionDataToModel(ITransitionRuleModel ruleModel)
        {
            ruleModel.FinalState = FinalState.Reverse().ToList();
            ruleModel.StartState = StartState.Reverse().ToList();
            ruleModel.EndIndexingDeltas = GetEndIndexingDeltaInversion();
            ruleModel.FinalStateCode = MakeInvertedStateCode(FinalState);
            ruleModel.StartStateCode = MakeInvertedStateCode(StartState);
            ruleModel.FinalTrackerOrderCode = CreateInvertedTrackerOrderCode(ruleModel.EndIndexingDeltas);
        }

        /// <inheritdoc />
        public abstract bool LinkIfLogicalInversions(ITransitionRuleModel ruleModel);

        /// <inheritdoc />
        public abstract bool IsLogicalInverse(ITransitionRuleModel ruleModel);

        /// <inheritdoc />
        public IParticleSet GetMobileParticles()
        {
            var comparer = Comparer<IParticle>.Create((a, b) => a.Index.CompareTo(b.Index));
            var setList = new SetList<IParticle>(comparer);

            for (var pathId = 0; pathId < StartState.Count; pathId++)
            {
                if (StartState[pathId].Index == FinalState[pathId].Index)
                    continue;

                setList.Add(StartState[pathId]);
                setList.Add(FinalState[pathId]);
            }

            return new ParticleSet {Particles = setList.ToList()};
        }

        /// <summary>
        ///     Creates the inverted version of end indexing deltas
        /// </summary>
        /// <returns></returns>
        protected IList<int> GetEndIndexingDeltaInversion()
        {
            var result = EndIndexingDeltas.Reverse().Select(a => -a).ToList();
            return result;
        }

        /// <summary>
        ///     Creates the inverted order code by code arithmetic (TARGET_ORDER = SOURCE_ORDER - SOURCE_EIDX + TARGET_EIDX)
        /// </summary>
        /// <param name="invertedEndIndexingDeltas"></param>
        /// <returns></returns>
        protected long CreateInvertedTrackerOrderCode(IList<int> invertedEndIndexingDeltas)
        {
            var bytes = BitConverter.GetBytes(FinalTrackerOrderCode);

            for (var i = 0; i < invertedEndIndexingDeltas.Count; i++)
                bytes[i] -= (byte) (EndIndexingDeltas[i] - invertedEndIndexingDeltas[i]);

            return BitConverter.ToInt64(bytes, 0);
        }

        /// <summary>
        ///     Creates an inverted state code for the passed <see cref="IParticle" /> sequence
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