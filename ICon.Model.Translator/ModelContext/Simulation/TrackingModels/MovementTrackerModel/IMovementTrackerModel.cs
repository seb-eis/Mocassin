using Mocassin.Model.Particles;

namespace Mocassin.Model.Translator.ModelContext
{
    /// <summary>
    ///     Represents a movement tracker model that describes and indexes a tracked physical movement property of the
    ///     simulation
    /// </summary>
    public interface IMovementTrackerModel : IModelComponent
    {
        /// <summary>
        ///     The particle that is tracked by this tracker model
        /// </summary>
        IParticle TrackedParticle { get; set; }

        /// <summary>
        ///     The kinetic transition model that belongs to this tracker model
        /// </summary>
        IKineticTransitionModel KineticTransitionModel { get; set; }
    }
}