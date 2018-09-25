using ICon.Model.Particles;
using ICon.Model.Transitions;
using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Model.Translator.ModelContext
{
    /// <inheritdoc cref="ICon.Model.Translator.ModelContext.IMetropolisTransitionModel"/>
    public class MetropolisTransitionModel : ModelComponentBase, IMetropolisTransitionModel
    {
        /// <inheritdoc />
        public bool HasInversion => this != InverseTransitionModel;

        /// <inheritdoc />
        public IMetropolisTransition Transition { get; set; }

        /// <inheritdoc />
        public IMetropolisTransitionModel InverseTransitionModel { get; set; }

        /// <inheritdoc />
        public IList<IMetropolisMappingModel> MappingModels { get; set; }

        /// <inheritdoc />
        public IList<IMetropolisRuleModel> RuleModels { get; set; }

        /// <inheritdoc />
        public IParticleSet MobileParticles { get; set; }
    }
}
