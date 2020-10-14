using System;
using Mocassin.Model.Particles;

namespace Mocassin.Tools.Evaluation.Queries.Data
{
    /// <summary>
    ///     Stores the simulation diffusion data of an <see cref="IParticle" /> ensemble
    /// </summary>
    public readonly struct EnsembleDiffusion
    {
        /// <summary>
        ///     Get the <see cref="IParticle" /> of the ensemble
        /// </summary>
        public IParticle Particle { get; }

        /// <summary>
        ///     Get the diffusion coefficient in X direction as [m^2/s]
        /// </summary>
        public double CoefficientX { get; }

        /// <summary>
        ///     Get the diffusion coefficient in Y direction as [m^2/s]
        /// </summary>
        public double CoefficientY { get; }

        /// <summary>
        ///     Get the diffusion coefficient in Z direction as [m^2/s]
        /// </summary>
        public double CoefficientZ { get; }

        /// <inheritdoc />
        public EnsembleDiffusion(IParticle particle, double coefficientX, double coefficientY, double coefficientZ)
            : this()
        {
            Particle = particle ?? throw new ArgumentNullException(nameof(particle));
            CoefficientX = coefficientX;
            CoefficientY = coefficientY;
            CoefficientZ = coefficientZ;
        }
    }
}