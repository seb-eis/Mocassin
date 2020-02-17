using System;
using System.Runtime.InteropServices;
using Mocassin.Model.Translator.Routines;
using Mocassin.Tools.UAccess.Readers.Base;

namespace Mocassin.Tools.UAccess.Readers
{
    /// <summary>
    ///     Provides fast read only access to the unmanaged binary output of the MMCFE routine of the 'C' Mocassin.Simulator
    /// </summary>
    /// <remarks>The access is context free and requires the affiliated model context for evaluation</remarks>
    public class MmcfeLogReader : IDisposable
    {
        /// <summary>
        ///     Get the <see cref="BinaryStructureReader" /> for the <see cref="CMmcfeParams" /> state bytes
        /// </summary>
        private BinaryStructureReader ParameterReader { get; }

        /// <summary>
        ///     Get the <see cref="McsContentReader" /> for the simulation state
        /// </summary>
        public McsContentReader StateReader { get; }

        /// <summary>
        ///     Get the <see cref="DynamicHistogramReader" /> for the energy histogram
        /// </summary>
        public DynamicHistogramReader EnergyHistogramReader { get; }

        /// <summary>
        ///     Creates a new <see cref="MmcfeLogReader" /> that provides read only access to all relevant components of the MMCFE
        ///     log
        /// </summary>
        /// <param name="parameterReader"></param>
        /// <param name="stateReader"></param>
        /// <param name="energyHistogramReader"></param>
        private MmcfeLogReader(BinaryStructureReader parameterReader, McsContentReader stateReader, DynamicHistogramReader energyHistogramReader)
        {
            ParameterReader = parameterReader ?? throw new ArgumentNullException(nameof(parameterReader));
            StateReader = stateReader ?? throw new ArgumentNullException(nameof(stateReader));
            EnergyHistogramReader = energyHistogramReader ?? throw new ArgumentNullException(nameof(energyHistogramReader));
        }

        /// <inheritdoc />
        public void Dispose()
        {
            ParameterReader.Dispose();
            StateReader.Dispose();
            EnergyHistogramReader.Dispose();
        }

        /// <summary>
        ///     Reads the byte representation of the parameters as an <see cref="CMmcfeParams" />
        /// </summary>
        /// <returns></returns>
        public ref CMmcfeParams ReadParameters()
        {
            return ref ParameterReader.ReadAs<CMmcfeParams>(0);
        }

        /// <summary>
        ///     Creates a new <see cref="MmcfeLogReader" /> for the provided set of binary representations and performs consistency
        ///     checks
        /// </summary>
        /// <param name="stateBytes"></param>
        /// <param name="histogramBytes"></param>
        /// <param name="parameterBytes"></param>
        /// <returns></returns>
        public static MmcfeLogReader Create(byte[] stateBytes, byte[] histogramBytes, byte[] parameterBytes)
        {
            var histogramReader = DynamicHistogramReader.Create(histogramBytes);
            var stateReader = McsContentReader.Create(stateBytes);
            if (parameterBytes.Length != Marshal.SizeOf<CMmcfeParams>()) throw new InvalidOperationException("Parameter byte array has wrong size.");
            var parameterReader = new BinaryStructureReader(parameterBytes);
            return new MmcfeLogReader(parameterReader, stateReader, histogramReader);
        }
    }
}