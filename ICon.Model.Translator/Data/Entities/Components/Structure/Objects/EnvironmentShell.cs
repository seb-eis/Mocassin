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
        [ForeignKey(nameof(CellPositionEntity))]
        public int CellPositionEntityId { get; set; }

        /// <summary>
        /// Environment entity that this pair energy table belongs to
        /// </summary>
        Environment CellPositionEntity { get; set; }

        /// <summary>
        /// Pair energy map entity key
        /// </summary>
        [ForeignKey(nameof(PairEnergyMatrix))]
        public int PairEnergyMatrixId { get; set; }

        /// <summary>
        /// Navigation property for the pair energy map affiliated with this shell
        /// </summary>
        PairEnergyMatrix PairEnergyMatrix { get; set; }

        /// <summary>
        /// The index of the shell within its affiliated environment
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// The distance of this shell from its core position
        /// </summary>
        public double CoreDistance { get; set; }

        /// <summary>
        /// The list of environment positions that describe the shell
        /// </summary>
        List<EnvironmentPosition> EnvironmentPositions { get; set; }
    }
}
