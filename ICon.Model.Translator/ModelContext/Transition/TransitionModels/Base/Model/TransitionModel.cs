using System.Collections.Generic;
using System.Linq;
using Mocassin.Framework.Collections;
using Mocassin.Framework.Extensions;
using Mocassin.Model.Particles;

namespace Mocassin.Model.Translator.ModelContext
{
    /// <inheritdoc cref="ITransitionModel" />
    public abstract class TransitionModel : ModelComponentBase, ITransitionModel
    {
        /// <inheritdoc />
        public IParticleSet SelectableParticles { get; set; }

        /// <inheritdoc />
        public long SelectableParticlesMask { get; set; }

        /// <inheritdoc />
        public abstract IEnumerable<ITransitionMappingModel> GetMappingModels();

        /// <inheritdoc />
        public abstract IEnumerable<ITransitionRuleModel> GetRuleModels();

        /// <inheritdoc />
        public IParticleSet GetMobileParticles()
        {
            return ParticleSet.ToSortedSet(GetRuleModels().SelectMany(x => x.GetMobileParticles()));
        }
    }
}