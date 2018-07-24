using System;
using System.Collections.Generic;
using System.Text;
using ICon.Framework.Collections;
using System.ComponentModel.DataAnnotations.Schema;

namespace ICon.Model.Translator
{
    /// <summary>
    /// Cell position entity that describes a simulated
    /// </summary>
    public class Environment : EntityBase
    {
        /// <summary>
        /// The mcs structure info entity key
        /// </summary>
        [ForeignKey(nameof(McsStructure))]
        public int McsStructureId { get; set; }

        /// <summary>
        /// Navigation property for the mcs structure info this environment belongs to
        /// </summary>
        public McsStructure McsStructure { get; set; }

        /// <summary>
        /// The cell position entity id
        /// </summary>
        [ForeignKey(nameof(CellPosition))]
        public int CellPositionId { get; set; }

        /// <summary>
        /// Navigation property for the cell position this environment belongs to
        /// </summary>
        public CellPosition CellPosition { get; set; }

        /// <summary>
        /// The list of environment shells that describe the environment
        /// </summary>
        public List<EnvironmentShell> EnvironmentShells { get; set; }

        /// <summary>
        /// The list of environment clusters that descirbe more complex energy portions of the environment
        /// </summary>
        public List<EnvironmentCluster> EnvironmentClusters { get; set; }
    }
}
