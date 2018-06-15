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
    /// Enum for properties of transition rules
    /// </summary>
    public enum RuleFlags : uint
    {
        Activatable = 0, Active = 1 << 1, Inversion = 1 << 2, CustomTransitionState = 1 << 3, PhysicallyInvalid = 1 << 4
    }

    /// <summary>
    /// Enum for closed periodic cell boundaries
    /// </summary>
    public enum CellBoundaryFlags : uint
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
        /// The object used for short term locking
        /// </summary>
        [IgnoreDataMember]
        private object LockObject { get; set; } = new object();
        
        /// <summary>
        /// The updating flag indicating if the object is currently doing an updating process
        /// </summary>
        [IgnoreDataMember]
        private bool UpdatingFlag { get; set; }

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
        /// The list of linked rule objects
        /// </summary>
        [DataMember]
        public List<ITransitionRule> LinkedRules { get; set; }

        /// <summary>
        /// The rule flags of the rule
        /// </summary>
        [DataMember]
        public RuleFlags RuleFlags { get; set; }

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
            return StartState.AsEnumerable();
        }

        /// <summary>
        /// Get the final state occupation as a seqeunce of particles
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IParticle> GetFinalStateOccupation()
        {
            return FinalState.AsEnumerable();
        }

        /// <summary>
        /// Get the transition state occupation as a seqeunce of particles
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IParticle> GetTransitionStateOccupation()
        {
            return TransitionState.AsEnumerable();
        }

        /// <summary>
        /// Get the movement information as an index based code sequence
        /// </summary>
        /// <returns></returns>
        public IEnumerable<int> GetMovementDescription()
        {
            return MovementCode.AsEnumerable();
        }

        /// <summary>
        /// Get all rules that are linked with this one and therefore have an activation toggle dependency
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ITransitionRule> GetLinkedRules()
        {
            return LinkedRules.AsEnumerable();
        }

        /// <summary>
        /// Sets the activation status on this and all linked rules (lock protection to prevent self reference deadlock)
        /// </summary>
        /// <param name="activate"></param>
        public void SetActivationStatus(bool activate)
        {
            if (SetUpdatingFlag(true))
            {
                if (!RuleFlags.HasFlag(RuleFlags.Active) && activate)
                {
                    RuleFlags &= RuleFlags.Active;
                }
                if (RuleFlags.HasFlag(RuleFlags.Active) && !activate)
                {
                    RuleFlags -= RuleFlags.Active;
                }
                foreach (var rule in LinkedRules)
                {
                    rule.SetActivationStatus(activate);
                }
                SetUpdatingFlag(false);
            }
        }

        /// <summary>
        /// Thread safe set of the updating flag. Retruns true if the flag sattus actually changed
        /// </summary>
        /// <param name="newStatus"></param>
        /// <returns></returns>
        private bool SetUpdatingFlag(bool newStatus)
        {
            lock (LockObject)
            {
                if (UpdatingFlag == newStatus)
                {
                    return false;
                }
                UpdatingFlag = newStatus;
                return true;
            }
        }

        /// <summary>
        /// Polulates the object from an interface and returns the object if population was successfull (Retruns null on fail)
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override ModelObject PopulateObject(IModelObject obj)
        {
            if (CastWithDepricatedCheck<ITransitionRule>(obj) is var rule)
            {
                StartState = new OccupationState() { Particles = rule.GetStartStateOccupation().ToList() };
                TransitionState = new OccupationState() { Particles = rule.GetTransitionStateOccupation().ToList() };
                FinalState = new OccupationState() { Particles = rule.GetFinalStateOccupation().ToList() };
                MovementCode = new MovementCode() { CodeValues = rule.GetMovementDescription().ToArray() };
                RuleFlags = rule.RuleFlags;
                AbstractTransition = rule.AbstractTransition;
                LinkedRules = rule.GetLinkedRules().ToList();
                return this;
            }
            return null;
        }
    }
}
