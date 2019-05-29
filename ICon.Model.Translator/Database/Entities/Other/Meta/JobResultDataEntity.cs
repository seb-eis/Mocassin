using System.ComponentModel.DataAnnotations.Schema;

namespace Mocassin.Model.Translator.Database.Entities.Other.Meta
{
    /// <summary>
    ///     Entity that stores result data about <see cref="SimulationJobModel" /> entities to supply data for evaluation
    /// </summary>
    public class JobResultDataEntity : EntityBase
    {
        /// <summary>
        ///     Get or set the <see cref="SimulationJobModel" /> navigation property
        /// </summary>
        public SimulationJobModel JobModel { get; set; }

        /// <summary>
        ///     Get or set the affiliated <see cref="SimulationJobModel" /> id
        /// </summary>
        [Column("JobModelId")]
        [ForeignKey(nameof(JobModelId))]
        public int JobModelId { get; set; }

        /// <summary>
        ///     Get or set the binary representation of the final simulation state
        /// </summary>
        [Column("ResultState")]
        public byte[] SimulationStateBinary { get; set; }

        /// <summary>
        ///     Get or set the binary representation of the final simulation pre-run state
        /// </summary>
        [Column("PreRunState")]
        public byte[] SimulationPreRunStateBinary { get; set; }

        /// <summary>
        ///     Get or set the <see cref="string" /> that was written to stdout during simulation
        /// </summary>
        [Column("Stdout")]
        public string SimulationStdout { get; set; }
    }
}