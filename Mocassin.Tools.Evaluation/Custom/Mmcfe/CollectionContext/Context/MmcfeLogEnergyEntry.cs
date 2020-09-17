using System;
using System.ComponentModel.DataAnnotations.Schema;
using Mocassin.Model.Translator;

namespace Mocassin.Tools.Evaluation.Custom.Mmcfe
{
    /// <summary>
    ///     Stores <see cref="MmcfeEnergyState" /> calculation results belonging to a single <see cref="MmcfeLogMetaEntry" />
    /// </summary>
    public class MmcfeLogEnergyEntry : EntityBase
    {
        /// <summary>
        ///     Get or set the <see cref="MmcfeExtendedLogEntry" /> navigation property
        /// </summary>
        public MmcfeExtendedLogEntry LogEntry { get; set; }

        /// <summary>
        ///     Get or set the context id for <see cref="LogEntry" />
        /// </summary>
        [Column("LogEntryId"), ForeignKey(nameof(LogEntry))]
        public int LogEntryId { get; set; }

        /// <summary>
        ///     Get or set the alpha value
        /// </summary>
        [Column("Alpha")]
        public double Alpha { get; set; }

        /// <summary>
        ///     Get or set the temperature in [K]
        /// </summary>
        [Column("Temperature")]
        public double Temperature { get; set; }

        /// <summary>
        ///     Get or set the inner energy in [eV]
        /// </summary>
        [Column("InnerEnergy")]
        public double InnerEnergy { get; set; }

        /// <summary>
        ///     Get or set the free energy in [eV]
        /// </summary>
        [Column("FreeEnergy")]
        public double FreeEnergy { get; set; }

        /// <summary>
        ///     Creates a new <see cref="MmcfeLogEnergyEntry" /> from <see cref="MmcfeEnergyState" /> and
        ///     <see cref="MmcfeExtendedLogEntry" />
        /// </summary>
        /// <param name="energyState"></param>
        /// <param name="logEntry"></param>
        public static MmcfeLogEnergyEntry Create(in MmcfeEnergyState energyState, MmcfeExtendedLogEntry logEntry)
        {
            if (logEntry == null) throw new ArgumentNullException(nameof(logEntry));
            return new MmcfeLogEnergyEntry
            {
                Alpha = energyState.Alpha,
                Temperature = energyState.Temperature,
                InnerEnergy = energyState.InnerEnergy,
                FreeEnergy = energyState.FreeEnergy,
                LogEntry = logEntry
            };
        }

        /// <summary>
        ///     Gets the data as an <see cref="MmcfeEnergyState" /> struct
        /// </summary>
        /// <returns></returns>
        public MmcfeEnergyState AsStruct() => new MmcfeEnergyState(Alpha, Temperature, FreeEnergy, InnerEnergy);
    }
}