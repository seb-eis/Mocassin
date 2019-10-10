using System;
using System.Collections.Generic;
using System.Linq;
using Mocassin.Mathematics.Extensions;
using Mocassin.Mathematics.ValueTypes;

namespace Mocassin.Tools.Evaluation.Queries.Base
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
            public static double BlotzmannEv = 8.617333262145e-5;
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
                double time)
            {
                return meanShift * normField / (fieldModulus * time);
            }

            /// <summary>
            ///     Calculates the mobility in field direction from mean shift, electric filed vector and time information
            /// </summary>
            /// <param name="meanShift"></param>
            /// <param name="field"></param>
            /// <param name="time"></param>
            /// <returns></returns>
            public static double DisplacementToMobility(in Cartesian3D meanShift, in Cartesian3D field, double time)
            {
                return DisplacementToMobility(meanShift, field.GetNormalized(), field.GetLength(), time);
            }

            /// <summary>
            ///     Calculates the conductivity value from mobility, charge number and particle density
            /// </summary>
            /// <param name="mobility"></param>
            /// <param name="chargeNumber"></param>
            /// <param name="particleDensity"></param>
            /// <returns></returns>
            public static double MobilityToConductivity(double mobility, double chargeNumber, double particleDensity)
            {
                return mobility * chargeNumber * particleDensity * Constants.ElementalCharge;
            }
        }

        public static class Diffusion
        {
            /// <summary>
            ///     Get the diffusion coefficient through mean square displacement and time in 1 dimension
            /// </summary>
            /// <param name="shift"></param>
            /// <param name="time"></param>
            /// <returns></returns>
            public static double MeanSquareToCoefficient(double shift, double time)
            {
                return shift / (2 * time);
            }

            /// <summary>
            ///     Get the diffusion coefficient by mean square displacement vector and time in X,Y,Z directions
            /// </summary>
            /// <param name="vector"></param>
            /// <param name="time"></param>
            /// <returns></returns>
            public static (double X, double Y, double Z) MeanSquareToCoefficient(in Cartesian3D vector, double time)
            {
                return (MeanSquareToCoefficient(vector.X, time),
                    MeanSquareToCoefficient(vector.Y, time),
                    MeanSquareToCoefficient(vector.Z, time));
            }
        }

        public static class Statistics
        {
            /// <summary>
            ///     Calculates the average values with standard deviation of a value sequence using a selector
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="source"></param>
            /// <param name="selector"></param>
            /// <returns></returns>
            public static (double Average, double Deviation) Average<T>(IEnumerable<T> source, Func<T, double> selector)
            {
                return Average(source as IReadOnlyCollection<T> ?? source.ToList(), selector);
            }

            /// <summary>
            ///     Calculates the average values with standard deviation of a value sequence using a selector
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="source"></param>
            /// <param name="selector"></param>
            /// <returns></returns>
            public static (double Average, double Deviation) Average<T>(IReadOnlyCollection<T> source, Func<T, double> selector)
            {
                var average = source.Select(selector).Sum() / source.Count;
                var deviation = Math.Sqrt(source.Select(selector).Sum(x => Math.Pow(x - average, 2)) / (source.Count - 1));
                return (average, deviation);
            }

            /// <summary>
            ///     Calculates an onsager coefficient utilizing the Kubo-Green formalism for cubic systems
            /// </summary>
            /// <param name="lhs"></param>
            /// <param name="rhs"></param>
            /// <param name="volume"></param>
            /// <param name="time"></param>
            /// <param name="temperature"></param>
            /// <returns></returns>
            public static double CubicOnsagerKuboGreen(in Cartesian3D lhs, in Cartesian3D rhs, double volume, double time,
                double temperature)
            {
                return lhs.GetLength() * rhs.GetLength() / (6 * volume * temperature * time * Constants.BlotzmannEv);
            }
        }
    }
}