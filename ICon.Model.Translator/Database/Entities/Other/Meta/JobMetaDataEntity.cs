using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mocassin.Model.Translator.Database.Entities.Other.Meta
{
    /// <summary>
    ///     Entity that stores meta data about <see cref="SimulationJobModel" /> entities to provide a simulation parameter to job mapping in the simulation database
    /// </summary>
    public class JobMetaDataEntity : EntityBase
    {
        /// <summary>
        ///     Get or set the <see cref="SimulationJobModel" /> navigation entity
        /// </summary>
        public SimulationJobModel JobModel { get; set; }

        /// <summary>
        ///     Get or set the <see cref="SimulationJobModel" /> context id
        /// </summary>
        [Column("JobModelId")]
        [ForeignKey(nameof(JobModel))]
        public int JobModelId { get; set; }

        /// <summary>
        ///     Get or set the source job collection name
        /// </summary>
        [Column("CollectionName")]
        public string CollectionName { get; set; }

        /// <summary>
        ///     Get or set the job configuration name
        /// </summary>
        [Column("ConfigName")]
        public string ConfigName { get; set; }

        /// <summary>
        ///     Get or set the relative job index due to job multiplication in the configuration
        /// </summary>
        [Column("JobIndex")]
        public int JobIndex { get; set; }

        /// <summary>
        ///     Get or set the config index in the collection
        /// </summary>
        [Column("ConfigIndex")]
        public int ConfigIndex { get; set; }

        /// <summary>
        ///     Get or set the job collection index
        /// </summary>
        [Column("CollectionIndex")]
        public int CollectionIndex { get; set; }

        /// <summary>
        ///     Get or set the thermodynamic temperature
        /// </summary>
        [Column("Temperature")]
        public double Temperature { get; set; }

        /// <summary>
        ///     Get or set the electric field modulus in [V/m]
        /// </summary>
        [Column("ElectricFieldModulus")]
        public double ElectricFieldModulus { get; set; }

        /// <summary>
        ///     Get or set the base frequency in [Hz]
        /// </summary>
        [Column("BaseFrequency")]
        public double BaseFrequency { get; set; }

        /// <summary>
        ///     Get or set the main run target MCSP
        /// </summary>
        [Column("Mcsp")]
        public long Mcsp { get; set; }

        /// <summary>
        ///     Get or set the pre-run run target MCSP
        /// </summary>
        [Column("PreRunMcsp")]
        public long PreRunMcsp { get; set; }

        /// <summary>
        ///     Get or set the fixed manual normalization
        /// </summary>
        [Column("NormFactor")]
        public double NormalizationFactor { get; set; }

        /// <summary>
        ///     Get or set the time limit in seconds
        /// </summary>
        [Column("TimeLimit")]
        public long TimeLimit { get; set; }

        /// <summary>
        ///     Get or set the job flags
        /// </summary>
        [Column("Flags")]
        public string FlagString { get; set; }

        /// <summary>
        ///     Get or set a doping information <see cref="string"/>
        /// </summary>
        [Column("DopingInfo")]
        public string DopingInfo { get; set; }

        /// <summary>
        ///     Get or set a lattice information <see cref="string"/>
        /// </summary>
        [Column("LatticeInfo")]
        public string LatticeInfo { get; set; }
    }
}