using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Mocassin.Framework.Extensions;
using Mocassin.Model.Basic;
using Mocassin.Model.Particles;

namespace Mocassin.Model.Transitions
{
    /// <summary>
    ///     Flag for properties of transition rules
    /// </summary>
    [Flags]
    public enum RuleFlags
    {
        /// <summary>
        ///     Flag for physically meaningless rules
        /// </summary>
        PhysicallyInvalid = 1
    }

    /// <summary>
    ///     Further specifies the type of the rule in terms of the physical process
    /// </summary>
    [Flags]
    public enum RuleMovementFlags
    {
        /// <summary>
        ///     Movement contains unsupported or unrecognized steps
        /// </summary>
        HasUnsupportedMovement = 1,

        /// <summary>
        ///     Movement contains physical movement
        /// </summary>
        HasPhysicalMovement = 1 << 1,

        /// <summary>
        ///     Movement contains property exchange movement
        /// </summary>
        HasPropertyMovement = 1 << 2,

        /// <summary>
        ///     Movement contains vacancy movement
        /// </summary>
        HasVacancyMovement = 1 << 3,

        /// <summary>
        ///     Movement contains vehicle movement
        /// </summary>
        HasVehicleMovement = 1 << 4,

        /// <summary>
        ///     Movement contains physical atom pushing movement
        /// </summary>
        HasChainedMovement = 1 << 5,

        /// <summary>
        ///     Movement is recognized as an exchange
        /// </summary>
        IsExchange = 1 << 6,

        /// <summary>
        ///     Movement is recognized as a migration
        /// </summary>
        IsMigration = 1 << 7,

        /// <summary>
        ///     Movement is recognized as an association/dissociation type
        /// </summary>
        IsAssociation = 1 << 8,
    }

    /// <inheritdoc cref="Mocassin.Model.Transitions.ITransitionRule" />
    [DataContract]
    public abstract class TransitionRule : ModelObject, ITransitionRule, IComparable<ITransitionRule>
    {
        /// <inheritdoc />
        [IgnoreDataMember]
        public int PathLength => StartState.StateLength;

        /// <inheritdoc />
        [DataMember]
        public IAbstractTransition AbstractTransition { get; set; }

        /// <inheritdoc />
        [DataMember]
        public RuleFlags RuleFlags { get; set; }

        /// <inheritdoc />
        [DataMember]
        public RuleMovementFlags MovementFlags { get; set; }

        /// <summary>
        ///     The start state of the transition
        /// </summary>
        [DataMember]
        public OccupationState StartState { get; set; }

        /// <summary>
        ///     The final state of the transition
        /// </summary>
        [DataMember]
        public OccupationState FinalState { get; set; }

        /// <summary>
        ///     The transition state of the transition
        /// </summary>
        [DataMember]
        public OccupationState TransitionState { get; set; }

        /// <summary>
        ///     Describes the physical movement as a sequence of integers
        /// </summary>
        [DataMember]
        public MovementCode MovementCode { get; set; }

        /// <inheritdoc />
        [IgnoreDataMember]
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
            return (RuleFlags & RuleFlags.PhysicallyInvalid) != 0;
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
            if (!(CastIfNotDeprecated<ITransitionRule>(obj) is ITransitionRule rule))
                return null;

            StartState = new OccupationState {Particles = rule.GetStartStateOccupation().ToList()};
            TransitionState = new OccupationState {Particles = rule.GetTransitionStateOccupation().ToList()};
            FinalState = new OccupationState {Particles = rule.GetFinalStateOccupation().ToList()};
            MovementCode = new MovementCode {CodeValues = rule.GetMovementDescription().ToArray()};
            RuleFlags = rule.RuleFlags;
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