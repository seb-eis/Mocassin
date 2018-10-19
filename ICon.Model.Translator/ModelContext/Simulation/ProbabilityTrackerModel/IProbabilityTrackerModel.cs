using Mocassin.Model.Particles;

namespace Mocassin.Model.Translator.ModelContext
{
    /// <summary>
    ///     Represents a probability tracker model for kinetic simulations that describes a tracked jump probability
    /// </summary>
    public interface IProbabilityTrackerModel : IModelComponent
    {
        /// <summary>
        ///     The kinetic transition model the tracker belongs to
        /// </summary>
        IKineticTransitionModel KineticTransitionModel { get; set; }

        /// <summary>
        ///     The particle that is tracked
        /// </summary>
        IParticle TrackedParticle { get; set; }
    }
}