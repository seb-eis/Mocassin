using System;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Translator.Jobs
{
    /// <summary>
    ///     A kinetic monte carlo job configuration for a single simulation
    /// </summary>
    public class KmcJobConfiguration : JobConfiguration
    {
        /// <summary>
        ///     Get or set the pre run mcsp of the simulation
        /// </summary>
        public int PreRunMcsp { get; set; }

        /// <summary>
        ///     Get or set the modulus of the electric field in [V/m]
        /// </summary>
        public double ElectricFieldModulus { get; set; }

        /// <summary>
        ///     Get or set the fixed normalization energy
        /// </summary>
        public double NormalizationEnergy { get; set; }

        /// <summary>
        ///     Get or set the base attempt frequency
        /// </summary>
        public double BaseFrequency { get; set; }

        /// <summary>
        ///     Creates new configuration with a norm factor of 1.0
        /// </summary>
        public KmcJobConfiguration()
        {
            NormalizationEnergy = 0;
        }

        /// <summary>
        ///     Copies the data to another kmc job configuration
        /// </summary>
        /// <param name="jobConfiguration"></param>
        public void CopyTo(KmcJobConfiguration jobConfiguration)
        {
            base.CopyTo(jobConfiguration);
            jobConfiguration.PreRunMcsp = PreRunMcsp;
            jobConfiguration.BaseFrequency = BaseFrequency;
            jobConfiguration.ElectricFieldModulus = ElectricFieldModulus;
            jobConfiguration.NormalizationEnergy = NormalizationEnergy;
        }

        /// <inheritdoc />
        public override InteropObject GetInteropJobHeader(MocassinConstantsSettings constantsSettings) =>
            new InteropObject<CKmcJobHeader>
            {
                Structure = new CKmcJobHeader
                {
                    PreRunMcsp = PreRunMcsp,
                    BaseFrequency = BaseFrequency,
                    FixedNormalizationFactor = GetFixedNormalizationFactor(constantsSettings),
                    ElectricFieldModulus = ElectricFieldModulus,
                    JobFlags = default
                }
            };

        /// <inheritdoc />
        public override JobConfiguration DeepCopy()
        {
            var result = new KmcJobConfiguration();
            CopyTo(result);
            return result;
        }

        /// <summary>
        ///     Converts the set normalization energy to a normalization factor as required by the simulation (=1.0 / exp(-E / (k * T)))
        /// </summary>
        /// <param name="constantsSettings"></param>
        /// <returns></returns>
        public double GetFixedNormalizationFactor(MocassinConstantsSettings constantsSettings)
        {
            var kbInEv = constantsSettings.BoltzmannConstantSi / constantsSettings.ElementalChargeSi;
            return 1.0 / Math.Exp(-NormalizationEnergy / (kbInEv * Temperature));
        }
    }
}