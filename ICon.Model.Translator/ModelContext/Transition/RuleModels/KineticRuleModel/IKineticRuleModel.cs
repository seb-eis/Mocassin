using ICon.Model.Particles;
using ICon.Model.Transitions;
using System;
using System.Collections.Generic;
using System.Text;
using ICon.Mathematics.ValueTypes;

namespace ICon.Model.Translator.ModelContext
{
    /// <summary>
    /// Transition rule model for kinetic transitions that extends the basic transition rule model
    /// </summary>
    public interface IKineticRuleModel : ITransitionRuleModel
    {
        /// <summary>
        /// The kinetic transition rule this model is based upon
        /// </summary>
        IKineticRule KineticRule { get; set; }

        /// <summary>
        /// The rule model that describes the neutralizing inverted sister rule
        /// </summary>
        IKineticRuleModel InverseRuleModel { get; set; }

        /// <summary>
        /// The list of particles that describes the path occupation in the transition state
        /// </summary>
        IList<IParticle> TransitionState { get; set; }

        /// <summary>
        /// Index encoded version of the transition state occupation
        /// </summary>
        long TransitionStateCode { get; set; }

        /// <summary>
        /// The effectively transported charge that the rule describes
        /// </summary>
        double EffectiveTransportCharge { get; set; }

        /// <summary>
        /// The charge transport 1xN matrix. Describes the transportable charge of each start occupation
        /// </summary>
        Matrix2D ChargeTransportMatrix { get; set; }
    }
}
