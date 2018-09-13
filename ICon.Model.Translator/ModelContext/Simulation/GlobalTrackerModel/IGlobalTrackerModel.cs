using ICon.Model.Particles;
using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Model.Translator.ModelContext
{
    /// <summary>
    /// Represents a global tracker model that describes and indexes a globally tracked movement property of the simulation
    /// </summary>
    public interface IGlobalTrackerModel : IModelComponent
    {
        /// <summary>
        /// The index of the tracker in the simulation state
        /// </summary>
        int TrackerId { get; set; }

        /// <summary>
        /// The particle that is tracked by this global tracker model
        /// </summary>
        IParticle TrackedParticle { get; set; }

        /// <summary>
        /// The kinetic transition model that belongs to this tracker model
        /// </summary>
        IKineticTransitionModel KineticTransitionModel { get; set; }
    }
}
