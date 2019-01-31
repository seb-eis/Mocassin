using System;
using System.Linq;
using System.Xml.Serialization;
using Mocassin.Model.Simulations;
using Mocassin.Model.Translator.Jobs;

namespace Mocassin.UI.Xml.JobCreationData
{
    /// <summary>
    ///     Serializable data object to store and create <see cref="MmcJobConfiguration" /> objects for the database creation
    ///     system
    /// </summary>
    [XmlRoot("MmcJobConfig")]
    public class XmlMmcJobConfiguration : XmlJobConfiguration
    {
        /// <summary>
        ///     Get or set the abort tolerance value as a string
        /// </summary>
        [XmlAttribute("BreakTolerance")]
        public string BreakTolerance { get; set; }

        /// <summary>
        ///     Get or set the abort sequence length as a string
        /// </summary>
        [XmlAttribute("BreakSampleLength")]
        public string BreakSampleLength { get; set; }

        /// <summary>
        ///     Get or set the abort sample interval as a string
        /// </summary>
        [XmlAttribute("BreakSampleInterval")]
        public string BreakSampleInterval { get; set; }

        /// <summary>
        ///     Get or set the result sample mcs as a string
        /// </summary>
        [XmlAttribute("ResultSampleMcs")]
        public string ResultSampleMcs { get; set; }

        /// <inheritdoc />
        protected override JobConfiguration GetPreparedInternal(ISimulation baseSimulation)
        {
            if (!(baseSimulation is IMetropolisSimulation mmcBaseSimulation))
                throw new ArgumentException("Type of base simulation is incompatible", nameof(baseSimulation));

            var obj = new MmcJobConfiguration
            {
                AbortTolerance = BreakTolerance is null 
                    ? mmcBaseSimulation.RelativeBreakTolerance 
                    : double.Parse(BreakTolerance),

                AbortSampleLength = BreakSampleLength is null
                    ? mmcBaseSimulation.BreakSampleLength
                    : int.Parse(BreakSampleLength),

                AbortSampleInterval = BreakSampleInterval is null 
                    ? mmcBaseSimulation.BreakSampleIntervalMcs
                    : int.Parse(BreakSampleInterval),

                AbortSequenceLength = ResultSampleMcs is null 
                    ? mmcBaseSimulation.ResultSampleMcs
                    : int.Parse(ResultSampleMcs)
            };

            return obj;
        }

        /// <inheritdoc />
        protected override JobConfiguration GetPreparedInternal(JobConfiguration baseConfiguration)
        {
            if (!(baseConfiguration is MmcJobConfiguration mmcJobConfiguration))
                throw new ArgumentException("Type of base configuration is incompatible", nameof(baseConfiguration));

            var obj = new MmcJobConfiguration
            {
                AbortTolerance = BreakTolerance is null 
                    ? mmcJobConfiguration.AbortTolerance 
                    : double.Parse(BreakTolerance),

                AbortSampleLength = BreakSampleLength is null
                    ? mmcJobConfiguration.AbortSampleLength
                    : int.Parse(BreakSampleLength),

                AbortSampleInterval = BreakSampleInterval is null 
                    ? mmcJobConfiguration.AbortSampleInterval
                    : int.Parse(BreakSampleInterval),

                AbortSequenceLength = ResultSampleMcs is null 
                    ? mmcJobConfiguration.AbortSequenceLength
                    : int.Parse(ResultSampleMcs)
            };

            return obj;
        }
    }
}