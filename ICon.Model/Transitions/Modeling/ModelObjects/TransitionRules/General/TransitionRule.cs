using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

using ICon.Framework.Extensions;
using ICon.Model.Basic;
using ICon.Model.Particles;

namespace ICon.Model.Transitions
{
    /// <summary>
    /// Enum for properties of transition rules (Inverted: Corss exchanged strat and end state, Reverted: stard and end state are themselves inverted)
    /// </summary>
    public enum RuleFlags : int
    {
        Activatable = 0, Active = 0b1, Inverted = 0b10, Reversed = 0b100, CustomTransitionState = 0b1000, PhysicallyInvalid = 0b10000
    }

    /// <summary>
    /// Further specifies the type of the rule in terms of the physical process
    /// </summary>
    public enum RuleMovementType : int
    {
        NotSupported = 0b1, Physical = 0b10, Property = 0b100, Exchange = 0b1000, Migration = 0b10000, Vacancy = 0b100000,
        VacancyMigration = Physical + Migration + Vacancy,
        PhysicalMigration = Physical + Migration,
        PhysicalExchange = Physical + Exchange,
        PropertyMigration = Property + Migration,
        PropertyExchange = Property + Exchange,
        CombinatoryMigration = Physical + Property + Migration + Vacancy
    }

    /// <summary>
    /// Enum for closed periodic cell boundaries
    /// </summary>
    public enum CellBoundaryFlags : int
    {
        AllOpen = 0, ClosedDirectionA = 1, ClosedDirectionB = 1 << 1, ClosedDirectionC = 1 << 2
    }

    /// <summary>
    /// Abstract base class for transition rule implementations that accries all information shared across KMC and MMC rules
    /// </summary>
    [DataContract]
    public abstract class TransitionRule : ModelObject, ITransitionRule, IComparable<ITransitionRule>
    {
        /// <summary>
        /// The path length of the transition
        /// </summary>
        [IgnoreDataMember]
        public int PathLength => StartState.StateLength;

        /// <summary>
        /// The abstract transition used to create this rule
        /// </summary>
        [DataMember]
        public IAbstractTransition AbstractTransition { get; set; }

        /// <summary>
        /// The rule flags of the rule
        /// </summary>
        [DataMember]
        public RuleFlags RuleFlags { get; set; }

        /// <summary>
        /// The movement type of the rule
        /// </summary>
        [DataMember]
        public RuleMovementType MovementType { get; set; }

        /// <summary>
        /// The start state of the transition
        /// </summary>
        [DataMember]
        public OccupationState StartState { get; set; }

        /// <summary>
        /// The final state of the transition
        /// </summary>
        [DataMember]
        public OccupationState FinalState { get; set; }

        /// <summary>
        /// The transition state of the transition
        /// </summary>
        [DataMember]
        public OccupationState TransitionState { get; set; }

        /// <summary>
        /// Describes the physical movement as a sequence of integers
        /// </summary>
        [DataMember]
        public MovementCode MovementCode { get; set; }

        /// <summary>
        /// The selectable particle the rule describes
        /// </summary>
        [IgnoreDataMember]
        public IParticle SelectableParticle => StartState[0];

        /// <summary>
        /// Create new empty transition rule
        /// </summary>
        protected TransitionRule()
        {

        }

        /// <summary>
        /// Compares to other rule (Lexicographical compare of start state and end state, returns 0 if start state and end state are indentical)
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(ITransitionRule other)
        {
            int startComp = StartState.Select(a => a.Index).LexicographicCompare(other.GetStartStateOccupation().Select(a => a.Index));
            if (startComp == 0)
            {
                return FinalState.Select(a => a.Index).LexicographicCompare(other.GetFinalStateOccupation().Select(a => a.Index));
            }
            return startComp;
        }

        /// <summary>
        /// Performs a lexicographic compare of start and end states while detecting inverted equivalency as well
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareToWithCodeInversion(ITransitionRule other)
        {
            if (CompareTo(other) != 0)
            {
                return StartState.Select(a => a.Index).LexicographicCompare(other.GetFinalStateOccupation().Select(a => a.Index));
            }
            return 0;
        }

        /// <summary>
        /// Checks if the rule is marked as physically invalid
        /// </summary>
        /// <returns></returns>
        public bool IsPhysicallyInvalid()
        {
            return (RuleFlags & RuleFlags.PhysicallyInvalid) != 0;
        }

        /// <summary>
        /// Get the start state occupation as a seqeunce of particles
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IParticle> GetStartStateOccupation()
        {
            return (StartState ?? new OccupationState()).AsEnumerable();
        }

        /// <summary>
        /// Get the final state occupation as a seqeunce of particles
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IParticle> GetFinalStateOccupation()
        {
            return (FinalState ?? new OccupationState()).AsEnumerable();
        }

        /// <summary>
        /// Get the transition state occupation as a seqeunce of particles
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IParticle> GetTransitionStateOccupation()
        {
            return (TransitionState ?? new OccupationState()).AsEnumerable();
        }

        /// <summary>
        /// Get the movement information as an index based code sequence
        /// </summary>
        /// <returns></returns>
        public IEnumerable<int> GetMovementDescription()
        {
            return (MovementCode ?? new MovementCode()).AsEnumerable();
        }

        /// <summary>
        /// Sets the activation status on this and all linked rules (lock protection to prevent self reference deadlock)
        /// </summary>
        /// <param name="activate"></param>
        public void SetActivationStatus(bool activate)
        {
            if (!RuleFlags.HasFlag(RuleFlags.Active) && activate)
            {
                RuleFlags &= RuleFlags.Active;
                return;
            }
            if (RuleFlags.HasFlag(RuleFlags.Active) && !activate)
            {
                RuleFlags -= RuleFlags.Active;
                return;
            }
        }

        /// <summary>
        /// Polulates the object from an interface and returns the object if population was successfull (Retruns null on fail)
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override ModelObject PopulateFrom(IModelObject obj)
        {
            if (CastWithDepricatedCheck<ITransitionRule>(obj) is var rule)
            {
                StartState = new OccupationState() { Particles = rule.GetStartStateOccupation().ToList() };
                TransitionState = new OccupationState() { Particles = rule.GetTransitionStateOccupation().ToList() };
                FinalState = new OccupationState() { Particles = rule.GetFinalStateOccupation().ToList() };
                MovementCode = new MovementCode() { CodeValues = rule.GetMovementDescription().ToArray() };
                RuleFlags = rule.RuleFlags;
                MovementType = rule.MovementType;
                AbstractTransition = rule.AbstractTransition;
                return this;
            }
            return null;
        }

        /// <summary>
        /// Adds a dependent rule to this rule
        /// </summary>
        /// <param name="rule"></param>
        public abstract void AddDependentRule(TransitionRule rule); 
    }
}
