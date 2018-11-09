using Mocassin.Model.Particles;

namespace Mocassin.Model.Translator.ModelContext
{
    /// <summary>
    ///     Represents a probability tracker model for kinetic simulations that describes a tracked jump probability
    /// </summary>
    public interface IProbabilityTrackerModel : IModelComponent
    {
        /// <summary>
        ///     The kinetic transition model the tracker belongs to the tracker model
        /// </summary>
        IKineticTransitionModel KineticTransitionModel { get; set; }

        /// <summary>
        ///     The particle that is tracked by the model
        /// </summary>
        IParticle TrackedParticle { get; set; }
    }
}