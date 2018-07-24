using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;
using ICon.Framework.Collections;

namespace ICon.Model.Translator
{
    /// <summary>
    /// Cell position netity that describes a single unit cell position for the simulation
    /// </summary>
    public class CellPosition : EntityBase
    {
        /// <summary>
        /// The mcs structure entity id
        /// </summary>
        [ForeignKey(nameof(McsStructure))]
        public int McsStructureId { get; set; }

        /// <summary>
        /// Navigation property for the mcs structure info this position belongs to
        /// </summary>
        public McsStructure McsStructure { get; set; }

        /// <summary>
        /// The environment entity id
        /// </summary>
        [ForeignKey(nameof(Environment))]
        public int EnvironmentId { get; set; }

        /// <summary>
        /// Navigation property for the environment that defines the surroundings of this position
        /// </summary>
        public Environment Environment { get; set; }

        /// <summary>
        /// The index of this position in the simulation
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Boolean flag that defines if the position is stable (Metastable if false)
        /// </summary>
        public bool IsStable { get; set; }

        /// <summary>
        /// The fractional vector component of the base position in A direction
        /// </summary>
        public double FractionalA { get; set; }

        /// <summary>
        /// The fractional vector component of the base position in B direction
        /// </summary>
        public double FractionalB { get; set; }

        /// <summary>
        /// The fractional vector component of the base position in C direction
        /// </summary>
        public double FractionalC { get; set; }
    }
}
