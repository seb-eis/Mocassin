using System.Runtime.InteropServices;

namespace Mocassin.Tools.UAccess.Readers.McsReader.Components
{
    /// <summary>
    ///     Simulation jump histogram struct that contains energy histogram data from a 'C' Simulator state file
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 4 * 8040, Pack = 8)]
    public readonly struct McsJumpStatistic
    {
        /// <summary>
        ///     Get the <see cref="McsJumpHistogram" /> that contains the sampling of edge energy occurrences
        /// </summary>
        public readonly McsJumpHistogram EdgeEnergyHistogram;

        /// <summary>
        ///     Get the <see cref="McsJumpHistogram" /> that contains the sampling of positive conformation energy
        ///     occurrences
        /// </summary>
        public readonly McsJumpHistogram PositiveConformationEnergyHistogram;

        /// <summary>
        ///     Get the <see cref="McsJumpHistogram" /> that contains the sampling of negative conformation energy
        ///     occurrences
        /// </summary>
        public readonly McsJumpHistogram NegativeConformationEnergyHistogram;

        /// <summary>
        ///     Get the <see cref="McsJumpHistogram" /> that contains the sampling of actual jump energy occurrences
        /// </summary>
        public readonly McsJumpHistogram JumpEnergyHistogram;
    }
}