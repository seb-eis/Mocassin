﻿using System;
using System.Collections.Generic;
using System.Text;

using ICon.Model.Basic;
using ICon.Model.Particles;

namespace ICon.Model.Transitions
{
    /// <summary>
    /// Represents a transition rule of unspecified type that describes the state change during a transition and can be toggled on or off 
    /// </summary>
    public interface ITransitionRule : IModelObject
    {
        /// <summary>
        /// The path length of the transition
        /// </summary>
        int PathLength { get; }

        /// <summary>
        /// The rule flags of the rule
        /// </summary>
        RuleFlags RuleFlags { get; }

        /// <summary>
        /// The abstract transition instance
        /// </summary>
        IAbstractTransition AbstractTransition { get; set; }

        /// <summary>
        /// The particle indices of the transition path in the start state
        /// </summary>
        IEnumerable<IParticle> GetStartStateOccupation();

        /// <summary>
        /// The particle indices of the transition in the final state
        /// </summary>
        IEnumerable<IParticle> GetFinalStateOccupation();

        /// <summary>
        /// The particle indices for the unstable transition positions (Size is always path size - 1 )
        /// </summary>
        IEnumerable<IParticle> GetTransitionStateOccupation();

        /// <summary>
        /// Get all linked rules of this transition
        /// </summary>
        /// <returns></returns>
        IEnumerable<ITransitionRule> GetLinkedRules();

        /// <summary>
        /// Get an index based description of the movement from start state to end state
        /// </summary>
        /// <returns></returns>
        IEnumerable<int> GetMovementDescription();

        /// <summary>
        /// Set the activation status of the rule (Automatically passed to linked rules)
        /// </summary>
        /// <param name="active"></param>
        void SetActivationStatus(bool active);
    }
}
