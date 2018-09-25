using ICon.Model.Particles;
using ICon.Model.Transitions;
using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Model.Translator.ModelContext
{
    /// <inheritdoc cref="ICon.Model.Translator.ModelContext.IKineticTransitionModel"/>
    public class KineticTransitionModel : ModelComponentBase, IKineticTransitionModel
    {
        /// <inheritdoc />
        public bool IsInverted { get; set; }

        /// <inheritdoc />
        public IKineticTransition Transition { get; set; }

        /// <inheritdoc />
        public IKineticTransitionModel InverseTransitionModel { get; set; }

        /// <inheritdoc />
        public IList<IKineticMappingModel> MappingModels { get; set; }

        /// <inheritdoc />
        public IList<IKineticRuleModel> RuleModels { get; set; }

        /// <inheritdoc />
        public IParticleSet MobileParticles { get; set; }

        /// <inheritdoc />
        public IParticle EffectiveParticle { get; set; }
    }
}
