using System;
using System.IO;
using Mocassin.Tools.Evaluation.PlotData;

namespace Mocassin.Tools.Evaluation.Queries
{
    /// <summary>
    ///     Describes the results of a <see cref="SiteCoordination" /> for a single distance as a site coordination
    /// </summary>
    public readonly struct CoordinationShell : IDatLineWritable
    {
        /// <summary>
        ///     The distance in [Ang]
        /// </summary>
        public double DistanceInAng { get; }

        /// <summary>
        ///     The affiliated coordination number
        /// </summary>
        public double CoordinationNumber { get; }

        /// <summary>
        ///     Creates new <see cref="CoordinationShell" />
        /// </summary>
        /// <param name="distanceInAng"></param>
        /// <param name="coordinationNumber"></param>
        public CoordinationShell(double distanceInAng, double coordinationNumber)
        {
            DistanceInAng = distanceInAng;
            CoordinationNumber = coordinationNumber;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"(Distance: {DistanceInAng} Ang; CN: {CoordinationNumber})";
        }

        /// <inheritdoc />
        public string ToDatLine(string format)
        {
            return string.Format(format, DistanceInAng, CoordinationNumber);
        }

        /// <summary>
        ///     Creates a dummy entry with zeros and a distances offset by the provided value
        /// </summary>
        /// <param name="distanceOffset"></param>
        /// <returns></returns>
        public CoordinationShell MakeDummy(double distanceOffset)
        {
            return new CoordinationShell(DistanceInAng + distanceOffset, 0);
        }
    }
}