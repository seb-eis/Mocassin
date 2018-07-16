﻿using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ICon.Model.Translator
{
    /// <summary>
    /// Status flags of a single mccs job
    /// </summary>
    public enum MccsJobStatus
    {
        Unfinished = 0, Finished = 0b1, Error = 0b10, Stuck = 0b100, DirtyBit = 0b1000
    }

    /// <summary>
    /// Mccs job entity that describes a single simulation job belonging to a parent simulation definition
    /// </summary>
    public class MccsJob : EntityBase
    {
        /// <summary>
        /// The context key of the mccs parent
        /// </summary>
        [ForeignKey(nameof(MccsParent))]
        public int ParentKey { get; set; }

        /// <summary>
        /// The context key of the mcss job result
        /// </summary>
        [ForeignKey(nameof(MccsJobResult))]
        public int JobResultKey { get; set; }

        /// <summary>
        /// The mccs parent object. Defines data translation of the simulation data components
        /// </summary>
        public MccsParent MccsParent { get; set; }

        /// <summary>
        /// The mccs job result object. Contains simulation result information created by the simulator
        /// </summary>
        public MccsJobResult MccsJobResult { get; set; }

        /// <summary>
        /// Status flag of the job. Dirty status bit indicates that a simulation is running or crashed
        /// </summary>
        public MccsJobStatus Status { get; set; }

        /// <summary>
        /// The absolute simulation temperature value in [K]
        /// </summary>
        public double Temperature { get; set; }

        /// <summary>
        /// The electric field magnitude in [k*T]
        /// </summary>
        public double ElectricField { get; set; }

        /// <summary>
        /// The encoded simulation lattice string description
        /// </summary>
        public string LatticeString { get; set; }
    }
}
