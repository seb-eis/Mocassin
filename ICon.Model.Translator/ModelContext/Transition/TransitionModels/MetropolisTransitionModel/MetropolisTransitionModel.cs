using ICon.Model.Particles;
using ICon.Model.Transitions;
using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Model.Translator.ModelContext
{
    /// <summary>
    /// The metropolis transition model that carries the full model context data of a single metropolis transition
    /// </summary>
    public class MetropolisTransitionModel : ModelComponentBase, IMetropolisTransitionModel
    {
        /// <summary>
        /// Boolean flag that describes if this transition model has an inversion that is not itself
        /// </summary>
        public bool HasInversion => this != InverseTransitionModel;

        /// <summary>
        /// The transition the model is based upon
        /// </summary>
        public IMetropolisTransition Transition { get; set; }

        /// <summary>
        /// The inverse transition model that describes the neutralizing transition
        /// </summary>
        /// <remarks> Is equal to itself if start and end unit cell positions are identical </remarks>
        public IMetropolisTransitionModel InverseTransitionModel { get; set; }

        /// <summary>
        /// The list of existing mapping models that describe all possible geometries this transition model can be applied to
        /// </summary>
        public IList<IMetropolisMappingModel> MappingModels { get; set; }

        /// <summary>
        /// The extended rule models that contain all possible rules for the transition
        /// </summary>
        public IList<IMetropolisRuleModel> RuleModels { get; set; }

        /// <summary>
        /// The particle set that describes which particles are mobile
        /// </summary>
        public IParticleSet MobileParticles { get; set; }
    }
}
