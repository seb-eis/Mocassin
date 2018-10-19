using System.Collections.Generic;
using Mocassin.Model.Particles;
using Mocassin.Model.Transitions;

namespace Mocassin.Model.Translator.ModelContext
{
    /// <inheritdoc cref="IMetropolisTransitionModel" />
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

        /// <inheritdoc />
        public IMetropolisTransitionModel CreateInverse()
        {
            return new MetropolisTransitionModel
            {
                InverseTransitionModel = this,
                Transition = Transition
            };
        }
    }
}