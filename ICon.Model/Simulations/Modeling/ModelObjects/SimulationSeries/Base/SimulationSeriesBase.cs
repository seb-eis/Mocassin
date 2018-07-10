using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using ICon.Framework.Constraints;
using ICon.Framework.Provider;
using ICon.Model.Basic;

namespace ICon.Model.Simulations
{
    /// <summary>
    /// Absttract base class for implementations of specialized simulation series model objects that define a set of simulations
    /// </summary>
    [DataContract]
    public abstract class SimulationSeriesBase : ModelObject, ISimulationSeriesBase
    {
        /// <summary>
        /// Get read only interface access to the energy file path list
        /// </summary>
        IReadOnlyList<string> ISimulationSeriesBase.EnergyFilepathSeries => EnergyFilepathSeries;

        /// <summary>
        /// Get read only interface access to the doping series list
        /// </summary>
        IReadOnlyList<IDopingSeries> ISimulationSeriesBase.DopingSeriesList => DopingSeriesList;

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
        public string Description { get; set; }

        /// <summary>
        /// Boolean flag taht specifies if the simulation should use the custom defined lattice or generate a fully randomized one
        /// </summary>
        [DataMember]
        public bool UseCustomBaseLattice { get; set; }

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
        /// The value series for the number of unit cells in A direction
        /// </summary>
        [DataMember]
        public IValueSeries LatticeSizeSeriesA { get; set; }

        /// <summary>
        /// The value series for the number of unit cells in B direction
        /// </summary>
        [DataMember]
        public IValueSeries LatticeSizeSeriesB { get; set; }

        /// <summary>
        /// The value series for the number of unit cells in C direction
        /// </summary>
        [DataMember]
        public IValueSeries LatticeSizeSeriesC { get; set; }

        /// <summary>
        /// The file path string series to load predfined energy datasets not currently loaded in the energy management system
        /// </summary>
        [DataMember]
        public List<string> EnergyFilepathSeries { get; set; }

        /// <summary>
        /// The list of values series for doping the lattice
        /// </summary>
        [DataMember]
        [LinkableByIndex(LinkableType =LinkableType.Content)]
        public List<IDopingSeries> DopingSeriesList { get; set; }

        /// <summary>
        /// The list of energy background load informations used to create energy background providers for the simulations
        /// </summary>
        [DataMember]
        public List<ExternalLoadInfo> EnergyBackgroundLoadInfos { get; set; }

        /// <summary>
        /// Populates the components of the base class from a model object interface and retunrs this object. Returns null if the population failed
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override ModelObject PopulateFrom(IModelObject obj)
        {
            if (CastWithDepricatedCheck<ISimulationSeriesBase>(obj) is ISimulationSeriesBase series)
            {
                UseCustomBaseLattice = series.UseCustomBaseLattice;
                Description = series.Description;
                TemperatureSeries = series.TemperatureSeries;
                McspSeries = series.McspSeries;
                LatticeSizeSeriesA = series.LatticeSizeSeriesA;
                LatticeSizeSeriesB = series.LatticeSizeSeriesB;
                LatticeSizeSeriesC = series.LatticeSizeSeriesC;
                EnergyFilepathSeries = series.EnergyFilepathSeries.ToList();
                DopingSeriesList = series.DopingSeriesList.ToList();
                EnergyBackgroundLoadInfos = series.EnergyBackgroundLoadInfos.Select(a => new ExternalLoadInfo(a)).ToList();
                return this;
            }
            return null;
        }
    }
}
