using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Reflection;
using Mocassin.Framework.Constraints;
using Mocassin.Framework.Provider;
using Mocassin.Model.Basic;

namespace Mocassin.Model.Simulations
{
    /// <inheritdoc cref="ISimulationSeries"/>
    /// <remarks> Abstract base class for implementations of simulation series objects </remarks>
    [DataContract]
    public abstract class SimulationSeriesBase : ModelObject, ISimulationSeries
    {
        /// <inheritdoc />
        [IgnoreDataMember]
        IReadOnlyList<IExternalLoadInfo> ISimulationSeries.EnergyBackgroundLoadInfos => EnergyBackgroundLoadInfos;

        /// <inheritdoc />
        [DataMember]
        [IndexResolved]
        public ISimulation BaseSimulation { get; set; }

        /// <inheritdoc />
        [DataMember]
        public string Name { get; set; }

        /// <inheritdoc />
        [DataMember]
        public IValueSeries TemperatureSeries { get; set; }

        /// <inheritdoc />
        [DataMember]
        public IValueSeries McspSeries { get; set; }

        /// <summary>
        /// The list of energy background load information used to create energy background providers for the simulations
        /// </summary>
        [DataMember]
        public List<ExternalLoadInfo> EnergyBackgroundLoadInfos { get; set; }

        public override ModelObject PopulateFrom(IModelObject obj)
        {
            if (!(CastIfNotDeprecated<ISimulationSeries>(obj) is ISimulationSeries series))
                return null;

            Name = series.Name;
            TemperatureSeries = series.TemperatureSeries;
            McspSeries = series.McspSeries;
            EnergyBackgroundLoadInfos = series.EnergyBackgroundLoadInfos.Select(a => new ExternalLoadInfo(a)).ToList();
            return this;
        }

        /// <inheritdoc />
        public long GetSimulationCount()
        {
            long count = BaseSimulation.JobCount;
            var properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var item in properties.Select(a => a.GetValue(this)).Where(value => value != null))
            {
                switch (item)
                {
                    case IValueSeries series:
                        count *= series.GetValueCount();
                        break;

                    case IList list:
                        count *= list.Count;
                        break;
                }
            }
            return count;
        }
    }
}
