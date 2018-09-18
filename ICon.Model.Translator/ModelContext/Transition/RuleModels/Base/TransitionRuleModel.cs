using ICon.Model.Particles;
using ICon.Model.Transitions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace ICon.Model.Translator.ModelContext
{
    /// <summary>
    /// Base class for the transition rule model that contains the shared properties of kinetic and metropolis rules
    /// </summary>
    public abstract class TransitionRuleModel : ITransitionRuleModel
    {
        /// <summary>
        /// Boolean flag that indicates that this rule model describes in inverted version of its source rule and abstract transition
        /// </summary>
        public bool IsSourceInversion { get; set; }

        /// <summary>
        /// The abstract transition the rule model is based upon
        /// </summary>
        public abstract IAbstractTransition AbstractTransition { get; }

        /// <summary>
        /// The selectable particle the rule describes
        /// </summary>
        public abstract IParticle SelectableParticle { get; }

        /// <summary>
        /// The list of particles that describes the path occupation in the start state
        /// </summary>
        public IList<IParticle> StartState { get; set; }

        /// <summary>
        /// The list of particles that describes the path occupation in the final state
        /// </summary>
        public IList<IParticle> FinalState { get; set; }

        /// <summary>
        /// The movement description that describes the offset of particles in the end state compared to the start state
        /// </summary>
        public IList<int> EndIndexingDeltas { get; set; }

        /// <summary>
        /// Index encoded version of the start state occupation
        /// </summary>
        public long StartStateCode { get; set; }

        /// <summary>
        /// Index encoded version of the final state occupation
        /// </summary>
        public long FinalStateCode { get; set; }

        /// <summary>
        /// Index encoded final order of the involved dynamic trackers
        /// </summary>
        public long FinalTrackerOrderCode { get; set; }

        /// <summary>
        /// Copies the data on this rule model as inverted info onto the passed rule model
        /// </summary>
        /// <param name="ruleModel"></param>
        public void CopyInversionDataToModel(ITransitionRuleModel ruleModel)
        {
            ruleModel.FinalState = StartState.ToList();
            ruleModel.StartState = FinalState.ToList();
            ruleModel.EndIndexingDeltas = GetEndIndexingDeltaInversion();
            ruleModel.FinalStateCode = StartStateCode;
            ruleModel.StartStateCode = FinalStateCode;
            ruleModel.FinalTrackerOrderCode = CreateInvertedTrackerOrderCode(ruleModel.EndIndexingDeltas);
        }
        
        /// <summary>
        /// Creates the inverted version of end indexing deltas
        /// </summary>
        /// <returns></returns>
        protected IList<int> GetEndIndexingDeltaInversion()
        {
            return EndIndexingDeltas.Reverse().Select(a => -a).ToList();
        }

        /// <summary>
        /// Creates the inverted order code by code arithmetic (TARGET_ORDER = SOURCE_ORDER - SOURCE_EIDX + TARGET_EIDX)
        /// </summary>
        /// <param name="invertedEndIndexingDeltas"></param>
        /// <returns></returns>
        protected long CreateInvertedTrackerOrderCode(IList<int> invertedEndIndexingDeltas)
        {
            var bytes = BitConverter.GetBytes(FinalTrackerOrderCode);

            for (int i = 0; i < invertedEndIndexingDeltas.Count; i++)
            {
                bytes[i] -= (byte) (EndIndexingDeltas[i] - invertedEndIndexingDeltas[i]);
            }

            return BitConverter.ToInt64(bytes, 0);
        }
    }
}
