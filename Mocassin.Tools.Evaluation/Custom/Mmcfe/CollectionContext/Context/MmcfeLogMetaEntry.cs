using System;
using System.ComponentModel.DataAnnotations.Schema;
using Mocassin.Model.Translator;
using Mocassin.Model.Translator.Database.Entities.Other.Meta;

namespace Mocassin.Tools.Evaluation.Custom.Mmcfe
{
    /// <summary>
    ///     Compatibility wrapper for <see cref="JobMetaDataEntity" /> and <see cref="MmcfeLogMetaEntry" />
    /// </summary>
    public class MmcfeLogMetaEntry : EntityBase, IJobMetaData
    {
        /// <summary>
        ///     The wrapped <see cref="JobMetaDataEntity" /> that stores the data
        /// </summary>
        [NotMapped]
        private JobMetaDataEntity JobMetaData { get; }

        /// <inheritdoc />
        public string CollectionName
        {
            get => JobMetaData.CollectionName;
            set => JobMetaData.CollectionName = value;
        }

        /// <inheritdoc />
        public string ConfigName
        {
            get => JobMetaData.ConfigName;
            set => JobMetaData.ConfigName = value;
        }

        /// <inheritdoc />
        public int JobIndex
        {
            get => JobMetaData.JobIndex;
            set => JobMetaData.JobIndex = value;
        }

        /// <inheritdoc />
        public int ConfigIndex
        {
            get => JobMetaData.ConfigIndex;
            set => JobMetaData.ConfigIndex = value;
        }

        /// <inheritdoc />
        public int CollectionIndex
        {
            get => JobMetaData.CollectionIndex;
            set => JobMetaData.CollectionIndex = value;
        }

        /// <inheritdoc />
        public double Temperature
        {
            get => JobMetaData.Temperature;
            set => JobMetaData.Temperature = value;
        }

        /// <inheritdoc />
        public double ElectricFieldModulus
        {
            get => JobMetaData.ElectricFieldModulus;
            set => JobMetaData.ElectricFieldModulus = value;
        }

        /// <inheritdoc />
        public double BaseFrequency
        {
            get => JobMetaData.BaseFrequency;
            set => JobMetaData.BaseFrequency = value;
        }

        /// <inheritdoc />
        public long Mcsp
        {
            get => JobMetaData.Mcsp;
            set => JobMetaData.Mcsp = value;
        }

        /// <inheritdoc />
        public long PreRunMcsp
        {
            get => JobMetaData.PreRunMcsp;
            set => JobMetaData.PreRunMcsp = value;
        }

        /// <inheritdoc />
        public double NormalizationFactor
        {
            get => JobMetaData.NormalizationFactor;
            set => JobMetaData.NormalizationFactor = value;
        }

        /// <inheritdoc />
        public long TimeLimit
        {
            get => JobMetaData.TimeLimit;
            set => JobMetaData.TimeLimit = value;
        }

        /// <inheritdoc />
        public string FlagString
        {
            get => JobMetaData.FlagString;
            set => JobMetaData.FlagString = value;
        }

        /// <inheritdoc />
        public string DopingInfo
        {
            get => JobMetaData.DopingInfo;
            set => JobMetaData.DopingInfo = value;
        }

        /// <inheritdoc />
        public string LatticeInfo
        {
            get => JobMetaData.LatticeInfo;
            set => JobMetaData.LatticeInfo = value;
        }

        /// <summary>
        ///     Get or set a  <see cref="string" /> that describes the particle counts for each ID (comma separated)
        /// </summary>
        public string ParticleCountInfo { get; set; }

        /// <summary>
        ///     Creates new empty <see cref="MmcfeLogMetaEntry" />
        /// </summary>
        public MmcfeLogMetaEntry()
        {
            JobMetaData = new JobMetaDataEntity();
        }

        /// <summary>
        ///     Creates new <see cref="MmcfeLogMetaEntry" /> that warps the provided <see cref="JobMetaDataEntity" />
        /// </summary>
        public MmcfeLogMetaEntry(JobMetaDataEntity jobMetaData)
        {
            JobMetaData = jobMetaData ?? throw new ArgumentNullException(nameof(jobMetaData));
        }
    }
}