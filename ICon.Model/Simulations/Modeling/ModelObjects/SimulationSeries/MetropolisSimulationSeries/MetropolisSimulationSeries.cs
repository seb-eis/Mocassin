using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using ICon.Framework.Constraints;
using ICon.Model.Basic;

namespace ICon.Model.Simulations
{
    /// <summary>
    /// Implementation of a specialized simulation series that defines a set of metropolis simulations
    /// </summary>
    [DataContract]
    public class MetropolisSimulationSeries : SimulationSeriesBase, IMetropolisSimulationSeries
    {
        /// <summary>
        /// The value series for the break tolerance of the simulation series
        /// </summary>
        [DataMember]
        public IValueSeries BreakToleranceSeries { get; set; }

        /// <summary>
        /// The value series for the break sample length of the simulation series
        /// </summary>
        [DataMember]
        public IValueSeries BreakSampleLengthSeries { get; set; }

        /// <summary>
        /// The value series for the break sample interval of the simulation series
        /// </summary>
        [DataMember]
        public IValueSeries BreakSampleIntervalSeries { get; set; }

        /// <summary>
        /// The value series ofr the result sample length of the simulation series
        /// </summary>
        [DataMember]
        public IValueSeries ResultSampleLengthSeries { get; set; }

        /// <summary>
        /// Interface access to the metropolis base simulation of the series
        /// </summary>
        [DataMember]
        public new IMetropolisSimulation BaseSimulation { get; set; }

        /// <summary>
        /// Get a string representing the model object name
        /// </summary>
        /// <returns></returns>
        public override string GetModelObjectName()
        {
            return "'Metropolis Simulation Series'";
        }

        /// <summary>
        /// Populates this object from a model object interface and retruns it. Returns null if the population fails
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override ModelObject PopulateFrom(IModelObject obj)
        {
            if (CastWithDepricatedCheck<IMetropolisSimulationSeries>(base.PopulateFrom(obj)) is IMetropolisSimulationSeries series)
            {
                BreakToleranceSeries = series.BreakToleranceSeries;
                BreakSampleLengthSeries = series.BreakSampleLengthSeries;
                BreakSampleIntervalSeries = series.BreakSampleIntervalSeries;
                ResultSampleLengthSeries = series.ResultSampleLengthSeries;
                return this;
            }
            return null;
        }
    }
}
