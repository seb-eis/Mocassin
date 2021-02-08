using System;
using System.Collections.Generic;
using System.Linq;
using Mocassin.Mathematics.Extensions;
using Mocassin.Mathematics.ValueTypes;

namespace Mocassin.Tools.Evaluation.Helper
{
    /// <summary>
    ///     Provides equations to calculate physical properties required by the evaluation system
    /// </summary>
    public static class Equations
    {
        /// <summary>
        ///     Contains the constants used in the equations
        /// </summary>
        public static class Constants
        {
            /// <summary>
            ///     The elemental charge in [C]
            /// </summary>
            public static double ElementalCharge = 1.602176634e-19;

            /// <summary>
            ///     The boltzmann constant in [eV/K]
            /// </summary>
            public static double BoltzmannEv = 8.617333262145e-05;

            /// <summary>
            ///     The boltzmann constant in [J/K]
            /// </summary>
            public static double BoltzmannSi = BoltzmannEv * ElementalCharge;
        }

        /// <summary>
        ///     Contains equations for mobility calculations
        /// </summary>
        public static class Mobility
        {
            /// <summary>
            ///     Calculates the mobility in field direction from mean shift, normalized electric field vector, field modulus and
            ///     time information
            /// </summary>
            /// <param name="meanShift"></param>
            /// <param name="normField"></param>
            /// <param name="fieldModulus"></param>
            /// <param name="time"></param>
            /// <returns></returns>
            public static double DisplacementToMobility(in Cartesian3D meanShift, in Cartesian3D normField, double fieldModulus,
                double time) =>
                meanShift * normField / (fieldModulus * time);

            /// <summary>
            ///     Calculates the mobility in field direction from mean shift, electric filed vector and time information
            /// </summary>
            /// <param name="meanShift"></param>
            /// <param name="field"></param>
            /// <param name="time"></param>
            /// <returns></returns>
            public static double DisplacementToMobility(in Cartesian3D meanShift, in Cartesian3D field, double time) =>
                DisplacementToMobility(meanShift, field.GetNormalized(), field.GetLength(), time);

            /// <summary>
            ///     Calculates the conductivity value from mobility, charge number and particle density
            /// </summary>
            /// <param name="mobility"></param>
            /// <param name="chargeNumber"></param>
            /// <param name="particleDensity"></param>
            /// <returns></returns>
            public static double MobilityToConductivity(double mobility, double chargeNumber, double particleDensity) =>
                mobility * chargeNumber * particleDensity * Constants.ElementalCharge;

            /// <summary>
            ///     Calculates the effective diffusion coefficient froma mobility using the Nernst-Einstein formalism
            /// </summary>
            /// <param name="mobility"></param>
            /// <param name="temperature"></param>
            /// <param name="chargeNumber"></param>
            /// <param name="correlationFactor"></param>
            /// <returns></returns>
            public static double MobilityToEffectiveDiffusionCoefficient(double mobility, double temperature, double chargeNumber, double correlationFactor = 1.0)
                => mobility * temperature * Constants.BoltzmannEv * correlationFactor * chargeNumber;
        }

        /// <summary>
        ///     Defines equations related to diffusion
        /// </summary>
        public static class Diffusion
        {
            /// <summary>
            ///     Get the diffusion coefficient through mean square displacement and time in 1 dimension
            /// </summary>
            /// <param name="shift"></param>
            /// <param name="time"></param>
            /// <returns></returns>
            public static double MeanSquareToCoefficient1D(double shift, double time) => shift / (2.0 * time);

            /// <summary>
            ///     Get the diffusion coefficient through mean square displacement and time in 3 dimensions
            /// </summary>
            /// <param name="shift"></param>
            /// <param name="time"></param>
            /// <returns></returns>
            public static double MeanSquareToCoefficient3D(double shift, double time) => shift / (6.0 * time);

            /// <summary>
            ///     Get the diffusion coefficient by mean square displacement vector and time in X,Y,Z directions
            /// </summary>
            /// <param name="vector"></param>
            /// <param name="time"></param>
            /// <returns></returns>
            public static (double X, double Y, double Z) MeanSquareToCoefficient1D(in Cartesian3D vector, double time) =>
                (MeanSquareToCoefficient1D(vector.X, time),
                    MeanSquareToCoefficient1D(vector.Y, time),
                    MeanSquareToCoefficient1D(vector.Z, time));

            /// <summary>
            ///     Calculates the tracer correlation factor using the mean r_i * r_i correlation for a specific average jump length
            /// </summary>
            /// <param name="meanRiRi"></param>
            /// <param name="avgMigrationRate"></param>
            /// <param name="time"></param>
            /// <param name="avgJumpLength"></param>
            /// <returns></returns>
            public static double CalculateTracerCorrelationFactor(double meanRiRi, double avgMigrationRate, double time, double avgJumpLength)
            {
                return meanRiRi / (avgMigrationRate * time * avgJumpLength * avgJumpLength);
            }

