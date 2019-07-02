using System;
using Mocassin.Mathematics.ValueTypes;
using Mocassin.Model.Particles;

namespace Mocassin.Tools.Evaluation.Selection.Result
{
    /// <summary>
    ///     Struct that describes the movement of a single <see cref="IParticle"/> in cartesian coordinates
    /// </summary>
    public readonly struct ParticleMovement
    {
        /// <summary>
        ///     Get the <see cref="IParticle"/> that the movement belongs to
        /// </summary>
        public IParticle Particle { get; }

        /// <summary>
        ///     Get the <see cref="Cartesian3D"/> that describes the displacement
        /// </summary>
        public Cartesian3D Vector { get; }

        public ParticleMovement(IParticle particle, in Cartesian3D vector) : this()
        {
            Particle = particle ?? throw new ArgumentNullException(nameof(particle));
            Vector = vector;
        }
    }
}