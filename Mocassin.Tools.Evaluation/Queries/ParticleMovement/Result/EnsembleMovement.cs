using System;
using Mocassin.Mathematics.ValueTypes;
using Mocassin.Model.Particles;

namespace Mocassin.Tools.Evaluation.Selection
{
    /// <summary>
    ///     Struct that describes the movement of a <see cref="IParticle"/> ensemble in cartesian coordinates
    /// </summary>
    public readonly struct EnsembleMovement
    {
        /// <summary>
        ///     Get a boolean flag if the displacement is squared
        /// </summary>
        public bool IsSquared { get; }

        /// <summary>
        ///     Get the number of <see cref="IParticle"/> in the affiliated ensemble
        /// </summary>
        public int EnsembleSize { get; }

        /// <summary>
        ///     Get the <see cref="IParticle"/> that the movement belongs to
        /// </summary>
        public IParticle Particle { get; }

        /// <summary>
        ///     Get the <see cref="Cartesian3D"/> that describes the displacement
        /// </summary>
        public Cartesian3D Vector { get; }

        /// <summary>
        ///     Get a boolean flag if the displacement is the mean displacement
        /// </summary>
        public bool IsMean => EnsembleSize == 1;

        public EnsembleMovement(bool isSquared, int ensembleSize, IParticle particle, Cartesian3D vector) : this()
        {
            if (ensembleSize == 0) throw new ArgumentException("Ensemble size cannot be 0", nameof(ensembleSize));
            IsSquared = isSquared;
            EnsembleSize = ensembleSize;
            Particle = particle ?? throw new ArgumentNullException(nameof(particle));
            Vector = vector;
        }

        /// <summary>
        ///     Get the mean <see cref="EnsembleMovement"/> of the current
        /// </summary>
        /// <returns></returns>
        public EnsembleMovement AsMean()
        {
            return new EnsembleMovement(IsSquared, 1, Particle, Vector / EnsembleSize);
        }
    }
}