            /// <summary>
            ///     Calculates the tracer correlation factor using the mean r_j * r_i correlation for a specific average jump length
            /// </summary>
            /// <param name="meanRiRj"></param>
            /// <param name="avgMigrationRate"></param>
            /// <param name="time"></param>
            /// <param name="ensembleSize"></param>
            /// <param name="avgJumpLength"></param>
            /// <returns></returns>
            public static double CalculateTwoParticleCorrelationFactor(double meanRiRj, double avgMigrationRate, double time, int ensembleSize, double avgJumpLength) =>
                ensembleSize * CalculateTracerCorrelationFactor(meanRiRj, avgMigrationRate, time, avgJumpLength);

            /// <summary>
            ///     Calculates the collective correlation factor from r_i * r_j of two ensembles and an average jump length
            /// </summary>
            /// <param name="riRj"></param>
            /// <param name="avgMigrationRate"></param>
            /// <param name="time"></param>
            /// <param name="ensembleSize"></param>
            /// <param name="avgJumpLength"></param>
            /// <returns></returns>
            public static double CalculateCollectiveCorrelationFactors(double riRj, double avgMigrationRate, double time, int ensembleSize, double avgJumpLength)
            {
                return riRj / (ensembleSize * avgMigrationRate * time * avgJumpLength * avgJumpLength);
            }
        }

        /// <summary>
        ///     Provides statistics helper methods
        /// </summary>
        public static class Statistics
        {
            /// <summary>
            ///     Calculates the average values with standard deviation of a value sequence using a selector
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="source"></param>
            /// <param name="selector"></param>
            /// <returns></returns>
            public static (double Average, double Deviation) AverageWithDeviation<T>(IEnumerable<T> source, Func<T, double> selector) =>
                AverageWithDeviation(source as IReadOnlyCollection<T> ?? source.ToList(), selector);

            /// <summary>
            ///     Calculates the average values with standard deviation of a value sequence using a selector
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="source"></param>
            /// <param name="selector"></param>
            /// <returns></returns>
            public static (double Average, double Deviation) AverageWithDeviation<T>(IReadOnlyCollection<T> source, Func<T, double> selector)
            {
                var average = source.Select(selector).Sum() / source.Count;
                var deviation = Math.Sqrt(source.Select(selector).Sum(x => Math.Pow(x - average, 2)) / (source.Count - 1));
                return (average, deviation);
            }

            /// <summary>
            ///     Calculates the average values with standard error of measurement of a value sequence using a selector
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="source"></param>
            /// <param name="selector"></param>
            /// <returns></returns>
            public static (double Average, double Error) AverageWithSem<T>(IEnumerable<T> source, Func<T, double> selector) =>
                AverageWithSem(source as IReadOnlyCollection<T> ?? source.ToList(), selector);

            /// <summary>
            ///     Calculates the average values with standard error of measurement of a value sequence using a selector
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="source"></param>
            /// <param name="selector"></param>
            /// <returns></returns>
            public static (double Average, double Error) AverageWithSem<T>(IReadOnlyCollection<T> source, Func<T, double> selector)
            {
                var (average, deviation) = AverageWithDeviation(source, selector);
                return (average, deviation / Math.Sqrt(source.Count));
            }

            /// <summary>
            ///     Calculates an onsager coefficient for all directions utilizing the Kubo-Green formalism for cubic systems
            /// </summary>
            /// <param name="ri"></param>
            /// <param name="rj"></param>
            /// <param name="volume"></param>
            /// <param name="time"></param>
            /// <param name="temperature"></param>
            /// <returns></returns>
            public static double CalcOnsagerR3FromTotalEnsembleShift(in Cartesian3D ri, in Cartesian3D rj, double volume, double time,
                double temperature) =>
                ri * rj / (6.0 * volume * temperature * time * Constants.BoltzmannSi);

            /// <summary>
            ///     Calculates the Lij onsager coefficient for R^3 utilizing the movement correlation of a simulation (different
            ///     ensembles)
            /// </summary>
            /// <param name="riRj"></param>
            /// <param name="time"></param>
            /// <param name="temperature"></param>
            /// <param name="cellVolume"></param>
            /// <returns></returns>
            public static double CalcOnsagerR3FromCorrelationLij(double riRj, double time, double temperature, double cellVolume) =>
                riRj / (6.0 * time * cellVolume * Constants.BoltzmannSi * temperature);

            /// <summary>
            ///     Calculates the Lii onsager coefficient for R^3 utilizing the movement correlation of a simulation (same ensemble)
            /// </summary>
            /// <param name="riRi"></param>
            /// <param name="riRj"></param>
            /// <param name="time"></param>
            /// <param name="temperature"></param>
            /// <param name="cellVolume"></param>
            /// <returns></returns>
            public static double CalcOnsagerR3FromCorrelationLii(double riRi, double riRj, double time, double temperature, double cellVolume)
            {
                var snd = 1.0 / (cellVolume * Constants.BoltzmannSi * temperature) * riRi / (6.0 * time);
                var fst = 1.0 / (cellVolume * Constants.BoltzmannSi * temperature) * riRj / (6.0 * time);
                return fst + snd;
            }
        }
    }
}