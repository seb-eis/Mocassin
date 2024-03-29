﻿using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Translator.Jobs
{
    /// <summary>
    ///     Abstract base class for job configuration implementations that holds common job information
    /// </summary>
    public abstract class JobConfiguration
    {
        /// <summary>
        ///     Get or set additional job info flags
        /// </summary>
        public SimulationExecutionFlags ExecutionFlags { get; set; }

        /// <summary>
        ///     Get or set the target mcsp of the job
        /// </summary>
        public long TargetMcsp { get; set; }

        /// <summary>
        ///     Get or set the time limit of the simulation
        /// </summary>
        public long TimeLimit { get; set; }

        /// <summary>
        ///     Get or set the random number generator state seed
        /// </summary>
        public long RngStateSeed { get; set; }

        /// <summary>
        ///     Get or set the random number generator increase seed
        /// </summary>
        public long RngIncreaseSeed { get; set; }

        /// <summary>
        ///     Get or set the temperature value in [K]
        /// </summary>
        public double Temperature { get; set; }

        /// <summary>
        ///     Get or set the minimal success rate
        /// </summary>
        public double MinimalSuccessRate { get; set; }

        /// <summary>
        ///     Get or set the job lattice configuration
        /// </summary>
        public LatticeConfiguration LatticeConfiguration { get; set; }

        /// <summary>
        ///     Get or set the local index for the job in the config
        /// </summary>
        public int JobIndex { get; set; }

        /// <summary>
        ///     Get or set the index for the parent config
        /// </summary>
        public int ConfigIndex { get; set; }

        /// <summary>
        ///     Get or set the source job collection name
        /// </summary>
        public string CollectionName { get; set; }

        /// <summary>
        ///     Get or set the source configuration name
        /// </summary>
        public string ConfigName { get; set; }

        /// <summary>
        ///     Get or set the attached instruction string
        /// </summary>
        public string Instruction { get; set; }

        /// <summary>
        ///     Copies all data to the passed job configuration
        /// </summary>
        /// <param name="jobConfiguration"></param>
        public void CopyTo(JobConfiguration jobConfiguration)
        {
            jobConfiguration.LatticeConfiguration ??= new LatticeConfiguration();

            LatticeConfiguration.CopyTo(jobConfiguration.LatticeConfiguration);
            jobConfiguration.ExecutionFlags = ExecutionFlags;
            jobConfiguration.JobIndex = JobIndex;
            jobConfiguration.ConfigIndex = ConfigIndex;
            jobConfiguration.MinimalSuccessRate = MinimalSuccessRate;
            jobConfiguration.RngIncreaseSeed = RngIncreaseSeed;
            jobConfiguration.RngStateSeed = RngStateSeed;
            jobConfiguration.TargetMcsp = TargetMcsp;
            jobConfiguration.Temperature = Temperature;
            jobConfiguration.TimeLimit = TimeLimit;
            jobConfiguration.ConfigName = ConfigName;
            jobConfiguration.CollectionName = CollectionName;
            jobConfiguration.Instruction = Instruction;
        }

        /// <summary>
        ///     Get the job configuration as an interop object
        /// </summary>
        /// <returns></returns>
        public InteropObject<CJobInfo> GetInteropJobInfo() =>
            new InteropObject<CJobInfo>
            {
                Structure = new CJobInfo
                {
                    JobFlags = (long) ExecutionFlags,
                    MinimalSuccessRate = MinimalSuccessRate,
                    ObjectId = JobIndex,
                    RngIncreaseSeed = RngIncreaseSeed,
                    RngStateSeed = RngStateSeed,
                    StatusFlags = default,
                    TargetMcsp = TargetMcsp,
                    Temperature = Temperature,
                    TimeLimit = TimeLimit
                }
            };

        /// <summary>
        ///     Get the job header as an interop object using the provided <see cref="MocassinConstantsSettings" />
        /// </summary>
        /// <param name="constantsSettings"></param>
        /// <returns></returns>
        public abstract InteropObject GetInteropJobHeader(MocassinConstantsSettings constantsSettings);

        /// <summary>
        ///     Creates a deep copy of the <see cref="JobConfiguration" />
        /// </summary>
        /// <returns></returns>
        public abstract JobConfiguration DeepCopy();
    }
}