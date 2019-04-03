using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Reflection;
using Mocassin.Framework.Constraints;
using Mocassin.Framework.Provider;
using Mocassin.Model.Basic;
using Mocassin.Mathematics.ValueTypes;
using Mocassin.Model.Lattices;

namespace Mocassin.Model.Simulations
{
    /// <summary>
    /// Absttract base class for implementations of specialized simulation series model objects that define a set of simulations
    /// </summary>
    [DataContract]
    public abstract class SimulationSeriesBase : ModelObject, ISimulationSeriesBase
    {
        /// <summary>
        /// Get read only interface access to the energy background provider laod info list
        /// </summary>
        IReadOnlyList<IExternalLoadInfo> ISimulationSeriesBase.EnergyBackgroundLoadInfos => EnergyBackgroundLoadInfos;

        /// <summary>
        /// The unspecififed base simulation affiliated with this series
        /// </summary>
        [DataMember]
        [LinkableByIndex]
        public ISimulationBase BaseSimulation { get; set; }

        /// <summary>
        /// The user given series indetifier string
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// The value series for the simulation temperature parameter
        /// </summary>
        [DataMember]
        public IValueSeries TemperatureSeries { get; set; }

        /// <summary>
        /// The value series for the simulation monte carlo steps per particle
        /// </summary>
        [DataMember]
        public IValueSeries McspSeries { get; set; }

        /// <summary>
        /// The list of energy background load informations used to create energy background providers for the simulations
        /// </summary>
        [DataMember]
        public List<ExternalLoadInfo> EnergyBackgroundLoadInfos { get; set; }

        /// <summary>
        /// The list of lattice sizes used for lattice creation
        /// </summary>
        public List<VectorInt3D> LatticeSizeSeries { get; set; }

        /// <summary>
        /// Series of doping concentration
        /// </summary>
        public List<IDictionary<IDoping, double>> DopingSeries { get; set; }

        /// <summary>
        /// Populates the components of the base class from a model object interface and retunrs this object. Returns null if the population failed
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override ModelObject PopulateFrom(IModelObject obj)
        {
            if (CastWithDepricatedCheck<ISimulationSeriesBase>(obj) is ISimulationSeriesBase series)
            {
                Name = series.Name;
                TemperatureSeries = series.TemperatureSeries;
                McspSeries = series.McspSeries;
                EnergyBackgroundLoadInfos = series.EnergyBackgroundLoadInfos.Select(a => new ExternalLoadInfo(a)).ToList();
                return this;
            }
            return null;
        }

        /// <summary>
        /// Get the number of simulations described by this series
        /// </summary>
        /// <returns></returns>
        public long GetSimulationCount()
        {
            long count = BaseSimulation.JobCount;
            var properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var item in properties.Select(a => a.GetValue(this)).Where(value => value != null))
            {
                if (item is IValueSeries series)
                {
                    count *= series.GetValueCount();
                }
                if (item is IList list)
                {
                    count *= list.Count;
                }
            }
            return count;
        }
    }
}
