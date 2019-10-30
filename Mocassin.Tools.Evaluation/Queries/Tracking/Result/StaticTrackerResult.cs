using System;
using Mocassin.Mathematics.ValueTypes;
using Mocassin.Model.Particles;

namespace Mocassin.Tools.Evaluation.Queries
{
    /// <summary>
    ///     Stores the static tracking result of a single <see cref="IParticle" /> on a specific position id
    /// </summary>
    public readonly struct StaticTrackerResult
    {
        /// <summary>
        ///     Get the position id the result belongs to
        /// </summary>
        public int PositionId { get; }

        /// <summary>
        ///     Get the <see cref="IParticle"/> that the data belongs to
        /// </summary>
        public IParticle Particle { get; }

        /// <summary>
        ///     Get the local velocity <see cref="Cartesian3D"/> vector of the <see cref="IParticle"/> movement in [m/s]
        /// </summary>
        public Cartesian3D VelocityVector { get; }

        public StaticTrackerResult(int positionId, IParticle particle, in Cartesian3D velocityVector) : this()
        {
            PositionId = positionId;
            Particle = particle ?? throw new ArgumentNullException(nameof(particle));
            VelocityVector = velocityVector;
        }
    }
}