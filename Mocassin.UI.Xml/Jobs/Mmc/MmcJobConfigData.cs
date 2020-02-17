using System;
using System.Xml.Serialization;
using Mocassin.Model.Simulations;
using Mocassin.Model.Translator.Jobs;
using Mocassin.UI.Xml.Base;

namespace Mocassin.UI.Xml.Jobs
{
    /// <summary>
    ///     Serializable data object to store and create <see cref="MmcJobConfiguration" /> objects for the database creation
    ///     system
    /// </summary>
    [XmlRoot]
    public class MmcJobConfigData : JobConfigData, IDuplicable<MmcJobConfigData>
    {
        private string breakSampleInterval;
        private string breakSampleLength;
        private string breakTolerance;
        private string resultSampleMcs;

        /// <summary>
        ///     Get or set the abort tolerance value as a string
        /// </summary>
        [XmlAttribute]
        public string BreakTolerance
        {
            get => breakTolerance;
            set => SetProperty(ref breakTolerance, value);
        }

        /// <summary>
        ///     Get or set the abort sequence length as a string
        /// </summary>
        [XmlAttribute]
        public string BreakSampleLength
        {
            get => breakSampleLength;
            set => SetProperty(ref breakSampleLength, value);
        }

        /// <summary>
        ///     Get or set the abort sample interval as a string
        /// </summary>
        [XmlAttribute]
        public string BreakSampleInterval
        {
            get => breakSampleInterval;
            set => SetProperty(ref breakSampleInterval, value);
        }

        /// <summary>
        ///     Get or set the result sample mcs as a string
        /// </summary>
        [XmlAttribute]
        public string ResultSampleMcs
        {
            get => resultSampleMcs;
            set => SetProperty(ref resultSampleMcs, value);
        }

        /// <inheritdoc />
        public MmcJobConfigData Duplicate()
        {
            var result = new MmcJobConfigData
            {
                BreakTolerance = BreakTolerance,
                BreakSampleInterval = BreakSampleInterval,
                BreakSampleLength = BreakSampleLength,
                ResultSampleMcs = ResultSampleMcs
            };
            CopyBaseDataTo(result);
            return result;
        }

        /// <inheritdoc />
        object IDuplicable.Duplicate()
        {
            return Duplicate();
        }

        /// <inheritdoc />
        protected override JobConfiguration GetPreparedInternal(ISimulation baseSimulation)
        {
            if (!(baseSimulation is IMetropolisSimulation mmcBaseSimulation))
                throw new ArgumentException("Type of base simulation is incompatible", nameof(baseSimulation));

            var obj = new MmcJobConfiguration
            {
                AbortTolerance = string.IsNullOrWhiteSpace(BreakTolerance)
                    ? mmcBaseSimulation.RelativeBreakTolerance
                    : double.Parse(BreakTolerance),

                AbortSampleLength = string.IsNullOrWhiteSpace(BreakSampleLength)
                    ? mmcBaseSimulation.BreakSampleLength
                    : int.Parse(BreakSampleLength),

                AbortSampleInterval = string.IsNullOrWhiteSpace(BreakSampleInterval)
                    ? mmcBaseSimulation.BreakSampleIntervalMcs
                    : int.Parse(BreakSampleInterval),

                AbortSequenceLength = string.IsNullOrWhiteSpace(ResultSampleMcs)
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
                AbortTolerance = string.IsNullOrWhiteSpace(BreakTolerance)
                    ? mmcJobConfiguration.AbortTolerance
                    : double.Parse(BreakTolerance),

                AbortSampleLength = string.IsNullOrWhiteSpace(BreakSampleLength)
                    ? mmcJobConfiguration.AbortSampleLength
                    : int.Parse(BreakSampleLength),

                AbortSampleInterval = string.IsNullOrWhiteSpace(BreakSampleInterval)
                    ? mmcJobConfiguration.AbortSampleInterval
                    : int.Parse(BreakSampleInterval),

                AbortSequenceLength = string.IsNullOrWhiteSpace(ResultSampleMcs)
                    ? mmcJobConfiguration.AbortSequenceLength
                    : int.Parse(ResultSampleMcs)
            };

            return obj;
        }
    }
}