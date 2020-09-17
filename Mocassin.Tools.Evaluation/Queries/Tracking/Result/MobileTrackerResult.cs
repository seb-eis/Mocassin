using System;
using Mocassin.Mathematics.ValueTypes;
using Mocassin.Model.Particles;

namespace Mocassin.Tools.Evaluation.Queries
{
    /// <summary>
    ///     Struct that contains the tracking data of a single mobile <see cref="IParticle" />
    /// </summary>
    public readonly struct MobileTrackerResult
    {
        /// <summary>
        ///     Get the index of the original position id
        /// </summary>
        public int OriginalPositionId { get; }

        /// <summary>
        ///     Get the <see cref="IParticle" /> that the movement belongs to
        /// </summary>
        public IParticle Particle { get; }

        /// <summary>
        ///     Get the <see cref="Cartesian3D" /> that describes the displacement
        /// </summary>
        public Cartesian3D Displacement { get; }

        /// <inheritdoc />
        public MobileTrackerResult(IParticle particle, int originalPositionId, in Cartesian3D displacement)
            : this()
        {
            Particle = particle ?? throw new ArgumentNullException(nameof(particle));
            OriginalPositionId = originalPositionId;
            Displacement = displacement;
        }
    }
}