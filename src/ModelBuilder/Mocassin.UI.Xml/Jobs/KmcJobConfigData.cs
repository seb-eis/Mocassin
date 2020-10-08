using System;
using System.Linq;
using System.Xml.Serialization;
using Mocassin.Model.Simulations;
using Mocassin.Model.Translator.Jobs;
using Mocassin.UI.Data.Base;

namespace Mocassin.UI.Data.Jobs
{
    /// <summary>
    ///     Serializable data object to store and create <see cref="KmcJobConfiguration" /> objects for the database creation
    ///     system
    /// </summary>
    [XmlRoot]
    public class KmcJobConfigData : JobConfigData, IDuplicable<KmcJobConfigData>
    {
        private string electricFieldModulus;
        private string maxAttemptFrequency;
        private string normalizationEnergy;
        private string preRunMcsp;

        /// <summary>
        ///     Get or set the pre run monte carlo steps per particle as a string
        /// </summary>
        [XmlAttribute]
        public string PreRunMcsp
        {
            get => preRunMcsp;
            set => SetProperty(ref preRunMcsp, value);
        }

        /// <summary>
        ///     Get or set the modulus of the electric field in [V/m] as a string
        /// </summary>
        [XmlAttribute]
        public string ElectricFieldModulus
        {
            get => electricFieldModulus;
            set => SetProperty(ref electricFieldModulus, value);
        }

        /// <summary>
        ///     Get or set the fixed normalization probability as a string
        /// </summary>
        [XmlAttribute]
        public string NormalizationEnergy
        {
            get => normalizationEnergy;
            set => SetProperty(ref normalizationEnergy, value);
        }

        /// <summary>
        ///     Get or set the overwrite max attempt frequency in [Hz] as a string
        /// </summary>
        [XmlAttribute]
        public string MaxAttemptFrequency
        {
            get => maxAttemptFrequency;
            set => SetProperty(ref maxAttemptFrequency, value);
        }

        /// <inheritdoc />
        public KmcJobConfigData Duplicate()
        {
            var result = new KmcJobConfigData
            {
                ElectricFieldModulus = ElectricFieldModulus,
                PreRunMcsp = PreRunMcsp,
                NormalizationEnergy = NormalizationEnergy,
                MaxAttemptFrequency = MaxAttemptFrequency
            };
            CopyBaseDataTo(result);
            return result;
        }

        /// <inheritdoc />
        object IDuplicable.Duplicate() => Duplicate();

        /// <inheritdoc />
        protected override JobConfiguration GetPreparedInternal(ISimulation baseSimulation)
        {
            if (!(baseSimulation is IKineticSimulation kmcBaseSimulation))
                throw new ArgumentException("Type of base simulation is incompatible", nameof(baseSimulation));

            var obj = new KmcJobConfiguration
            {
                PreRunMcsp = string.IsNullOrWhiteSpace(PreRunMcsp)
                    ? kmcBaseSimulation.PreRunMcsp
                    : int.Parse(PreRunMcsp),

                BaseFrequency = string.IsNullOrWhiteSpace(MaxAttemptFrequency)
                    ? kmcBaseSimulation.Transitions.SelectMany(x => x.GetTransitionRules()).Max(rule => rule.AttemptFrequency)
                    : double.Parse(MaxAttemptFrequency),

                NormalizationEnergy = string.IsNullOrWhiteSpace(NormalizationEnergy)
                    ? kmcBaseSimulation.NormalizationEnergy
                    : double.Parse(NormalizationEnergy),

                ElectricFieldModulus = string.IsNullOrWhiteSpace(ElectricFieldModulus)
                    ? kmcBaseSimulation.ElectricFieldMagnitude
                    : double.Parse(ElectricFieldModulus)
            };

            return obj;
        }

        /// <inheritdoc />
        protected override JobConfiguration GetPreparedInternal(JobConfiguration baseConfiguration)
        {
            if (!(baseConfiguration is KmcJobConfiguration kmcBaseConfig))
                throw new ArgumentException("Type of base configuration is incompatible", nameof(baseConfiguration));

            var obj = new KmcJobConfiguration
            {
                PreRunMcsp = string.IsNullOrWhiteSpace(PreRunMcsp)
                    ? kmcBaseConfig.PreRunMcsp
                    : int.Parse(PreRunMcsp),

                BaseFrequency = string.IsNullOrWhiteSpace(MaxAttemptFrequency)
                    ? kmcBaseConfig.BaseFrequency
                    : double.Parse(MaxAttemptFrequency),

                NormalizationEnergy = string.IsNullOrWhiteSpace(NormalizationEnergy)
                    ? kmcBaseConfig.NormalizationEnergy
                    : double.Parse(NormalizationEnergy),

                ElectricFieldModulus = string.IsNullOrWhiteSpace(ElectricFieldModulus)
                    ? kmcBaseConfig.ElectricFieldModulus
                    : double.Parse(ElectricFieldModulus)
            };

            return obj;
        }
    }
}