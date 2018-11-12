using System.Collections.Generic;
using Mocassin.Model.Particles;

namespace Mocassin.Model.Translator.ModelContext
{
    /// <inheritdoc cref="ITransitionModel" />
    public abstract class TransitionModel : ModelComponentBase, ITransitionModel
    {
        /// <inheritdoc />
        public abstract bool HasInversion { get; }

        /// <inheritdoc />
        public IParticleSet MobileParticles { get; set; }

        /// <inheritdoc />
        public abstract IEnumerable<ITransitionMappingModel> GetMappingModels();
    }
}