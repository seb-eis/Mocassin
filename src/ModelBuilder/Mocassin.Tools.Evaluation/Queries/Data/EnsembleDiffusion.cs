using System;
using Mocassin.Mathematics.ValueTypes;
using Mocassin.Model.Particles;
using Mocassin.Tools.Evaluation.Helper;

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
        ///     Get the number of particles that belong to the ensemble
        /// </summary>
        public int EnsembleSize { get; }

        /// <summary>
        ///     Get the time in [s] in which the MSD data was produced
        /// </summary>
        public double SimulatedTime { get; }

        /// <summary>
        ///     Get the mean square displacement (MSD) components in [m^2] for x,y,z and radial directions
        /// </summary>
        public (double X, double Y, double Z, double R) Msd { get; }

        /// <summary>
        ///     Get the tracer diffusion coefficient components in [m^2/s] for x,y,z and radial directions
        /// </summary>
        public (double X, double Y, double Z, double R) DiffusionCoefficient { get; }

        /// <inheritdoc />
        public EnsembleDiffusion(IParticle particle, int ensembleSize, double simulatedTime, (double, double, double, double) msd)
            : this()
        {
            Particle = particle ?? throw new ArgumentNullException(nameof(particle));
            EnsembleSize = ensembleSize;
            Msd = msd;
            SimulatedTime = simulatedTime;
            DiffusionCoefficient = GetCoefficients(msd, simulatedTime);
        }

        /// <summary>
        ///     Calculates the (x,y,z,r) diffusion coefficient tuple using the MSDs and time info
        /// </summary>
        /// <param name="msds"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        private static (double, double, double, double) GetCoefficients((double X, double Y, double Z, double R) msds, double time)
        {
            var (x, y, z, r) = msds;
            var xD = Equations.Diffusion.MeanSquareToCoefficient1D(x, time);
            var yD = Equations.Diffusion.MeanSquareToCoefficient1D(y, time);
            var zD = Equations.Diffusion.MeanSquareToCoefficient1D(z, time);
            var rD = Equations.Diffusion.MeanSquareToCoefficient3D(r, time);
            return (xD, yD, zD, rD);
        }
    }
}