using ICon.Model.Particles;
using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Model.Translator.ModelContext
{
    /// <summary>
    /// Represents a probability tracker model for kinetic siumlations that describes a treacked jump probability
    /// </summary>
    public interface IProbabilityTrackerModel : IModelComponent
    {
        /// <summary>
        /// The id of the probability tracker
        /// </summary>
        int TrackerId { get; set; }

        /// <summary>
        /// The kinetic transition model the tracker belongs to
        /// </summary>
        IKineticTransitionModel KineticTransitionModel { get; set; }

        /// <summary>
        /// The particle that is tracked
        /// </summary>
        IParticle TrackedParticle { get; set; }
    }
}
