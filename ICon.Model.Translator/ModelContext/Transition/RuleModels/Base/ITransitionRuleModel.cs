using System.Collections.Generic;
using Mocassin.Model.Particles;
using Mocassin.Model.Transitions;

namespace Mocassin.Model.Translator.ModelContext
{
    /// <summary>
    ///     Represents a transition rule model that describes the physics of the state manipulation of a specific transition
    ///     path occupation
    /// </summary>
    public interface ITransitionRuleModel
    {
        /// <summary>
        ///     Boolean flag if the inverse rule is already set
        /// </summary>
        bool InverseIsSet { get; }

        /// <summary>
        ///     Boolean flag that indicates that this rule model describes in inverted version
        ///     of its source rule and abstract transition
        /// </summary>
        bool IsSourceInversion { get; set; }

        /// <summary>
        ///     The abstract transition the rule model is based upon
        /// </summary>
        IAbstractTransition AbstractTransition { get; }

        /// <summary>
        ///     The selectable particle the rule describes
        /// </summary>
        IParticle SelectableParticle { get; }

        /// <summary>
        ///     Get or the attempt frequency of the rule model
        /// </summary>
        double AttemptFrequency { get; }

        /// <summary>
        ///     The list of particles that describes the path occupation in the start state
        /// </summary>
        IList<IParticle> StartState { get; set; }

        /// <summary>
        ///     The list of particles that describes the path occupation in the final state
        /// </summary>
        IList<IParticle> FinalState { get; set; }

        /// <summary>
        ///     The indexing description that describes the offset of particles in the end state compared to the start state
        /// </summary>
        IList<int> EndIndexingDeltas { get; set; }

        /// <summary>
        ///     Index encoded version of the start state occupation
        /// </summary>
        long StartStateCode { get; set; }

        /// <summary>
        ///     Index encoded version of the transition state occupation
        /// </summary>
        long TransitionStateCode { get; set; }

        /// <summary>
        ///     Index encoded version of the final state occupation
        /// </summary>
        long FinalStateCode { get; set; }

        /// <summary>
        ///     Index encoded final order of the involved dynamic trackers
        /// </summary>
        long FinalTrackerOrderCode { get; set; }

        /// <summary>
        ///     Copies the data on this rule model as inverted info onto the passed rule model
        /// </summary>
        /// <param name="ruleModel"></param>
        void CopyInversionDataToModel(ITransitionRuleModel ruleModel);

        /// <summary>
        ///     Links this and the passed rule model together if they are an inversion match
        /// </summary>
        /// <param name="ruleModel"></param>
        /// <returns></returns>
        bool LinkIfInverseMatch(ITransitionRuleModel ruleModel);

        /// <summary>
        ///     Checks if the passed rule model is the inverse of this one
        /// </summary>
        /// <param name="ruleModel"></param>
        /// <returns></returns>
        bool IsInverse(ITransitionRuleModel ruleModel);

        /// <summary>
        ///     Get a particle set that contains all particles that are not immobile in the context of the rule
        /// </summary>
        /// <returns></returns>
        IParticleSet GetMobileParticles();
    }
}