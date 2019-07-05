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
        ///     Get the vector sum of the local <see cref="IParticle"/> movement
        /// </summary>
        public Cartesian3D VectorSum { get; }

        public StaticTrackerResult(int positionId, IParticle particle, in Cartesian3D vectorSum) : this()
        {
            PositionId = positionId;
            Particle = particle ?? throw new ArgumentNullException(nameof(particle));
            VectorSum = vectorSum;
        }
    }
}