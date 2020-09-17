using System.Collections.Generic;
using Mocassin.Mathematics.ValueTypes;
using Mocassin.Model.Particles;
using Mocassin.Model.Transitions;

namespace Mocassin.Model.Translator.ModelContext
{
    /// <summary>
    ///     Transition rule model for kinetic transitions that extends the basic transition rule model
    /// </summary>
    public interface IKineticRuleModel : ITransitionRuleModel
    {
        /// <summary>
        ///     The rule direction value [-1;0;1] that describes the transport direction encoded in the rule
        /// </summary>
        int RuleDirectionValue { get; set; }

        /// <summary>
        ///     The kinetic transition model the rule model belongs to
        /// </summary>
        IKineticTransitionModel TransitionModel { get; set; }

        /// <summary>
        ///     The kinetic transition rule this model is based upon
        /// </summary>
        IKineticRule KineticRule { get; set; }

        /// <summary>
        ///     The rule model that describes the neutralizing inverted sister rule
        /// </summary>
        IKineticRuleModel InverseRuleModel { get; set; }

        /// <summary>
        ///     The list of particles that describes the path occupation in the transition state
        /// </summary>
        IList<IParticle> TransitionState { get; set; }

        /// <summary>
        ///     The 1xN charge transport matrix that describes the transport charge for each start occupation
        /// </summary>
        Matrix2D ChargeTransportMatrix { get; set; }

        /// <summary>
        ///     Creates a raw geometry inversion of this rule model
        /// </summary>
        /// <returns></returns>
        IKineticRuleModel CreateGeometricInversion();
    }
}