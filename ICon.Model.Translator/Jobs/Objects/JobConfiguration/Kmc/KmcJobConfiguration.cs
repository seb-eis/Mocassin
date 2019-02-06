namespace Mocassin.Model.Translator.Jobs
{
    /// <summary>
    ///     A kinetic monte carlo job configuration fo a single simulation
    /// </summary>
    public class KmcJobConfiguration : JobConfiguration
    {
        /// <summary>
        ///     Get or set the modulus of the electric field in [V/m]
        /// </summary>
        public double ElectricFieldModulus { get; set; }

        /// <summary>
        ///     Get or set the fixed normalization factor
        /// </summary>
        public double FixedNormalizationFactor { get; set; }

        /// <summary>
        ///     Get or set the base attempt frequency
        /// </summary>
        public double BaseFrequency { get; set; }

        /// <summary>
        ///     Creates new configuration with a norm factor of 1.0
        /// </summary>
        public KmcJobConfiguration()
        {
            FixedNormalizationFactor = 1.0;
        }

        /// <summary>
        ///     Copies the data to another kmc job configuration
        /// </summary>
        /// <param name="jobConfiguration"></param>
        public void CopyTo(KmcJobConfiguration jobConfiguration)
        {
            base.CopyTo(jobConfiguration);
            jobConfiguration.BaseFrequency = BaseFrequency;
            jobConfiguration.ElectricFieldModulus = ElectricFieldModulus;
            jobConfiguration.FixedNormalizationFactor = FixedNormalizationFactor;
        }

        /// <inheritdoc />
        public override InteropObject GetInteropJobHeader()
        {
            return new InteropObject<CKmcJobHeader>
            {
                Structure = new CKmcJobHeader
                {
                    BaseFrequency = BaseFrequency,
                    FixedNormalizationFactor = FixedNormalizationFactor,
                    ElectricFieldModulus = ElectricFieldModulus,
                    JobFlags = default
                }
            };
        }

        /// <inheritdoc />
        public override JobConfiguration DeepCopy()
        {
            var result = new KmcJobConfiguration();
            CopyTo(result);
            return result;
        }
    }
}