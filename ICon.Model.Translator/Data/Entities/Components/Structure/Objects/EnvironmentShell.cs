using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;

namespace ICon.Model.Translator
{
    /// <summary>
    /// Define a single environment shell that contains multiple positions that are modelled through the same pair energy delta table
    /// </summary>
    public class EnvironmentShell : EntityBase
    {
        /// <summary>
        /// The environment entity key
        /// </summary>
        [ForeignKey(nameof(Environment))]
        public int EnvironmentId { get; set; }

        /// <summary>
        /// Environment entity that this pair energy table belongs to
        /// </summary>
        Environment Environment { get; set; }

        /// <summary>
        /// The pair energy table entity key
        /// </summary>
        [ForeignKey(nameof(PairEnergyTable))]
        public int PairEnergyTableId { get; set; }

        /// <summary>
        /// Navigation property for the pair energy table that belongs to this shell
        /// </summary>
        PairEnergyTable PairEnergyTable { get; set; }

        /// <summary>
        /// The list of environment positions that describe the shell
        /// </summary>
        List<EnvironmentPosition> EnvironmentPositions { get; set; }
    }
}
