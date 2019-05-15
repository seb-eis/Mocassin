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
        public string JobCollectionName { get; set; }

        /// <summary>
        ///     Get or set the job configuration name
        /// </summary>
        [Column("ConfigName")]
        public string JobConfigName { get; set; }

        /// <summary>
        ///     Get or set the relative job index due to job multiplication
        /// </summary>
        [Column("JobIndex")]
        public int JobIndex { get; set; }

        /// <summary>
        ///     Get or set the thermodynamic temperature
        /// </summary>
        [Column("Temperature")]
        public double Temperature { get; set; }

        /// <summary>
        ///     Get or set the main run target MCSP
        /// </summary>
        [Column("Mcsp")]
        public long MainRunMcsp { get; set; }

        /// <summary>
        ///     Get or set the time limit in seconds
        /// </summary>
        [Column("TimeLimit")]
        public long TimeLimit { get; set; }

        /// <summary>
        ///     Get or set the job flags
        /// </summary>
        [Column("Flags")]
        public string FlagValues { get; set; }

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