using System;
using Mocassin.Mathematics.ValueTypes;
using Mocassin.Model.Particles;

namespace Mocassin.Tools.Evaluation.Queries.Data
{
    /// <summary>
    ///     Struct that describes the movement of a <see cref="IParticle" /> ensemble in cartesian coordinates
    /// </summary>
    public readonly struct EnsembleDisplacement
    {
        /// <summary>
        ///     Get a boolean flag if the displacement is squared
        /// </summary>
        public bool IsSquared { get; }

        /// <summary>
        ///     Get the number of <see cref="IParticle" /> in the affiliated ensemble
        /// </summary>
        public int EnsembleSize { get; }

        /// <summary>
        ///     Get the <see cref="IParticle" /> that the movement belongs to
        /// </summary>
        public IParticle Particle { get; }

        /// <summary>
        ///     Get the <see cref="Cartesian3D" /> that describes the displacement as either [x] or [x^2] for (x,y,z) directions
        /// </summary>
        public Cartesian3D VectorR { get; }

        /// <summary>
        ///     Get the displacement in [x] or [x^2] for all directions
        /// </summary>
        public double DisplacementR { get; }

        /// <summary>
        ///     Get a boolean flag if the displacement is the mean displacement
        /// </summary>
        public bool IsMean { get; }

        /// <inheritdoc />
        public EnsembleDisplacement(bool isSquared, bool isMean, int ensembleSize, IParticle particle, double displacementR, Cartesian3D vectorR)
            : this()
        {
            if (ensembleSize == 0) throw new ArgumentException("Ensemble size cannot be 0", nameof(ensembleSize));
            IsMean = isMean;
            IsSquared = isSquared;
            EnsembleSize = ensembleSize;
            Particle = particle ?? throw new ArgumentNullException(nameof(particle));
            VectorR = vectorR;
            DisplacementR = displacementR;
        }

        /// <summary>
        ///     Get an <see cref="EnsembleDisplacement" /> of the current that contains 1 particles and represents the mean
        ///     behavior
        /// </summary>
        /// <returns></returns>
        public EnsembleDisplacement AsMean() => IsMean ? this : new EnsembleDisplacement(IsSquared, true, EnsembleSize, Particle, DisplacementR / EnsembleSize, VectorR / EnsembleSize);
    }
}