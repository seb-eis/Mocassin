using System;
using System.ComponentModel.DataAnnotations.Schema;
using Mocassin.Model.Translator;

namespace Mocassin.Tools.Evaluation.Custom.Mmcfe
{
    /// <summary>
    ///     The <see cref="EntityBase" /> implementation for MMCFE routine log entities
    /// </summary>
    public class MmcfeLogEntry : EntityBase
    {
        /// <summary>
        ///     Get or set the <see cref="DateTime" /> log time stamp
        /// </summary>
        [Column("TimeStamp")]
        public DateTime TimeStamp { get; set; }

        /// <summary>
        ///     Get or set the simulation state binary representation
        /// </summary>
        [Column("State")]
        public byte[] StateBytes { get; set; }

        /// <summary>
        ///     Get or set the energy histogram binary representation
        /// </summary>
        [Column("Histogram")]
        public byte[] HistogramBytes { get; set; }

        /// <summary>
        ///     Get or set the parameter state binary representation
        /// </summary>
        [Column("ParamState")]
        public byte[] ParameterBytes { get; set; }

        /// <summary>
        ///     Get or set the alpha value
        /// </summary>
        [Column("Alpha")]
        public double Alpha { get; set; }
    }
}