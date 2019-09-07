using System;
using System.Linq;
using System.Xml.Serialization;
using Mocassin.Model.Simulations;
using Mocassin.Model.Translator.Jobs;
using Mocassin.UI.Xml.Base;

namespace Mocassin.UI.Xml.Jobs
{
    /// <summary>
    ///     Serializable data object to store and create <see cref="KmcJobConfiguration" /> objects for the database creation
    ///     system
    /// </summary>
    [XmlRoot("KmcJobConfig")]
    public class KmcJobDescriptionGraph : JobDescriptionGraph, IDuplicable<KmcJobDescriptionGraph>
    {
        private string preRunMcsp;
        private string electricFieldModulus;
        private string normalizationProbability;
        private string maxAttemptFrequency;

        /// <summary>
        ///     Get or set the pre run monte carlo steps per particle as a string
        /// </summary>
        [XmlAttribute("PrerunMcsp")]
        public string PreRunMcsp
        {
            get => preRunMcsp;
            set => SetProperty(ref preRunMcsp, value);
        }

        /// <summary>
        ///     Get or set the modulus of the electric field in [V/m] as a string
        /// </summary>
        [XmlAttribute("ElectricFieldModulus")]
        public string ElectricFieldModulus
        {
            get => electricFieldModulus;
            set => SetProperty(ref electricFieldModulus, value);
        }

        /// <summary>
        ///     Get or set the fixed normalization probability as a string
        /// </summary>
        [XmlAttribute("NormalizationProbability")]
        public string NormalizationProbability
        {
            get => normalizationProbability;
            set => SetProperty(ref normalizationProbability, value);
        }

        /// <summary>
        ///     Get or set the overwrite max attempt frequency in [Hz] as a string
        /// </summary>
        [XmlAttribute("MaxAttemptFrequency")]
        public string MaxAttemptFrequency
        {
            get => maxAttemptFrequency;
            set => SetProperty(ref maxAttemptFrequency, value);
        }

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

                FixedNormalizationFactor = string.IsNullOrWhiteSpace(NormalizationProbability)
                    ? 1.0 / kmcBaseSimulation.NormalizationProbability
                    : 1.0 / double.Parse(NormalizationProbability),

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

                FixedNormalizationFactor = string.IsNullOrWhiteSpace(NormalizationProbability)
                    ? kmcBaseConfig.FixedNormalizationFactor
                    : 1.0 / double.Parse(NormalizationProbability),

                ElectricFieldModulus = string.IsNullOrWhiteSpace(ElectricFieldModulus)
                    ? kmcBaseConfig.ElectricFieldModulus
                    : double.Parse(ElectricFieldModulus)
            };

            return obj;
        }

        /// <inheritdoc />
        public KmcJobDescriptionGraph Duplicate()
        {
            var result = new KmcJobDescriptionGraph
            {
                ElectricFieldModulus = ElectricFieldModulus,
                PreRunMcsp = PreRunMcsp,
                NormalizationProbability = NormalizationProbability,
                MaxAttemptFrequency = MaxAttemptFrequency
            };
            CopyBaseDataTo(result);
            return result;
        }

        /// <inheritdoc />
        object IDuplicable.Duplicate()
        {
            return Duplicate();
        }
    }
}