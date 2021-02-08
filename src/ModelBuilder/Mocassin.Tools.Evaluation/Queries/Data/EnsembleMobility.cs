using System;
using Mocassin.Model.Particles;

namespace Mocassin.Tools.Evaluation.Queries.Data
{
    /// <summary>
    ///     Contains the mobility data for a <see cref="IParticle" /> ensemble
    /// </summary>
    public class EnsembleMobility
    {
        /// <summary>
        ///     Get the <see cref="IParticle" /> that forms the ensemble
        /// </summary>
        public IParticle Particle { get; }

        /// <summary>
        ///     Get the ionic mobility value in [m/(s*V)]
        /// </summary>
        public double IonicMobility { get; }

        /// <summary>
        ///     Get the ensemble conductivity in [S/m]
        /// </summary>
        public double Conductivity { get; }

        /// <summary>
        ///     Get the ensemble conductivity in [S/m] for a pseudo charge of +1
        /// </summary>
        public double NormalizedConductivity { get; }

        /// <summary>
        ///     Get the effective diffusion coefficient in [m^2/s] as calculated using the Nernst-Einstein formalism without correlation factors
        /// </summary>
        /// <value></value>
        public double EffectiveDiffusionCoefficient { get; }

        /// <summary>
        ///     Creates a new <see cref="EnsembleMobility"/> for an <see cref="IParticle"/>
        /// </summary>
        /// <param name="particle"></param>
        /// <param name="ionicMobility"></param>
        /// <param name="conductivity"></param>
        /// <param name="normalizedConductivity"></param>
        /// <param name="effectiveDiffusionCoefficient"></param>
        public EnsembleMobility(IParticle particle, double ionicMobility, double conductivity, double normalizedConductivity, double effectiveDiffusionCoefficient)
        {
            Particle = particle ?? throw new ArgumentNullException(nameof(particle));
            IonicMobility = ionicMobility;
            Conductivity = conductivity;
            NormalizedConductivity = normalizedConductivity;
            EffectiveDiffusionCoefficient = effectiveDiffusionCoefficient;
        }
    }
}