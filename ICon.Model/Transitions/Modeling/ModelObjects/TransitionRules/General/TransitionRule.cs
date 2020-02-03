using System;
using System.Collections.Generic;
using System.Linq;
using Mocassin.Framework.Extensions;
using Mocassin.Model.Basic;
using Mocassin.Model.Particles;

namespace Mocassin.Model.Transitions
{
    /// <inheritdoc cref="Mocassin.Model.Transitions.ITransitionRule" />
    public abstract class TransitionRule : ModelObject, ITransitionRule, IComparable<ITransitionRule>
    {
        /// <inheritdoc />
        public int PathLength => StartState.StateLength;

        /// <inheritdoc />
        [UseTrackedData]
        public IAbstractTransition AbstractTransition { get; set; }

        /// <inheritdoc />
        public TransitionRuleFlags TransitionRuleFlags { get; set; }

        /// <inheritdoc />
        public RuleMovementFlags MovementFlags { get; set; }

        /// <summary>
        ///     The start state of the transition
        /// </summary>
        public OccupationState StartState { get; set; }

        /// <summary>
        ///     The final state of the transition
        /// </summary>
        public OccupationState FinalState { get; set; }

        /// <summary>
        ///     The transition state of the transition
        /// </summary>
        public OccupationState TransitionState { get; set; }

        /// <summary>
        ///     Describes the physical movement as a sequence of integers
        /// </summary>
        public MovementCode MovementCode { get; set; }

        /// <inheritdoc />
        public IParticle SelectableParticle => StartState[0];

        /// <summary>
        ///     Compares to other rule (Lexicographical compare of start state and end state, returns 0 if start state and end
        ///     state are identical)
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(ITransitionRule other)
        {
            var startComp = StartState.Select(a => a.Index).LexicographicCompare(other.GetStartStateOccupation().Select(a => a.Index));
            return startComp == 0
                ? FinalState.Select(a => a.Index).LexicographicCompare(other.GetFinalStateOccupation().Select(a => a.Index))
                : startComp;
        }

        /// <summary>
        ///     Performs a lexicographic compare of start and end states while detecting inverted equivalency as well
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareToWithCodeInversion(ITransitionRule other)
        {
            return CompareTo(other) != 0
                ? StartState.Select(a => a.Index).LexicographicCompare(other.GetFinalStateOccupation().Select(a => a.Index))
                : 0;
        }

        /// <summary>
        ///     Checks if the rule is marked as physically invalid
        /// </summary>
        /// <returns></returns>
        public bool IsPhysicallyInvalid()
        {
            return (TransitionRuleFlags & TransitionRuleFlags.PhysicallyInvalid) != 0;
        }

        /// <inheritdoc />
        public IEnumerable<IParticle> GetStartStateOccupation()
        {
            return (StartState ?? new OccupationState()).AsEnumerable();
        }

        /// <inheritdoc />
        public IEnumerable<IParticle> GetFinalStateOccupation()
        {
            return (FinalState ?? new OccupationState()).AsEnumerable();
        }

        /// <inheritdoc />
        public IEnumerable<IParticle> GetTransitionStateOccupation()
        {
            return (TransitionState ?? new OccupationState()).AsEnumerable();
        }

        /// <inheritdoc />
        public IEnumerable<int> GetMovementDescription()
        {
            return (MovementCode ?? new MovementCode()).AsEnumerable();
        }

        /// <inheritdoc />
        public override ModelObject PopulateFrom(IModelObject obj)
        {
            if (!(CastIfNotDeprecated<ITransitionRule>(obj) is { } rule))
                return null;

            StartState = new OccupationState {Particles = rule.GetStartStateOccupation().ToList()};
            TransitionState = new OccupationState {Particles = rule.GetTransitionStateOccupation().ToList()};
            FinalState = new OccupationState {Particles = rule.GetFinalStateOccupation().ToList()};
            MovementCode = new MovementCode {CodeValues = rule.GetMovementDescription().ToArray()};
            TransitionRuleFlags = rule.TransitionRuleFlags;
            MovementFlags = rule.MovementFlags;
            AbstractTransition = rule.AbstractTransition;
            return this;
        }

        /// <summary>
        ///     Adds a dependent rule to this rule
        /// </summary>
        /// <param name="rule"></param>
        public abstract void AddDependentRule(TransitionRule rule);
    }
}