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
        public abstract double AttemptFrequency { get; set; }

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
            ruleModel.FinalState = StartState.ToList();
            ruleModel.StartState = FinalState.ToList();
            ruleModel.EndIndexingDeltas = GetEndIndexingDeltaInversion();
            ruleModel.FinalStateCode = StartStateCode;
            ruleModel.StartStateCode = FinalStateCode;
            ruleModel.FinalTrackerOrderCode = CreateInvertedTrackerOrderCode(ruleModel.EndIndexingDeltas);
        }

        /// <inheritdoc />
        public abstract bool LinkIfInverseMatch(ITransitionRuleModel ruleModel);

        /// <inheritdoc />
        public abstract bool IsInverse(ITransitionRuleModel ruleModel);

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
            return EndIndexingDeltas.Reverse().Select(a => -a).ToList();
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
    }
}