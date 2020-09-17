using Mocassin.Model.Particles;

namespace Mocassin.Model.Translator.ModelContext
{
    /// <summary>
    ///     Represents a global tracker model for kinetic simulations that defines a <see cref="IParticle" /> and
    ///     <see cref="IKineticTransitionModel" /> combination that is tracked by the simulator
    /// </summary>
    public interface IGlobalTrackerModel : IModelComponent
    {
        /// <summary>
        ///     The kinetic transition model the tracker belongs to
        /// </summary>
        IKineticTransitionModel KineticTransitionModel { get; set; }

        /// <summary>
        ///     The particle that is tracked by the model
        /// </summary>
        IParticle TrackedParticle { get; set; }
    }
}