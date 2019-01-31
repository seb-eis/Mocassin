namespace Mocassin.Model.Translator.Jobs
{
    /// <summary>
    ///     A metropolis monte carlo job configuration fo a single simulation
    /// </summary>
    public class MmcJobConfiguration : JobConfiguration
    {
        /// <summary>
        ///     Get or set the abort tolerance value
        /// </summary>
        public double AbortTolerance { get; set; }

        /// <summary>
        ///     Get or set the abort sequence length
        /// </summary>
        public int AbortSequenceLength { get; set; }

        /// <summary>
        ///     Gte or set the abort sample length
        /// </summary>
        public int AbortSampleLength { get; set; }

        /// <summary>
        ///     Get or set the abort sample interval
        /// </summary>
        public int AbortSampleInterval { get; set; }

        /// <summary>
        /// Copies the data to another mmc job configuration
        /// </summary>
        /// <param name="jobConfiguration"></param>
        public void CopyTo(MmcJobConfiguration jobConfiguration)
        {
            base.CopyTo(jobConfiguration);
            jobConfiguration.AbortSampleInterval = AbortSampleInterval;
            jobConfiguration.AbortSampleLength = AbortSampleLength;
            jobConfiguration.AbortSequenceLength = AbortSequenceLength;
            jobConfiguration.AbortTolerance = AbortTolerance;
        }

        /// <inheritdoc />
        public override InteropObject GetInteropJobHeader()
        {
            return new InteropObject<CMmcJobHeader>
            {
                Structure = new CMmcJobHeader
                {
                    AbortSampleInterval = AbortSampleInterval,
                    AbortSampleLength = AbortSampleLength,
                    AbortSequenceLength = AbortSequenceLength,
                    AbortTolerance = AbortTolerance,
                    JobFlags = default
                }
            };
        }
    }
}