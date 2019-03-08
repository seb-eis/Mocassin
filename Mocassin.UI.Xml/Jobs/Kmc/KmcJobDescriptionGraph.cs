using System;
using System.Linq;
using System.Xml.Serialization;
using Mocassin.Model.Simulations;
using Mocassin.Model.Translator.Jobs;

namespace Mocassin.UI.Xml.Jobs
{
    /// <summary>
    ///     Serializable data object to store and create <see cref="KmcJobConfiguration" /> objects for the database creation
    ///     system
    /// </summary>
    [XmlRoot("KmcJobConfig")]
    public class KmcJobDescriptionGraph : JobDescriptionGraph
    {
        /// <summary>
        ///     Get or set the pre run monte carlo steps per particle as a string
        /// </summary>
        [XmlAttribute("PrerunMcsp")]
        public string PreRunMcsp { get; set; }

        /// <summary>
        ///     Get or set the modulus of the electric field in [V/m] as a string
        /// </summary>
        [XmlAttribute("ElectricFieldModulus")]
        public string ElectricFieldModulus { get; set; }

        /// <summary>
        ///     Get or set the fixed normalization probability as a string
        /// </summary>
        [XmlAttribute("NormalizationProbability")]
        public string NormalizationProbability { get; set; }

        /// <summary>
        ///     Get or set the overwrite max attempt frequency in [Hz] as a string
        /// </summary>
        [XmlAttribute("MaxAttemptFrequency")]
        public string MaxAttemptFrequency { get; set; }

        /// <inheritdoc />
        protected override JobConfiguration GetPreparedInternal(ISimulation baseSimulation)
        {
            if (!(baseSimulation is IKineticSimulation kmcBaseSimulation))
                throw new ArgumentException("Type of base simulation is incompatible", nameof(baseSimulation));

            var obj = new KmcJobConfiguration
            {
                PreRunMcsp = PreRunMcsp is null 
                    ? kmcBaseSimulation.PreRunMcsp
                    : int.Parse(PreRunMcsp),

                BaseFrequency = MaxAttemptFrequency is null
                    ? kmcBaseSimulation.Transitions.SelectMany(x => x.GetTransitionRules()).Max(rule => rule.AttemptFrequency)
                    : double.Parse(MaxAttemptFrequency),

                FixedNormalizationFactor = NormalizationProbability is null
                    ? 1.0 / kmcBaseSimulation.NormalizationProbability
                    : 1.0 / double.Parse(NormalizationProbability),

                ElectricFieldModulus = ElectricFieldModulus is null
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
                PreRunMcsp = PreRunMcsp is null 
                    ? kmcBaseConfig.PreRunMcsp
                    : int.Parse(PreRunMcsp),

                BaseFrequency = MaxAttemptFrequency is null
                    ? kmcBaseConfig.BaseFrequency
                    : double.Parse(MaxAttemptFrequency),

                FixedNormalizationFactor = NormalizationProbability is null
                    ? kmcBaseConfig.FixedNormalizationFactor
                    : 1.0 / double.Parse(NormalizationProbability),

                ElectricFieldModulus = ElectricFieldModulus is null
                    ? kmcBaseConfig.ElectricFieldModulus
                    : double.Parse(ElectricFieldModulus)
            };

            return obj;
        }
    }
}