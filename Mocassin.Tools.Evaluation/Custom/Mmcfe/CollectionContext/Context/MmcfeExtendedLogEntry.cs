using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Mocassin.Framework.Extensions;
using Mocassin.Tools.UAccess.Readers;

namespace Mocassin.Tools.Evaluation.Custom.Mmcfe
{
    /// <summary>
    ///     Extended version of the <see cref="MmcfeLogEntry" /> that enables the 1 to N relation with a
    ///     <see cref="MmcfeLogMetaEntry" />
    /// </summary>
    public class MmcfeExtendedLogEntry : MmcfeLogEntry
    {
        /// <summary>
        ///     Get or set the <see cref="MmcfeLogMetaEntry" /> navigation property
        /// </summary>
        public MmcfeLogMetaEntry MetaEntry { get; set; }

        /// <summary>
        ///     Get or set the context id of the <see cref="MetaEntry" />
        /// </summary>
        [Column("MetaEntryId")]
        [ForeignKey(nameof(MetaEntry))]
        public int MetaEntryId { get; set; }

        /// <summary>
        ///     Creates an <see cref="MmcfeExtendedLogEntry" /> from a <see cref="MmcfeLogEntry" /> and
        ///     <see cref="MmcfeLogMetaEntry" />. This implicitly adds the particle count string to the meta entry
        /// </summary>
        /// <param name="logEntry"></param>
        /// <param name="metaEntry"></param>
        /// <param name="excludeRawData"></param>
        /// <returns></returns>
        public static MmcfeExtendedLogEntry Create(MmcfeLogEntry logEntry, MmcfeLogMetaEntry metaEntry, bool excludeRawData = false)
        {
            if (logEntry == null) throw new ArgumentNullException(nameof(logEntry));
            if (metaEntry == null) throw new ArgumentNullException(nameof(metaEntry));

            metaEntry.ParticleCountInfo = BuildParticleCountString(logEntry.StateBytes);
            return new MmcfeExtendedLogEntry
            {
                MetaEntry = metaEntry,
                Alpha = logEntry.Alpha,
                TimeStamp = logEntry.TimeStamp,
                HistogramBytes = excludeRawData ? null : logEntry.HistogramBytes,
                ParameterBytes = excludeRawData ? null : logEntry.ParameterBytes,
                StateBytes = excludeRawData ? null : logEntry.StateBytes
            };
        }

        /// <summary>
        ///     Reads the provided bytes as a simulation state and creates the particle count <see cref="string" />
        /// </summary>
        /// <param name="stateBytes"></param>
        /// <returns></returns>
        private static string BuildParticleCountString(byte[] stateBytes)
        {
            var buffer = new int[64];
            byte maxId = 0;
            using (var mcsContentReader = McsContentReader.Create(stateBytes))
            {
                foreach (var item in mcsContentReader.ReadLattice())
                {
                    buffer[item]++;
                    maxId = Math.Max(maxId, item);
                }
            }

            var stringBuilder = new StringBuilder(100);
            for (var i = 0; i <= maxId; i++)
            {
                stringBuilder.Append(buffer[i]);
                stringBuilder.Append(',');
            }

            stringBuilder.PopBack(1);
            return stringBuilder.ToString();
        }
    }
}