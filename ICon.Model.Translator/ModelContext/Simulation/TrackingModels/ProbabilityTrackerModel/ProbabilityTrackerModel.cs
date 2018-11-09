using Mocassin.Model.Particles;

namespace Mocassin.Model.Translator.ModelContext
{
    /// <inheritdoc />
    public class ProbabilityTrackerModel : IProbabilityTrackerModel
    {
        /// <inheritdoc />
        public int ModelId { get; set; }

        /// <inheritdoc />
        public IKineticTransitionModel KineticTransitionModel { get; set; }

        /// <inheritdoc />
        public IParticle TrackedParticle { get; set; }
    }
}