using ICon.Model.Particles;
using ICon.Model.Transitions;
using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Model.Translator.ModelContext
{
    /// <summary>
    /// Represents a transition rule model that describes the physics of the state manipulation of a specific transition path occupation
    /// </summary>
    public interface ITransitionRuleModel
    {
        /// <summary>
        /// The abstract transition the rule model is based upon
        /// </summary>
        IAbstractTransition AbstractTransition { get; }

        /// <summary>
        /// The selectable particle the rule describes
        /// </summary>
        IParticle SelectableParticle { get; }

        /// <summary>
        /// The list of particles that describes the path occupation in the start state
        /// </summary>
        IList<IParticle> StartState { get; set; }

        /// <summary>
        /// The list of particles that describes the path occupation in the final state
        /// </summary>
        IList<IParticle> FinalState { get; set; }

        /// <summary>
        /// The movement description that describes the offset of particles in the end state compared to the start state
        /// </summary>
        IList<int> MovementDescription { get; set; }

        /// <summary>
        /// Determination matrix that describes the full determination reorder process of the rule
        /// </summary>
        int[,] RuleDeterminationMatrix { get; set; }

        /// <summary>
        /// Index encoded version of the start state occupation
        /// </summary>
        long StartStateCode { get; set; }

        /// <summary>
        /// Index encoded version of the final state occupation
        /// </summary>
        long FinalStateCode { get; set; }

        /// <summary>
        /// Index encoded final order of the involved dynamic trackers
        /// </summary>
        long FinalTrackerOrderCode { get; set; }
    }
}
