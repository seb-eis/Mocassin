using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using ICon.Framework.Constraints;
using ICon.Model.Basic;

namespace ICon.Model.Simulations
{
    /// <inheritdoc cref="ICon.Model.Simulations.IMetropolisSimulationSeries"/>
    [DataContract]
    public class MetropolisSimulationSeries : SimulationSeriesBase, IMetropolisSimulationSeries
    {
        /// <inheritdoc />
        [DataMember]
        public IValueSeries BreakToleranceSeries { get; set; }

        /// <inheritdoc />
        [DataMember]
        public IValueSeries BreakSampleLengthSeries { get; set; }

        /// <inheritdoc />
        [DataMember]
        public IValueSeries BreakSampleIntervalSeries { get; set; }

        /// <inheritdoc />
        [DataMember]
        public IValueSeries ResultSampleLengthSeries { get; set; }

        /// <inheritdoc />
        [DataMember]
        public new IMetropolisSimulation BaseSimulation { get; set; }

        /// <inheritdoc />
        public override string GetObjectName()
        {
            return "'Metropolis Simulation Series'";
        }

        /// <inheritdoc />
        public override ModelObject PopulateFrom(IModelObject obj)
        {
            if (!(CastIfNotDeprecated<IMetropolisSimulationSeries>(obj) is IMetropolisSimulationSeries series))
                return null;

            base.PopulateFrom(obj);
            BreakToleranceSeries = series.BreakToleranceSeries;
            BreakSampleLengthSeries = series.BreakSampleLengthSeries;
            BreakSampleIntervalSeries = series.BreakSampleIntervalSeries;
            ResultSampleLengthSeries = series.ResultSampleLengthSeries;
            return this;
        }
    }
}
