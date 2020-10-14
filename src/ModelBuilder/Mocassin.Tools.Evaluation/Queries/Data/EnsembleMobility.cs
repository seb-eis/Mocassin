using System;
using Mocassin.Model.Particles;

namespace Mocassin.Tools.Evaluation.Queries.Data
{
    /// <summary>
    ///     Contains the mobility data for a <see cref="IParticle" /> ensemble
    /// </summary>
    public readonly struct EnsembleMobility
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

        /// <inheritdoc />
        public EnsembleMobility(IParticle particle, double ionicMobility, double conductivity, double normalizedConductivity)
            : this()
        {
            Particle = particle ?? throw new ArgumentNullException(nameof(particle));
            IonicMobility = ionicMobility;
            Conductivity = conductivity;
            NormalizedConductivity = normalizedConductivity;
        }
    }
}