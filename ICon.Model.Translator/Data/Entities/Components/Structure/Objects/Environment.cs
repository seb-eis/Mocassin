using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;

namespace ICon.Model.Translator
{
    /// <summary>
    /// Environment entity that fully describe cell position and its surroundings for the simulation
    /// </summary>
    public class Environment : EntityBase
    {
        /// <summary>
        /// The mccs structure info entity key
        /// </summary>
        [ForeignKey(nameof(MccsStructureInfo))]
        public int MccsStructureInfoId { get; set; }

        /// <summary>
        /// Navigation property for the mccs structure info this environment belongs to
        /// </summary>
        public MccsStructureInfo MccsStructureInfo { get; set; }

        /// <summary>
        /// The list of environment shells that describe the environment
        /// </summary>
        public List<EnvironmentShell> EnvironmentShells { get; set; }

        /// <summary>
        /// The list of environment clusters that descirbe more complex energy portions of the environment
        /// </summary>
        public List<EnvironmentCluster> EnvironmentClusters { get; set; }

        /// <summary>
        /// Boolean flag that defines if the position is stable
        /// </summary>
        public bool IsStable { get; set; }

        /// <summary>
        /// The index of the cell position this environment belongs to
        /// </summary>
        public int CellPositionIndex { get; set; }

        /// <summary>
        /// The fractional vector component of the base position in A direction
        /// </summary>
        public double VecCompA { get; set; }

        /// <summary>
        /// The fractional vector component of the base position in B direction
        /// </summary>
        public double VecCompB { get; set; }

        /// <summary>
        /// The fractional vector component of the base position in C direction
        /// </summary>
        public double VecCompC { get; set; }
    }
}
