using Mocassin.Model.Particles;

namespace Mocassin.Tools.Evaluation.Queries.Data
{
    /// <summary>
    ///     Stores the ensemble correlation data for two <see cref="IParticle" /> ensembles of a simulation
    /// </summary>
    public class EnsembleCorrelationData
    {
        /// <summary>
        ///     Get the <see cref="IParticle" /> of the first ensemble
        /// </summary>
        public IParticle Particle1 { get; }

        /// <summary>
        ///     Get the <see cref="IParticle" /> of the second ensemble
        /// </summary>
        public IParticle Particle2 { get; }

        /// <summary>
        ///     Get the number of particles in the first ensemble
        /// </summary>
        public int EnsembleSize1 { get; }

        /// <summary>
        ///     Get the number of particles in the second ensemble
        /// </summary>
        public int EnsembleSize2 { get; }

        /// <summary>
        ///     Get the sum of the two particles sum(r_i * r_j) correlation in [m^2]
        /// </summary>
        public double RiRj { get; }

        /// <summary>
        ///     Get the sum of the squared displacement of the particle (r_i * r_i) in [m^2] (IS zero if two different particles)
        /// </summary>
        public double RiRi { get; }

        /// <summary>
        ///     Get the affiliated Onsager coefficient (L_ii for same particle, L_ij for different particle)
        /// </summary>
        public double OnsagerCoefficient { get; }

        /// <summary>
        ///     Get the simulated tim ein [s]
        /// </summary>
        public double SimulatedTime { get; }

        /// <summary>
        ///     Get the cell volume in [1/m^3]
        /// </summary>
        public double CellVolume { get; }

        /// <summary>
        ///     Creates a new <see cref="EnsembleCorrelationData" />
        /// </summary>
        /// <param name="riRi"></param>
        /// <param name="riRj"></param>
        /// <param name="onsagerCoefficient"></param>
        /// <param name="cellVolume"></param>
        /// <param name="simulatedTime"></param>
        /// <param name="ensembleSize1"></param>
        /// <param name="ensembleSize2"></param>
        /// <param name="particle1"></param>
        /// <param name="particle2"></param>
        public EnsembleCorrelationData(double riRi, double riRj, double onsagerCoefficient, double cellVolume, double simulatedTime, int ensembleSize1, int ensembleSize2, IParticle particle1,
            IParticle particle2)
        {
            RiRj = riRj;
            EnsembleSize1 = ensembleSize1;
            Particle1 = particle1;
            Particle2 = particle2;
            CellVolume = cellVolume;
            SimulatedTime = simulatedTime;
            RiRi = riRi;
            EnsembleSize2 = ensembleSize2;
            OnsagerCoefficient = onsagerCoefficient;
        }

        /// <summary>
        ///     Gets the mean value of r_i * r_i
        /// </summary>
        /// <returns></returns>
        public double GetMeanRiRi() => RiRi / EnsembleSize1;

        /// <summary>
        ///     Gets the mean value of r_i * r_j
        /// </summary>
        /// <returns></returns>
        public double GetMeanRiRj() => Particle1.Equals(Particle2) ? RiRj / (EnsembleSize1 * (EnsembleSize1 - 1)) : RiRj / (EnsembleSize1 * EnsembleSize2);
    }
}