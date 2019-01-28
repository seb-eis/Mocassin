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
        IsCustomTransitionState = 0b1,
        IsPhysicallyInvalid = 0b10
    }

    /// <summary>
    ///     Further specifies the type of the rule in terms of the physical process
    /// </summary>
    [Flags]
    public enum RuleMovementFlags
    {
        NotSupported = 1 << 1,
        Physical = 1 << 2,
        Property = 1 << 3,
        Exchange = 1 << 4,
        Migration = 1 << 5,
        Vacancy = 1 << 6,
        Vehicle = 1 << 7,
        AssociationDissociation = 1 << 8,
        IntersticialcyLike = 1 << 9,
        VacancyMigration = Physical + Migration + Vacancy,
        PhysicalMigration = Physical + Migration,
        PhysicalExchange = Physical + Exchange,
        PropertyMigration = Property + Migration,
        PropertyExchange = Property + Exchange,
        CombinatoryMigration = Physical + Property + Migration + Vacancy
    }

    /// <inheritdoc cref="Mocassin.Model.Transitions.ITransitionRule"/>
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
            return (RuleFlags & RuleFlags.IsPhysicallyInvalid) != 0;
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