using System;
using System.Collections.Generic;
using System.Text;
using ICon.Model.Particles;
using ICon.Model.Transitions;

namespace ICon.Model.Translator.ModelContext
{
    /// <summary>
    /// Represents a kinetic transition model that fully describes geometry and mobility information of a reference transition
    /// </summary>
    public interface IKineticTransitionModel : IModelComponent
    {
        /// <summary>
        /// Boolean flag that describes if this transition model describes an inverted transition
        /// </summary>
        bool IsInverted { get; }

        /// <summary>
        /// The transition the model is based upon
        /// </summary>
        IKineticTransition Transition { get; set; }

        /// <summary>
        /// The inverse transition model that describes the neutralizing transition
        /// </summary>
        /// <remarks> Is equal to itself if start and end unit cell positions are identical </remarks>
        IKineticTransitionModel InverseTransitionModel { get; set; }

        /// <summary>
        /// The list of existing mapping models that describe all possible geometries this transition model can be applied to
        /// </summary>
        IList<IKineticMappingModel> MappingModels { get; set; }

        /// <summary>
        /// The extended rule models that contain all possible rules for the transition
        /// </summary>
        IList<IKineticRuleModel> RuleModels { get; set; }

        /// <summary>
        /// The particle set that describes which particles are mobile
        /// </summary>
        IParticleSet MobileParticles { get; set; }

        /// <summary>
        /// Describes which effective particle describes the global tracking process
        /// </summary>
        IParticle EffectiveParticle { get; set; }
    }
}
