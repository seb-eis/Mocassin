using System.ComponentModel.DataAnnotations.Schema;

namespace Mocassin.Model.Translator.Database.Entities.Other.Meta
{
    /// <summary>
    ///     Entity that stores meta data about <see cref="SimulationJobModel" /> entities to provide a simulation parameter to
    ///     job mapping in the simulation database
    /// </summary>
    public class JobMetaDataEntity : EntityBase, IJobMetaData
    {
        /// <summary>
        ///     Get or set the <see cref="SimulationJobModel" /> navigation entity
        /// </summary>
        public SimulationJobModel JobModel { get; set; }

        /// <summary>
        ///     Get or set the <see cref="SimulationJobModel" /> context id
        /// </summary>
        [Column("JobModelId"), ForeignKey(nameof(JobModel))]
        public int JobModelId { get; set; }

        /// <inheritdoc />
        [Column("CollectionName")]
        public string CollectionName { get; set; }

        /// <inheritdoc />
        [Column("ConfigName")]
        public string ConfigName { get; set; }

        /// <inheritdoc />
        [Column("JobIndex")]
        public int JobIndex { get; set; }

        /// <inheritdoc />
        [Column("ConfigIndex")]
        public int ConfigIndex { get; set; }

        /// <inheritdoc />
        [Column("CollectionIndex")]
        public int CollectionIndex { get; set; }

        /// <inheritdoc />
        [Column("Temperature")]
        public double Temperature { get; set; }

        /// <inheritdoc />
        [Column("ElectricFieldModulus")]
        public double ElectricFieldModulus { get; set; }

        /// <inheritdoc />
        [Column("BaseFrequency")]
        public double BaseFrequency { get; set; }

        /// <inheritdoc />
        [Column("Mcsp")]
        public long Mcsp { get; set; }

        /// <inheritdoc />
        [Column("PreRunMcsp")]
        public long PreRunMcsp { get; set; }

        /// <inheritdoc />
        [Column("NormFactor")]
        public double NormalizationFactor { get; set; }

        /// <inheritdoc />
        [Column("TimeLimit")]
        public long TimeLimit { get; set; }

        /// <inheritdoc />
        [Column("Flags")]
        public string FlagString { get; set; }

        /// <inheritdoc />
        [Column("DopingInfo")]
        public string DopingInfo { get; set; }

        /// <inheritdoc />
        [Column("LatticeInfo")]
        public string LatticeInfo { get; set; }
    }
}