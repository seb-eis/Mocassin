using System.Collections.Generic;
using System.Linq;
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
        public IParticleSet SelectableParticles { get; set; }

        /// <inheritdoc />
        public long SelectableParticleMask { get; set; }

        /// <inheritdoc />
        public IEnumerable<ITransitionMappingModel> GetMappingModels()
        {
            return MappingModels.AsEnumerable();
        }

        /// <inheritdoc />
        public IEnumerable<ITransitionRuleModel> GetRuleModels()
        {
            return RuleModels.AsEnumerable();
        }

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