using ICon.Model.Particles;

namespace ICon.Model.Translator.ModelContext
{
    /// <summary>
    /// Represents an unspecified transition model that fully describes a simulation transition
    /// </summary>
    public interface ITransitionModel : IModelComponent
    {
        /// <summary>
        /// Flag that indicates if this model has an inversion that is not itself
        /// </summary>
        bool HasInversion { get; }

        /// <summary>
        /// The particle set of all mobiles on the transition
        /// </summary>
        IParticleSet MobileParticles { get; set; }
    }
}