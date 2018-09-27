using ICon.Model.Particles;

namespace ICon.Model.Translator.ModelContext
{
    /// <inheritdoc cref="ICon.Model.Translator.ModelContext.ITransitionModel"/>
    public abstract class TransitionModel : ModelComponentBase, ITransitionModel
    {
        /// <inheritdoc />
        public abstract bool HasInversion { get; }

        /// <inheritdoc />
        public IParticleSet MobileParticles { get; set; }
    }
}