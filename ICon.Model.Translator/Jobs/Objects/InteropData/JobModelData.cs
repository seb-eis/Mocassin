using System;

namespace Mocassin.Model.Translator.Jobs
{
    /// <summary>
    ///     Job model interop data for a single simulation job that can be stored in the simulation database context
    /// </summary>
    public readonly struct JobModelData
    {
        /// <summary>
        ///     The job info interop object
        /// </summary>
        public readonly InteropObject<CJobInfo> JobInfo;

        /// <summary>
        ///     The job header interop object
        /// </summary>
        public readonly InteropObject JobHeader;

        /// <summary>
        ///     Creates new job interop data from job info and jobHeader
        /// </summary>
        /// <param name="jobInfo"></param>
        /// <param name="jobHeader"></param>
        public JobModelData(InteropObject<CJobInfo> jobInfo, InteropObject jobHeader)
        {
            JobInfo = jobInfo ?? throw new ArgumentNullException(nameof(jobInfo));
            JobHeader = jobHeader ?? throw new ArgumentNullException(nameof(jobHeader));
        }
    }
}