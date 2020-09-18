﻿using Mocassin.Model.Particles;

namespace Mocassin.Model.Translator.ModelContext
{
    /// <inheritdoc />
    public class MovementTrackerModel : IMovementTrackerModel
    {
        /// <inheritdoc />
        public int ModelId { get; set; }

        /// <inheritdoc />
        public IParticle TrackedParticle { get; set; }

        /// <inheritdoc />
        public IKineticTransitionModel KineticTransitionModel { get; set; }
    }
}